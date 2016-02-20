using UnityEngine;
using System.Collections;
using DevilMind;

namespace Ritualist
{
    public class CatchPoint : MonoBehaviour
    {
        [SerializeField] private Transform _frontCheck;
        [SerializeField] private LayerMask _collideAbleObjectsMask;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private GameObject _particlesOnCollision;

        private bool _alreadyDocked;

        private void Awake()
        {
            _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (_alreadyDocked)
            {
                return;
            }

            if (collision2D.collider.gameObject.layer != _collideAbleObjectsMask)
            {
                Log.Error(MessageGroup.Gameplay, "Catchable object should not collide with other layers than world !");
                return;
            }

            _rigidbody2D.isKinematic = true;
            _alreadyDocked = true;
            Instantiate(_particlesOnCollision, transform.position, Quaternion.identity);
        }
       
        public Vector2 GetDockPosition()
        {
            return _frontCheck.transform.position;
        }

        public void Shoot()
        {
            _rigidbody2D.AddRelativeForce(new Vector2(0, 140), ForceMode2D.Force);
        }
    }
}