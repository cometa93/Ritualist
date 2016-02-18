using UnityEngine;
using System.Collections.Generic;

namespace DevilMind.Utils {
    public class TileableObject : MonoBehaviour
    {
        [SerializeField] private List<Sprite> PossibleTiles;
        [SerializeField] private float _edgeOffset;
        [SerializeField] private bool _deleteWhenFarAway = true;
        [SerializeField] private int _maximumDistance;
        [SerializeField] private Transform _prefabToSpawn;

        private float _leftEdge;
        private float _rightEdge;
        private float _spriteWidth;
        private Camera _mainCamera;

        public TileableObject RightBrother { get; set; }
        public TileableObject LeftBrother { get; set; } 
       
        public bool HasRightBrother { get { return RightBrother != null; }}
        public bool HasLeftBrother { get { return LeftBrother != null; } }

        protected void Awake()
        {
            _spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            _spriteWidth = _spriteWidth*transform.localScale.x;
            _mainCamera = Camera.main;  
        }

        public void SetRightBrother()   
        {
            if (HasRightBrother)
            {
                return;
            }
            Vector3 brotherPosition = new Vector3(transform.position.x + _spriteWidth, transform.position.y, transform.position.z);
            if (PossibleTiles == null)
            {
                Log.Error(MessageGroup.Gameplay, "Tileable object with name : " + transform.name + " tried spawn brother but there is no possible tiles to choose from.");
                return;
            }
            GameObject brother = Instantiate(_prefabToSpawn).gameObject;
            if (brother == null)
            {
                Log.Error(MessageGroup.Gameplay, "Tileable object with name : " + transform.name + " tried spawn brother but it is null");
                return;
            }
            brother.GetComponent<SpriteRenderer>().sprite = PossibleTiles[Random.Range(0, PossibleTiles.Count - 1)];
            RightBrother = brother.GetComponent<TileableObject>();
            RightBrother.LeftBrother = this;
            RightBrother.SetCamera(_mainCamera);
            RightBrother.RightBrother = null;
            brother.transform.parent = transform.parent;
            brother.transform.position = brotherPosition;
            brother.name = "tile";

        }

        public void SetLeftBrother()
        {
            if (HasLeftBrother)
            {
                return;
            }
            Vector3 brotherPosition = new Vector3(transform.position.x - _spriteWidth, transform.position.y,transform.position.z);
            if (PossibleTiles == null)
            {
                Log.Error(MessageGroup.Gameplay, "Tileable object with name : " + transform.name + " tried spawn brother but there is no possible tiles to choose from.");
                return;
            }
            var brother = Instantiate(_prefabToSpawn).gameObject;
            if (brother == null)
            {
                Log.Error(MessageGroup.Gameplay, "Tileable object with name : " + transform.name + " tried spawn brother but it is null");
                return;
            }
            brother.GetComponent<SpriteRenderer>().sprite = PossibleTiles[Random.Range(0, PossibleTiles.Count - 1)];
            LeftBrother = brother.GetComponent<TileableObject>();
            LeftBrother.SetCamera(_mainCamera);
            LeftBrother.RightBrother = this;
            LeftBrother.LeftBrother = null;
            brother.transform.parent = transform.parent;
            brother.transform.position = brotherPosition;
            brother.name = "tile";
        }

        public void SetCamera(Camera mainCamera)
        {
            if (_mainCamera == null)
            {
                _mainCamera = mainCamera;
            }
        }
        //Use for calculations
        void Update()
        {

            if (_deleteWhenFarAway && IsTooFar() && ( HasLeftBrother || HasRightBrother))
            {
                DestroyMe();
            }

            if (HasLeftBrother && HasRightBrother)
            {
                return;
            }

            _leftEdge = transform.position.x - _spriteWidth / 2;
            _rightEdge = transform.position.x + _spriteWidth / 2;
            _rightEdge -= WidthOfHalfScreen();
            _leftEdge += WidthOfHalfScreen();
            
            if (_mainCamera.transform.position.x >= _rightEdge - _edgeOffset && HasRightBrother == false)
            {
                SetRightBrother();
            }
            if (_mainCamera.transform.position.x <= _leftEdge + _edgeOffset && HasLeftBrother == false)
            {
                SetLeftBrother();
            }

        }

        private float WidthOfHalfScreen()
        {
            return _mainCamera.orthographicSize*Screen.width/Screen.height;
        }

        private bool IsTooFar()
        {
            float distance = Vector2.Distance(_mainCamera.transform.position, gameObject.transform.position);
            return (distance >= _maximumDistance);
        }

        private void DestroyMe()
        {
            Destroy(gameObject);
        }
    }
}
