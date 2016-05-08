using DevilMind;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.Controller
{
    public class GoodSoulController : DevilBehaviour
    {
        private const string AnimatorIsControlledBoolName = "IsControlled";

        [SerializeField] private float MaxSpeed = 1f;
        [SerializeField] private Rigidbody2D _myRigidBody;
        [SerializeField] private Animator _animator;
        private bool _isControlled = false;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.CharacterChanged);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.CharacterChanged)
            {
                _isControlled = !_isControlled;
                _animator.SetBool(AnimatorIsControlledBoolName, _isControlled);
            }
            base.OnEvent(gameEvent);
        }

        protected override void Update()
        {
            base.Update();
            if (_isControlled == false)
            {
                return;
            }
            Move();
        }

        private void Move()
        {
            var xVelocityValue = MyInputManager.GetAxis(InputAxis.LeftStickX)*MaxSpeed;
            var yVelocityValue = MyInputManager.GetAxis(InputAxis.LeftStickY)*MaxSpeed;
            _myRigidBody.velocity = new Vector2(xVelocityValue, yVelocityValue);
        }
    }
}