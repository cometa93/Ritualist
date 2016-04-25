using System.Runtime.InteropServices;
using DevilMind;
using Ritualist;
using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public enum SkillTargetType
    {
        None,
        CatchPoint,
        Enemy
    }

    public class SkillTarget : DevilBehaviour
    {
        [SerializeField] private SkillTargetType _targetType;

        public SkillTargetType Type { get { return _targetType; } }

        protected virtual void OnTriggerEnter2D(Collider2D collider2D)
        {
        }

        protected virtual void OnTriggerExit2D(Collider2D collider2D)
        {
        }

        private Vector2 _myPosition;

        public Vector2 Position
        {
            get
            {
                _myPosition.x = _myTransform.position.x;
                _myPosition.y = _myTransform.position.y;
                return _myPosition;
            }
        }

        private Transform _myTransform;
        
        public bool IsEnabled { get; set; }

        private void Unregister()
        {
            GameplayController.Instance.UnregisterTarget(this);
        }

        protected override void Awake()
        {
            _myTransform = transform;
            _myPosition = new Vector2(_myTransform.position.x, _myTransform.position.y);
            base.Awake();
        }

        protected override void OnDisable()
        {
            Unregister();
            base.OnDisable();
        }

        public float Distance(Vector2 to)
        {
            return Vector2.Distance(Position, to);
        }
    }
}