using DevilMind;
using UnityEngine;

namespace Fading
{
    public class GoodSoulParentController : DevilBehaviour
    {
        [SerializeField] private Transform _transformToFollow;
        [SerializeField] private Transform _myTransform;
        protected override void Awake()
        {
            _myTransform = transform;
            base.Awake();
        }

        protected override void Update()
        {
            if (Vector2.Distance(_myTransform.position, _transformToFollow.position) < 0.05f)
            {
                return;
            }
            _myTransform.position = Vector2.Lerp(_myTransform.position, _transformToFollow.position, Time.deltaTime*4);
            base.Update();
        }
    }
}