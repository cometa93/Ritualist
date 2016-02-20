using UnityEngine;
using System.Collections;
using DevilMind;
using DevilMind.Utils;

namespace Ritualist
{
    public class CatchPoint : MonoBehaviour
    {
        [SerializeField] private Transform _frontCheck;
        [SerializeField] private LayerMask _collideAbleObjectsMask;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private GameObject _particlesOnCollision;
        [SerializeField] private GameObject _destroyParticles;
        [SerializeField] private GameObject _dockedParticles;

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

            _rigidbody2D.isKinematic = true;
            _alreadyDocked = true;
            _dockedParticles.gameObject.SetActive(true);
            GameplayController.Instance.PlacePoint(this);
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

        public void DestroyMe()
        {
            Instantiate(_destroyParticles, transform.position, Quaternion.identity);
            StartCoroutine(TimeHelper.RunAfterSeconds(0.5f, () =>
            {
                Destroy(gameObject);
            }));
        }

    }
}