using DevilMind;
using UnityEngine;

namespace Ritualist
{
    public class CharacterController : MonoBehaviour
    {
        private struct SlopeRayResult
        {
            public RaycastHit2D CenterBottomHit;

            public float SlopeAngle
            {
                get { return Mathf.Abs(DevilMath.GetAngleBeetwenPoints(CenterBottomHit.normal.x, CenterBottomHit.normal.y)); }
            }
        }

        private const string CharacterHorizontalSpeed = "HorizontalSpeed";
        private const string CharacterVerticalSpeed = "VerticalSpeed";
        private const string CharacterIsGrounded = "IsGrounded";
        private const float GroundCheckRadius = .1f;
        private const float MaximalWalkingSpeed = 0.5f;
        private const float MinimalWalkingSpeed = 0.01f;

        [Range(1f, 4f)] [SerializeField] private float _runningAnimationSpeed;
        [Range(1f, 4f)] [SerializeField] private float _walkingAnimationSpeed;
        [Range(1, 15f)] [SerializeField] private float _doubleJumpPower;
        [Range(0.1f, 1f)] [SerializeField] private float _airControllPowerModyfier;
        [Range(0f, 10f)] [SerializeField] private float _minimumAirControllSpeed;

        [SerializeField] private float _maxSpeed = 10f;
        [SerializeField] private float _jumpForce = 400f;
        [SerializeField] private bool _airControl = false;
        [SerializeField] private LayerMask _whatIsGround;     
        [SerializeField] private Rigidbody2D _myRigidBody2D;
        [SerializeField] private Transform _backCheck;
        [SerializeField] private Transform _frontCheck;
        [SerializeField] private Transform[] _groundCheck;
        [SerializeField] private Animator _characterAnimator;
        [SerializeField] private BoxCollider2D _boxCollider;

        private bool _doubleJump;
        private bool _handIsVisible;
        private bool _isGrounded;
        private float _lastFloatMoveValue;
        private SlopeRayResult _rayResult = new SlopeRayResult();

        private bool IsFacingRight
        {
            get { return transform.localScale.z > 0; }
        }

        private void Update()
        {
            SetupClipSpeedBasedOnVelocity();
            SetRaysResult();
        }

        private void FixedUpdate()
        {
            _isGrounded = false;
            for (int i = 0, c = _groundCheck.Length; i < c; ++i)
            {
                if (Physics2D.OverlapCircle(_groundCheck[i].position, GroundCheckRadius + 1, _whatIsGround))
                {
                    _isGrounded = true;
                    _doubleJump = false;
                }
            }

            _characterAnimator.SetBool(CharacterIsGrounded, _isGrounded);
            _characterAnimator.SetFloat(CharacterVerticalSpeed, _myRigidBody2D.velocity.y);
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
        
        private void Flip()
        {
            Vector3 theScale = transform.localScale;
            theScale.z *= -1;
            transform.localScale = theScale;
            Vector3 theRotation = transform.localEulerAngles;
            theRotation.y = theScale.z > 0 ? 0 : 180;
            transform.localEulerAngles = theRotation;
        }

        private void SetRaysResult()
        {
            var centerBottomPosition = new Vector2(_groundCheck[2].position.x, _groundCheck[2].position.y + 0.1f);
            var rayCenterResult = Physics2D.Raycast(centerBottomPosition, Vector2.down, 1f, _whatIsGround);
            _rayResult.CenterBottomHit = rayCenterResult;
            Debug.DrawRay(new Vector2(_groundCheck[2].position.x, _groundCheck[2].position.y + 0.1f),
                                    Vector2.down,Color.blue,0.1f);
            Debug.Log((_rayResult.SlopeAngle - 90) * ( IsFacingRight? 1:-1) );
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
                _myRigidBody2D.velocity = new Vector2( move * _maxSpeed, _myRigidBody2D.velocity.y);
            }

            // If the player should jump...
            if (_isGrounded && jump)
            {
                // Add a vertical force to the player.
                _isGrounded = false;
                _characterAnimator.SetBool(CharacterIsGrounded, false);
                jump = false;
                _myRigidBody2D.AddForce(new Vector2(0f, _jumpForce));
            }

            if ( jump && _isGrounded == false && _characterAnimator.GetBool(CharacterIsGrounded) == false && _doubleJump == false)
            {
                _myRigidBody2D.velocity = new Vector2(0, _doubleJumpPower);
                _doubleJump = true;
            }
        }
    }
}
