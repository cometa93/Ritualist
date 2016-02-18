using UnityEngine;
using System.Collections;

namespace Ritualist
{
    public class RuneBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform _frontCheck;
        [SerializeField]
        private LayerMask _maskForFrontCheck;
        public Rigidbody2D _rigidbody2D;

        private bool _alreadyDocked;

        private void FixedUpdate()
        {
            if (_alreadyDocked)
            {
                return;
            }

            if (Physics2D.OverlapCircle(GetDockPosition(), 0.2f, _maskForFrontCheck))
            {
                GameplayController.Instance.PlaceRune(this);
                _rigidbody2D.isKinematic = true;
                _alreadyDocked = true;
            }
        }

        public Vector2 GetDockPosition()
        {
            return _frontCheck.transform.position;
        }

        public void AnimateDestroy()
        {
            StartCoroutine(DestroyCoroutineCounter());
        }

        private IEnumerator DestroyCoroutineCounter()
        {
            yield return new WaitForSeconds(1f);
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }

}