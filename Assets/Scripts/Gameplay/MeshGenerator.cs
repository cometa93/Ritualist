using System.Collections;
using System.Collections.Generic;
using DevilMind;
using Ritualist;
using UnityEngine;

namespace DevilMind.Utils
{
    public class MeshGenerator : DevilBehaviour
    {
        private MeshFilter _meshFilter;
        private PolygonCollider2D _polygonCollider2D;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;
        
        [SerializeField] private Material _rendererMaterial;
        [SerializeField] private LayerMask _enemyLayer;

        [SerializeField] private List<Texture2D> _frames;
        [SerializeField] private int _framesPerSecond;

        private List<Vector2> _points;
        private float _counter;
        private int _currentFrameIndex;
        private float Factor { get { return _frames.Count != 0 ? 1f / _frames.Count: 1; } }
        protected override void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _polygonCollider2D.enabled = false;
            EventsToListen.Add(EventType.GenerateMagicField);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.GenerateMagicField)
            {
                SetupMagicField((List<RuneBehaviour>) gameEvent.Parameter);
            }
            base.OnEvent(gameEvent);
        }

        private void Update()
        {
            if (_frames.Count <= 0)
            {
                return;
            }
            _counter += Time.deltaTime;
            var progress = _counter/Factor;
            if (progress > 1f)
            {
                _counter = 0;
                _currentFrameIndex++;
            }
            if (_currentFrameIndex >= _frames.Count)
            {
                _currentFrameIndex = 0;
            }

            _meshRenderer.material.mainTexture = _frames[_currentFrameIndex];
        }

        private void DeactivateMagicField()
        {
            _polygonCollider2D.enabled = false;

            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("from", _meshRenderer.material.GetColor("_TintColor"));
            tweenParams.Add("to", Color.clear);
            tweenParams.Add("time", 0.3f);
            tweenParams.Add("onupdate", "OnColorUpdated");
            iTween.ValueTo(gameObject, tweenParams);

            GameplayController.Instance.RefillAmmo();
        }

        private List<Vector2> GetPoints(List<RuneBehaviour> runes)
        {
            var list = new List<Vector2>();
            for (int i = 0, c = runes.Count; i < c; ++i)
            {
                var rune = runes[i];
                if (rune != null)
                {
                    list.Add(rune.GetDockPosition());
                }
            }
            return list;
        }

        private void SetupMagicField(List<RuneBehaviour> magicFieldPoints)
        {
            _polygonCollider2D.enabled = true;
            _points = GetPoints(magicFieldPoints);
            _meshRenderer.enabled = true;

            Vector3[] vertices = new Vector3[4];
            int[] triangles = new int[6];
            Vector3[] normals = new Vector3[4];
            Vector2[] texturePoints = new Vector2[4];
            _polygonCollider2D.points = new Vector2[4];

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 3;
            triangles[3] = 1;
            triangles[4] = 2;
            triangles[5] = 3;


            for (int i = 0, c = normals.Length; i < c; ++i)
            {
                normals[i] = -Vector3.forward;
            }

            //Calculating vertices 
            vertices[0] = new Vector2(_points[0].x, _points[0].y);
            vertices[1] = new Vector2((_points[0].x + _points[1].x) / 2, (_points[0].y + _points[1].y) / 2);
            vertices[2] = new Vector2(_points[1].x, _points[1].y);
            vertices[3] = new Vector2(_points[2].x, _points[2].y);

            //setting uvs
            texturePoints[0] = new Vector2(0, 1);
            texturePoints[1] = new Vector2(1, 1);
            texturePoints[2] = new Vector2(1, 0);
            texturePoints[3] = new Vector2(0, 0);

            _mesh = _mesh ?? new Mesh();
            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.normals = normals;
            _mesh.triangles = triangles;
            _mesh.uv = texturePoints;
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();

            _meshFilter.mesh = _mesh;
            _meshRenderer.additionalVertexStreams = _mesh;
            _meshRenderer.material = _rendererMaterial;

            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("from", Color.clear);
            tweenParams.Add("to", Color.white);
            tweenParams.Add("time", 0.3f);
            tweenParams.Add("onupdate", "OnColorUpdated");
            tweenParams.Add("oncomplete", "SetupCollider");
            iTween.ValueTo(gameObject, tweenParams);
        }

        private void CheckForCatchedEnemies()
        {
            StartCoroutine(TimeHelper.RunAfterFrames(2, () =>
            {
                if (Physics2D.IsTouchingLayers(_polygonCollider2D, _enemyLayer) == false &&
                    _polygonCollider2D.IsTouchingLayers(_enemyLayer) == false)
                {
                    DeactivateMagicField();
                }
            }));
        }

        private void SetupCollider()
        {
            _polygonCollider2D.SetPath(0,_points.ToArray());
            CheckForCatchedEnemies();
            StartCoroutine(TimeHelper.RunAfterSeconds(3, DeactivateMagicField));
        }

        private void OnColorUpdated(Color color)
        {
            _meshRenderer.material.SetColor("_TintColor", color);
        }
    }
}