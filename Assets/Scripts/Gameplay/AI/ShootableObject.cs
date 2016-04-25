using System.Collections;
using DevilMind;
using UnityEngine;

namespace Ritualist.AI
{
    public class ShootableObject : MonoBehaviour
    {
        private bool _lock;
        protected Vector3 Target { get; set; }

        public void OnCollisionEnter2D(Collision2D coll)
        {
            if (_lock)
            {
                return;
            }

            iTween.Stop(gameObject);
            if (coll.gameObject.CompareTag("Player") == false)
            {
               OnShootMissed();
                return;
            }

            OnShootHitThePlayer();
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