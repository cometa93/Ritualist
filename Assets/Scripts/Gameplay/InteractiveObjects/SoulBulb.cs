using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class SoulBulb : CatchAbleObject
    {


        [SerializeField] private float _timeToCatch = 2f;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _catchedParticlesTransform;

        protected override void Awake()
        {
            TimeToCatch = _timeToCatch;
            base.Awake();
        }

        protected override void OnCatched()
        {
            _animator.SetBool(IsCatchedAnimatorBool, true);
            base.OnCatched();
        }

        protected override void OnRelease()
        {
            _animator.SetBool(IsCatchedAnimatorBool, false);
            base.OnRelease();
        }

        public void EmitCatchedParticles()
        {
            var go = Instantiate(_catchedParticlesTransform, transform.position, Quaternion.identity) as GameObject;
            go.transform.parent = transform.parent;
        }


        protected override void OnFinished()
        {
            _animator.SetBool(IsFinished, true);
            Destroy(gameObject, 3f);
            base.OnFinished();
        }
    }
}