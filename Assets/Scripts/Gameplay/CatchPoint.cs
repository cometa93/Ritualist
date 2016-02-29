using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;

namespace Ritualist
{
    public class CatchPoint : MonoBehaviour
    {
        [SerializeField] private float _timeToTarget = 1f;
        
        [SerializeField] private LayerMask _collideAbleObjectsMask;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private GameObject _particlesOnCollision;
        [SerializeField] private GameObject _destroyParticles;

        private bool _alreadyDocked;
        private Vector2 _target;

        private void Awake()
        {
            _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            iTween.Stop(gameObject, true);
            if (_alreadyDocked)
            {
                return;
            }
            
            _alreadyDocked = true;
            Instantiate(_particlesOnCollision, transform.position, Quaternion.identity);
            StartCoroutine(TimeHelper.RunAfterSeconds(1f, DestroyMe));
        }
       
        public void Shoot(Vector2 target)
        {
            iTween.MoveTo(gameObject, new Hashtable
            {
                { iT.MoveTo.orienttopath,true},
                { iT.MoveTo.path, CreateRandomPath(new Vector3(target.x, target.y,0)) },
                { iT.MoveTo.easetype, EaseType.easeInOutCirc },
                { iT.MoveTo.time, _timeToTarget}
            });
        }

        private Transform[] CreateRandomPath(Vector3 target)
        {
                var direction = target - transform.position;
                var normal = direction.normalized;
                var cross = Vector3.Cross(Vector3.up, normal);
                var variance = direction.magnitude * 0.25f;

                var amount = 0.0f;
                var path = new List<Vector3>{ transform.position};

                while (amount < 1.0)
                {
                    amount = Mathf.Clamp01(amount + Random.Range(0.1f, 0.2f));
                    var point = Vector3.Lerp(transform.position, target, amount);
                     point += cross * Random.Range(-variance, variance);
                    if (amount >= 1.0) point = target;
                    Debug.DrawLine(point, point + Vector3.up, Color.Lerp(Color.green, Color.red, amount), 5);
                    path.Add(point);
                }

            var transforms = new List<Transform>();
            foreach (var x in path)
            {
                var go = new GameObject("pathPoint");
                Destroy(go, 10f);
                go.transform.position = x;
                transforms.Add(go.transform);
            }
            return transforms.ToArray();
        }

        public void DestroyMe()
        {
            Instantiate(_destroyParticles, transform.position, Quaternion.identity);
            iTween.Stop(gameObject, true);
            Destroy(gameObject, 1f);
        }
    }
}