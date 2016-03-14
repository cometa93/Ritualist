using DevilMind;
using UnityEngine;

namespace Ritualist
{
    public class CharacterController : MonoBehaviour
    {
        private struct SlopeRayResult
        {
            public RaycastHit2D CenterBottomHit;
            public RaycastHit2D BackBottomHit;
            public RaycastHit2D FrontBottomHit;

            public float LeftHitAngle
            {
                get
                {
                    return BackBottomHit.collider != null
                        ? DevilMath.GetAngleBeetwenPoints(BackBottomHit.normal.x, BackBottomHit.normal.y ) - 90
                        : 0;
                }
            }

            public float RightHitAngle
            {
                get
                {
                    return FrontBottomHit.collider != null
                        ? DevilMath.GetAngleBeetwenPoints(FrontBottomHit.normal.x, FrontBottomHit.normal.y) - 90
                        : 0;
                }
            }

            public bool IsGrounded
            {
                get { return CenterBottomHit.collider != null && CenterBottomHit.distance < 0.5f; }
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
                return
                    "LeftHitAngle : " + LeftHitAngle + "\n" +
                    "RightHitAngle : " + RightHitAngle + "\n" +
                    "SlopeHitAngle : " + SlopeAngle;
            }
        }

        private const string CharacterHorizontalSpeed = "HorizontalSpeed";
        private const string CharacterVerticalSpeed = "VerticalSpeed";
        private const string CharacterIsGrounded = "IsGrounded";

        private const float MaximalWalkingSpeed = 0.5f;
        private const float MinimalWalkingSpeed = 0.01f;

        [Range(0, 90)] [SerializeField] private int _slopeAngleOffset = 10;
        [Range(1f, 4f)] [SerializeField] private float _runningAnimationSpeed;
        [Range(1f, 4f)] [SerializeField] private float _walkingAnimationSpeed;
        [Range(1, 15f)] [SerializeField] private float _doubleJumpPower;
        [Range(0.1f, 1f)] [SerializeField] private float _airControllPowerModyfier;
        [Range(0f, 10f)] [SerializeField] private float _minimumAirControllSpeed;

        [SerializeField] private float _maxSpeed = 10f;
        [SerializeField] private float _jumpForce = 400f;
        [SerializeField] private bool _airControl = false;

        [SerializeField] private Transform _centerRayPosition;
        [SerializeField] private Transform _leftRayPosition;
        [SerializeField] private Transform _rightRayPosition;

        [SerializeField] private LayerMask _whatIsGround;     
        [SerializeField] private Rigidbody2D _myRigidBody2D;
        [SerializeField] private Animator _characterAnimator;

        private bool _isGrounded;
        private SlopeRayResult _rayResult;

        private bool IsFacingRight
        {
            get { return transform.localScale.z > 0; }
        }
        
        private void Update()
        {
            SetRaysResult();
            ConsoleText.Instance.UpdateText(_rayResult.ToString());
            SetupClipSpeedBasedOnVelocity();
            _isGrounded = _rayResult.IsGrounded;
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
            var rayCenterResult = Physics2D.Raycast(_centerRayPosition.position, Vector2.down, 2f, _whatIsGround);
            _rayResult.CenterBottomHit = rayCenterResult;

            var leftRayResult = Physics2D.Raycast(_leftRayPosition.position, new Vector3(IsFacingRight ? -1 : 1, -0.5f, 0), 2f, _whatIsGround);
            _rayResult.BackBottomHit = leftRayResult;

            var rightRayResult = Physics2D.Raycast(_rightRayPosition.position, new Vector3(IsFacingRight ? 1 : -1, -0.5f,0), 2f, _whatIsGround);
            _rayResult.FrontBottomHit = rightRayResult;

            Debug.Log(IsFacingRight ? _rayResult.SlopeAngle : -_rayResult.SlopeAngle);
            Debug.DrawRay(new Vector2(_centerRayPosition.position.x, _centerRayPosition.position.y), Vector2.down*2,Color.red,0.1f);
            Debug.DrawRay(new Vector2(_leftRayPosition.position.x, _leftRayPosition.position.y), new Vector3(IsFacingRight ? -1 : 1, -0.5f, 0), Color.red, 0.1f);
            Debug.DrawRay(new Vector2(_rightRayPosition.position.x, _rightRayPosition.position.y), new Vector3(IsFacingRight ? 1 : -1, -0.5f, 0), Color.red, 0.1f);
            Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + _myRigidBody2D.velocity, Color.cyan);
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

            if (_isGrounded && jump == false)
            {
                if (Mathf.Abs(move) < 0.02f)
                {
                    _myRigidBody2D.velocity = new Vector2(0, _myRigidBody2D.velocity.y);
                }
                else
                {
                    SetGroundedVelocity(move);
                }
            }
        }


        

        private void SetGroundedVelocity(float move)
        {
            Vector2 moveVector = new Vector2(move*_maxSpeed, 0);


            int angle = (int) (IsFacingRight ? _rayResult.SlopeAngle : -_rayResult.SlopeAngle);

            //////////////////////////////////////
            var y = _myRigidBody2D.velocity.y;
            if (y > 2 && angle < 0)
            {
                Debug.Log("Dupa");
            }
            //////////////////////////////////////
            if (angle < 10)
            {
                Vector2 velocity = _myRigidBody2D.velocity;
                velocity.x = moveVector.x;
                _myRigidBody2D.velocity = velocity;
                return;
            }

            int frontAngle = (int) (IsFacingRight ? _rayResult.RightHitAngle : -_rayResult.RightHitAngle);
            if (angle > 0 &&
                angle > frontAngle &&
                angle - frontAngle > _slopeAngleOffset)
            {
                moveVector += Physics2D.gravity*Time.deltaTime*_myRigidBody2D.mass;
                _myRigidBody2D.velocity = moveVector;
                return;
            }

            if (angle >= 10)
            {
                moveVector = DevilMath.RotateVectorZByAngle(moveVector, IsFacingRight ? angle : - angle);
                Vector2 velocity = _myRigidBody2D.velocity;
                velocity.y = moveVector.y;
                velocity.x = moveVector.x;
                _myRigidBody2D.velocity = velocity;
                return;
            }



        }
    }
}
