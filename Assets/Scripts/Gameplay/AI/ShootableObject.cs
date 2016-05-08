using System.Collections;
using DevilMind;
using UnityEngine;

namespace Fading.AI
{
    public class ShootableObject : MonoBehaviour
    {
        private const string EnemyMissleLayerName = "EnemyMissle";
        private bool _lock;
        protected Vector3 Target { get; set; }
        private Transform _myTransform;
        [SerializeField] private CircleCollider2D _myCollider2D;
        [SerializeField] private LayerMask _playerLayerMask;

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer(EnemyMissleLayerName);
            _myTransform = transform;
        }

        private void LateUpdate()
        {
            if (_lock)
            {
                return;
            }

            var result = Physics2D.OverlapCircle(_myTransform.position, _myCollider2D.radius, _playerLayerMask);
            if (result != null)
            {
                _lock = true;
                iTween.Stop(gameObject);
                OnShootHitThePlayer();
            }
        }

        public void OnCollisionEnter2D(Collision2D coll)
        {
            if (_lock)
            {
                return;
            }

            iTween.Stop(gameObject);
            OnShootMissed();
        }

        protected virtual void OnShootMissed()
        {
            _lock = true;
        }

        protected virtual void OnShootHitThePlayer()
        {
            _lock = true;
        }

        protected virtual void OnTargetReached()
        {
            _lock = true;
        }

        public void Shoot(Vector3 target)
        {
            Target = target;
            var hashset = ShootItweenAnimation();
            hashset.Add(iT.MoveTo.position, target);
            hashset.Add(iT.MoveTo.oncomplete, "OnTargetReached");
            iTween.MoveTo(gameObject, hashset);
        }

        protected virtual Hashtable ShootItweenAnimation()
        {
            Log.Warning(MessageGroup.Gameplay, "Enemy shoot able object doesn't " +
                                               "implement Correct ShootItweenAnimation function");
            return null;
        }

    }
}