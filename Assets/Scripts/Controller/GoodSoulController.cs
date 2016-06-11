using System.Collections;
using DevilMind;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.Controller
{
    public class GoodSoulController : DevilBehaviour
    {
        private const string AnimatorIsControlledBoolName = "IsControlled";
        private const float MaximalDistance = 100f;


        [SerializeField] private float MaxSpeed = 4f;
        [SerializeField] private Rigidbody2D _myRigidBody;
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider2D _collider2D;
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
                _collider2D.enabled = _isControlled;

                if (_isControlled == false)
                {
                    BackToStartingPostion();
                }
                else
                {
                    iTween.Stop(gameObject);
                    _animator.enabled = false;
                    _animator.SetBool(AnimatorIsControlledBoolName, true);
                }
            }
            base.OnEvent(gameEvent);
        }

        private void BackToStartingPostion()
        {
            iTween.Stop(gameObject);
            Hashtable parameters =
            new Hashtable
            {
                {iT.MoveTo.islocal, true },
                {iT.MoveTo.oncomplete,"OnMoveBackComplete"},
                {iT.MoveTo.position, Vector3.zero},
                {iT.MoveTo.time, Vector2.Distance(transform.localPosition, Vector2.zero) / MaxSpeed },
                {iT.MoveTo.easetype, EaseType.easeInOutBounce },
            };
            iTween.MoveTo(gameObject, parameters);
        }

        private void OnMoveBackComplete()
        {
            _animator.enabled = true;
            _animator.SetBool(AnimatorIsControlledBoolName, true);
        }

        protected override void Update()
        {
            base.Update();
            if (_isControlled == false || GameplayController.IsGameplayPaused)
            {
                return;
            }
            Move();
        }

        private void Move()
        {
            if (Vector2.Distance(transform.localPosition, Vector2.zero) > MaximalDistance)
            {
                GameMaster.Events.Rise(EventType.CharacterChanged);
                return;
            }
            var xVelocityValue = MyInputManager.GetAxis(InputAxis.HorizontalMovement)*MaxSpeed;
            var yVelocityValue = MyInputManager.GetAxis(InputAxis.VerticalMovement)*MaxSpeed;
            _myRigidBody.velocity = new Vector2(xVelocityValue, yVelocityValue);
        }
    }
}