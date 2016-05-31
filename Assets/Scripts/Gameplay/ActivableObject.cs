using DevilMind;
using UnityEngine;

namespace Fading.InteractiveObjects
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ActivableObject : DevilBehaviour
    {
        public bool IsActivated = false;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                {
                    return;
                }
                _isEnabled = value;
                if (_isEnabled)
                {
                    OnEnabled();
                }
                else
                {
                    OnDisabled();
                }
            }
        }

        private bool _isEnabled;
        private CircleCollider2D _collider;
        private Rigidbody2D _rigidbody;
        private Transform _myTransform;

        [Header("Activable Object Setup")]
        [SerializeField] protected float ColliderRange = 1f;
        [SerializeField] protected GameObject InformationObject;
        [SerializeField] protected LayerMask InteractiveMask;
        
        protected override void Awake()
        {
            _myTransform = transform;
            _collider = GetComponent<CircleCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            SetupCollider();
            base.Awake();
        }

        private void SetupCollider()
        {
            if (_collider == null)
            {
                Log.Error(MessageGroup.Gameplay, "Cant't get circle collider from activable object of type : " + GetType());
                return;
            }
            _collider.radius = ColliderRange;
        }

        protected override void Update()
        {
            if (IsEnabled == false)
            {
                return;
            }

            bool isInteracting = Physics2D.OverlapCircle(_myTransform.position, _collider.radius, InteractiveMask);
            if (isInteracting)
            {
                if (IsActivated == true)
                {
                    return;
                }
                OnActivation();
                InformationObject.SetActive(true);
                return;
            }
          
            if (IsActivated == false)
            {
                return;
            }
            OnDeactivation();
            InformationObject.SetActive(false);
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }

        protected virtual void OnActivation()
        {
        }

        protected virtual void OnDeactivation()
        {
        }
    }
}