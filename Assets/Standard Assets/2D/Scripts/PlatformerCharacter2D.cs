using System;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityStandardAssets.Cameras;

namespace Ritualist
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        private const string CharacterHorizontalSpeed = "HorizontalSpeed";
        private const string CharacterVerticalSpeed = "VerticalSpeed";
        private const string CharacterIsGrounded = "IsGrounded";

        private const float MaximalWalkingSpeed = 0.5f;
        private const float MinimalWalkingSpeed = 0.01f;

        [Range(1f, 4f)] [SerializeField] private float _runningAnimationSpeed;
        [Range(1f, 4f)] [SerializeField] private float _walkingAnimationSpeed;
        [Range(1, 15f)] [SerializeField] private float _doubleJumpPower;
        [Range(0.1f, 1f)] [SerializeField] private float _airControllPowerModyfier;
        [Range(0f, 10f)] [SerializeField] private float _minimumAirControllSpeed;

        [SerializeField] private float _maxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float _jumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool _airControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask _whatIsGround;      // A mask determining what is ground to the character
        [SerializeField] private Rigidbody2D _myRigidBody2D;
        [SerializeField] private Transform _backCheck;
        [SerializeField] private Transform _frontCheck;
        [SerializeField] private Transform[] _groundCheck;
        [SerializeField] private Animator _characterAnimator;

        public bool DoubleJump;
        private bool _handIsVisible;
        private bool _isGrounded;
        const float _groundedCheckRadius = .1f;
        private float _lastFloatMoveValue;

        private bool IsFacingRight
        {
            get { return transform.localScale.z > 0; }
        }

        private void Update()
        {
            SetupClipSpeedBasedOnVelocity();
        }

        private void SetupClipSpeedBasedOnVelocity()
        {
            float velocity = _characterAnimator.GetFloat(CharacterHorizontalSpeed);
            bool isRunning = velocity > MaximalWalkingSpeed && _isGrounded;
            bool isWalking = isRunning == false && velocity >= MinimalWalkingSpeed && _isGrounded;

            if (isRunning)
            {
                float currentMaxSpeedPercent = (velocity - MaximalWalkingSpeed)/(1f - MaximalWalkingSpeed);
                float currentClipSpeed = Mathf.Lerp(1f, _runningAnimationSpeed, currentMaxSpeedPercent);

                _characterAnimator.speed = currentClipSpeed;
                return;
            }

            if (isWalking)
            {
                float currentMaxSpeedPercent = velocity/MaximalWalkingSpeed;
                float currentClipSpeed = Mathf.Lerp(1f, _walkingAnimationSpeed, currentMaxSpeedPercent);

                _characterAnimator.speed = currentClipSpeed;
                return;
            }

            _characterAnimator.speed = 1;
        }

        private void FixedUpdate()
        {
            _isGrounded = false;
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.

            for (int i = 0, c = _groundCheck.Length; i < c; ++i)
            {
                if (Physics2D.OverlapCircle(_groundCheck[i].position, _groundedCheckRadius+1, _whatIsGround))
                {
                    _isGrounded = true;
                    DoubleJump = false;
                }
            }
            
           
            _characterAnimator.SetBool(CharacterIsGrounded, _isGrounded);
            // Set the vertical animation
            _characterAnimator.SetFloat(CharacterVerticalSpeed, _myRigidBody2D.velocity.y);
        }

        public Vector2 CharacterFront()
        {
            return IsFacingRight == false ? new Vector2(-1, 0) : new Vector2(1, 0);
        }

        public void Move(float move, bool crouch, bool jump)
        {

            if (move > 0 && IsFacingRight == false)
            {
                Flip();
            }
            else if (move < 0 && IsFacingRight)
            {
                Flip();
            }

           //only control the player if grounded or airControl is turned on
            if (_isGrounded || _airControl )
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                _characterAnimator.SetFloat(CharacterHorizontalSpeed, Mathf.Abs(move));

                // Move the character
                _lastFloatMoveValue = move;
                var xSpeedVelocity = _lastFloatMoveValue * _maxSpeed;
                _myRigidBody2D.velocity = new Vector2( xSpeedVelocity , _myRigidBody2D.velocity.y);
            }

            // If the player should jump...
            if (_isGrounded && jump && _characterAnimator.GetBool(CharacterIsGrounded))
            {
                // Add a vertical force to the player.
                _isGrounded = false;
                _characterAnimator.SetBool(CharacterIsGrounded, false);
                jump = false;
                _myRigidBody2D.AddForce(new Vector2(0f, _jumpForce));
            }

            if ( jump && _isGrounded == false && _characterAnimator.GetBool(CharacterIsGrounded) == false && DoubleJump == false)
            {
                _myRigidBody2D.velocity = new Vector2(0, _doubleJumpPower);
                DoubleJump = true;
            }
        }

        private void Flip()
        {
            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.z *= -1;
            transform.localScale = theScale;
            Vector3 theRotation = transform.localEulerAngles;
            theRotation.y = theScale.z > 0 ? 0 : 180;
            transform.localEulerAngles = theRotation;
        }
    }
}
