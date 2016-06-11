using DevilMind;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fading
{
    public class MyCharacterController : MonoBehaviour
    {
        private struct SlopeRayResult
        {
            public RaycastHit2D CenterBottomHit;
            public RaycastHit2D FrontBottomHit;

        
            public float RightHitAngle
            {
                get
                {
                    return FrontBottomHit.collider != null
                        ? DevilMath.GetAngleBeetwenPoints(FrontBottomHit.normal.x, FrontBottomHit.normal.y) - 90
                        : 0;
                }
            }

            public float SlopeAngle
            {
                get
                {
                    return CenterBottomHit.collider != null ?
                        DevilMath.GetAngleBeetwenPoints(CenterBottomHit.normal.x, CenterBottomHit.normal.y) - 90:
                        0 ;
                }
            }

            public override string ToString()
            {
                return "RightHitAngle : " + RightHitAngle + "\n" +
                       "SlopeHitAngle : " + SlopeAngle;
            }
        }

        private const string CharacterHorizontalSpeed = "HorizontalSpeed";
        private const string CharacterVerticalSpeed = "VerticalSpeed";
        private const string CharacterIsGrounded = "IsGrounded";

        private const int CharacterAirLayerIndex = 1;
        private const int CharacterGroundLayerIndex = 0;

        private const float MaximalWalkingSpeed = 3.1f;
        private const float MinimalWalkingSpeed = 0.05f;

        [Range(0, 90)] [SerializeField] private int _slopeAngleOffset = 10;
        [Range(0.5f, 2.5f)] [SerializeField] private float _runningAnimationSpeed;
        [Range(0.5f, 2.5f)] [SerializeField] private float _walkingAnimationSpeed;
        [Range(1, 15f)] [SerializeField] private float _doubleJumpPower;
        [Range(0.1f, 1f)] [SerializeField] private float _airControllPowerModyfier;
        [Range(0f, 10f)] [SerializeField] private float _minimumAirControllSpeed;

        [SerializeField] private float _maxSpeed = 10f;
        [SerializeField] private float _jumpForce = 25f;
        [SerializeField] private bool _airControl = false;

        [SerializeField] private Transform _centerRayPosition;
        [SerializeField] private Transform _rightRayPosition;
        [SerializeField] private Transform _groundCheck;

        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private LayerMask _platformsLayer;
        [SerializeField] private Animator _characterAnimator;

        [FormerlySerializedAs("_myRigidBody2D")]
        public Rigidbody2D CharacterRigidbody;

        private float _currentAirAnimationLayerWeight;
        private float _currentGroundAnimationLayerWeight = 1f;

        private bool _isGrounded;
        private bool _isOnPlatform;
        private bool _jumped;

        private SlopeRayResult _rayResult;

        private bool IsFacingRight
        {
            get { return transform.localScale.z > 0; }
        }
        
        private void Update()
        {
            ResetCharacterState();
            SetRaysResult();
            SetCharacterState();
           
            SetupClipSpeedBasedOnVelocity();
            SetAnimationLayersWeight();

            _characterAnimator.SetFloat(CharacterVerticalSpeed, CharacterRigidbody.velocity.y);
            _characterAnimator.SetBool(CharacterIsGrounded, _isGrounded);
            _characterAnimator.SetFloat(CharacterHorizontalSpeed, Mathf.Abs(CharacterRigidbody.velocity.x));

            UpdateConsoleText();
        }

        private void ResetCharacterState()
        {
            _isGrounded = false;
            _isOnPlatform = false;
            _jumped = false;
        }

        private void SetCharacterState()
        {
            var collider = Physics2D.OverlapCircle(_groundCheck.position, 1.5f, _platformsLayer);
            if (collider != null)
            {
                _isGrounded = true;
                _isOnPlatform = true;
                return;
            }

            collider = Physics2D.OverlapCircle(_groundCheck.position, 1.5f, _whatIsGround);
            if (collider != null)
            {
                _isGrounded = true;
                return;
            }
        }

        private void UpdateConsoleText()
        {
            if (ConsoleText.Instance != null)
            {
                ConsoleText.Instance.UpdateText(
                    _rayResult + "\n" +
                    ToString() + "\n" +
                    "CURRENT AIR WEIGHT:" + _currentAirAnimationLayerWeight
                );
            }
        }
        
        private void SetupClipSpeedBasedOnVelocity()
        {
            float velocity = Mathf.Abs(CharacterRigidbody.velocity.x);
            if (velocity <= 0.05f && _characterAnimator.IsInTransition(0))
            {
                _characterAnimator.speed = 2.5f;
                return;
            }
            if (velocity <= 0.05f)
            {
                _characterAnimator.speed = 1;
                return;
            }
            
            bool isRunning = velocity > MaximalWalkingSpeed && _isGrounded;
            bool isWalking = isRunning == false && velocity >= MinimalWalkingSpeed && _isGrounded;
            
            if (isRunning)
            {
                float currentMaxSpeedPercent = (velocity - MaximalWalkingSpeed)/(_maxSpeed - MaximalWalkingSpeed);
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
            var rayCenterResult = Physics2D.Raycast(_centerRayPosition.position, Vector2.down, 2f, _whatIsGround);
            _rayResult.CenterBottomHit = rayCenterResult;

            var rightRayResult = Physics2D.Raycast(_rightRayPosition.position, new Vector3(IsFacingRight ? 1 : -1, -0.75f,0), 3f, _whatIsGround);
            _rayResult.FrontBottomHit = rightRayResult;
            
            Debug.DrawRay(new Vector2(_centerRayPosition.position.x, _centerRayPosition.position.y), Vector2.down*2,Color.red,0.1f);
            Debug.DrawRay(new Vector2(_rightRayPosition.position.x, _rightRayPosition.position.y), new Vector3(IsFacingRight ? 1 : -1, -0.75f, 0), Color.red, 0.1f);
            Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + CharacterRigidbody.velocity, Color.cyan);
        }

        public void Move(float move, bool jump)
        {

            if (move > 0 && IsFacingRight == false)
            {
                Flip();
            }
            else if (move < 0 && IsFacingRight)
            {
                Flip();
            }

            if (_isGrounded && jump)
            {
                _isGrounded = false;
                var velo = CharacterRigidbody.velocity;
                velo.y = 0;
                CharacterRigidbody.velocity = velo;
                CharacterRigidbody.AddRelativeForce(Vector2.up *_jumpForce, ForceMode2D.Impulse);
                return;
            }

            if (_isGrounded && jump == false)
            {
                if (Mathf.Abs(move) < 0.02f)
                {
                    CharacterRigidbody.velocity = new Vector2(0, CharacterRigidbody.velocity.y);
                }
                else
                {
                    SetGroundedVelocity(move);
                }
            }

            if (_isGrounded == false && _airControl)
            {
                var velo = CharacterRigidbody.velocity;
                velo.x = move*_maxSpeed;
                CharacterRigidbody.velocity = velo;
            }
        }

        private void SetGroundedVelocity(float move)
        {
            Vector2 moveVector = new Vector2(move*_maxSpeed, 0);


            int angle = (int) (IsFacingRight ? _rayResult.SlopeAngle : -_rayResult.SlopeAngle);
            if (angle < 10)
            {
                Vector2 velocity = CharacterRigidbody.velocity;
                velocity.x = moveVector.x;
                CharacterRigidbody.velocity = velocity;
                return;
            }

            int frontAngle = (int) (IsFacingRight ? _rayResult.RightHitAngle : -_rayResult.RightHitAngle);
            if (angle > 0 &&
                angle > frontAngle &&
                angle - frontAngle > _slopeAngleOffset)
            {
                moveVector += Physics2D.gravity*Time.deltaTime*CharacterRigidbody.mass;
                CharacterRigidbody.velocity = moveVector;
                return;
            }

            if (angle >= 10)
            {
                moveVector = DevilMath.RotateVectorZByAngle(moveVector, IsFacingRight ? angle : - angle);
                Vector2 velocity = CharacterRigidbody.velocity;
                velocity.y = moveVector.y;
                velocity.x = moveVector.x;
                CharacterRigidbody.velocity = velocity;
            }
        }

        private void SetAnimationLayersWeight()
        {
            _currentAirAnimationLayerWeight = Mathf.Lerp(_currentAirAnimationLayerWeight, _isGrounded ? 0f : 1f, 5 * Time.deltaTime);

            if (_currentAirAnimationLayerWeight > 0.98f)
            {
                _currentAirAnimationLayerWeight = 1f;
            }
            else if (_currentAirAnimationLayerWeight < 0.02f)
            {
                _currentAirAnimationLayerWeight = 0f;
            }
            _currentGroundAnimationLayerWeight = 1f - _currentAirAnimationLayerWeight;

            _characterAnimator.SetLayerWeight(CharacterAirLayerIndex, _currentAirAnimationLayerWeight);
            _characterAnimator.SetLayerWeight(CharacterGroundLayerIndex, _currentGroundAnimationLayerWeight);
        }

        public override string ToString()
        {
            return
                "Is grounded: " + _isGrounded + "\n" +
                "Is on Platform: " + _isOnPlatform + "\n" +
                "Jumped " + _jumped;
        }
    }
}
