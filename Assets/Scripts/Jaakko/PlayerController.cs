using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using Physics2D = Nomnom.RaycastVisualization.VisualPhysics2D;
#else
using Physics2D = UnityEngine.Physics2D;
#endif

namespace AG3958
{
    /// <summary>
    /// Basic control and movement script for a 2D player entity. Attached object must have a 2D Rigidbody.
    /// </summary>
    [RequireComponent (typeof(Rigidbody2D), typeof(Collider2D), typeof(PlayerCore))]
    public class PlayerController : MonoBehaviour
    {
        // Object references
        private Rigidbody2D _rb;
        private Collider2D _coll;
        private LayerMask _envLayerMask;
        private Vector2 _edgeOffset;
        private Vector2 _leftEdge;
        private Vector2 _rightEdge;
        private SpriteRenderer _playerSprite;
        private PlayerCore _playerCore;
        private Transform _transform;

        // Script-local state identifiers and helpers
        private bool _applyMoveRight = false;
        private bool _applyMoveLeft = false;
        private bool _jumpBuffer = false;
        private bool _fireBuffer = false;
        private bool _meleeBuffer = false;
        private bool _chargeActive = false;
        private bool _chargeReady = false;
        private bool _isGrounded;
        private float _jumpCooldown = 0f;
        private float _groundCheckRayDist;
        private float _groundCheckRayOffset;
        private float _physMatFriction;
        private Color _playerColor;
        private bool _coyoteActive = false;
        private bool _boosterActive = false;
        private bool _boosterGraceActive = false;

        // Properties for animation state script
        public bool ApplyMoveRight { get { return _applyMoveRight; } }
        public bool ApplyMoveLeft {  get { return _applyMoveLeft; } }
        public bool IsGrounded { get { return _isGrounded; } }
        public bool CoyoteActive { get { return _coyoteActive; } }
        public bool BoosterActive { get { return _boosterActive; } }
        public bool BoosterGraceActive { get { return _boosterGraceActive; } }

        // Editor parameters
        [Header("Input")]
        [SerializeField] private KeyCode _leftKey = KeyCode.A;
        [SerializeField] private KeyCode _rightKey = KeyCode.D;
        [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode _fireKey = KeyCode.L;
        [SerializeField] private KeyCode _meleeKey = KeyCode.K;
        [Header("Basic Movement")]
        [SerializeField] private float _movementSpeed = 3.0f;
        [SerializeField] private float _maximumSpeed = 5.0f;
        [SerializeField] private float _jumpPower = 7.5f;
        [SerializeField] private float _coyoteDuration = 0.5f;
        [SerializeField] private float _wallSlideFriction = 1.0f;
        [Header("Fire Magic")]
        [SerializeField] private float _fireCost = 10f;
        [SerializeField] private float _fireCooldown = 1.5f;
        [Header("Charge Shot")]
        [SerializeField] private float _chargeTime = 2.5f;
        [SerializeField] private float _chargeCost = 2f;
        [SerializeField] private List<GameObject> _chargePool;
        [Header("Speed Booster")]
        [SerializeField] private float _boosterImpulseSpeed; // movement force change
        [SerializeField] private float _boosterSpeed; // maximum speed change
        [SerializeField] private float _boosterActivationTime;
        [SerializeField] private float _boosterGraceTime;

        // Editor param-derived variables
        private Vector2 _horizontalForce;
        private Vector2 _jumpForce;
        private Vector2 _baseJumpForce;
        private Vector2 _wallJumpForceRight;
        private Vector2 _wallJumpForceLeft;
        private float _resetCoyoteDuration;
        private float _originalMoveSpeed;
        private float _originalMaxSpeed; // store value of normal maximum speed in awake
        private float _boosterTimer = 0.0f;
        private float _boosterGrace = 0.0f;

        private void Awake()
        {
            _transform = transform;
            _playerCore = GetComponent<PlayerCore>();
            _rb = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _envLayerMask = LayerMask.GetMask("Default");
            _groundCheckRayOffset = 0.01f; // making this very slightly positive instead of negative enables wall jumping with no additional code
            _physMatFriction = _rb.sharedMaterial.friction;
            _edgeOffset = new Vector2((_transform.localScale.x / 2) + _groundCheckRayOffset, _transform.position.y);
            _leftEdge = (Vector2)_transform.position - _edgeOffset;
            _rightEdge = (Vector2)_transform.position + _edgeOffset;
            _playerSprite = GetComponent<SpriteRenderer>();
            _playerColor = _playerSprite.color;
            _resetCoyoteDuration = _coyoteDuration;
            _groundCheckRayDist = (_transform.localScale.y / 2) + 0.05f;
            _horizontalForce = new Vector2(_movementSpeed, 0);
            _baseJumpForce = new Vector2(0, _jumpPower);
            _jumpForce = _baseJumpForce;
            _wallJumpForceRight = new Vector2(_jumpPower * 0.75f, _jumpPower);
            _wallJumpForceLeft = new Vector2(-_jumpPower * 0.75f, _jumpPower);
            _originalMoveSpeed = _movementSpeed;
            _originalMaxSpeed = _maximumSpeed;
        }

        private void Update()
        {
            _jumpCooldown += Time.deltaTime;
            if (Input.GetKey(_leftKey)) { _applyMoveLeft = true; }
            else _applyMoveLeft = false;
            if (Input.GetKey(_rightKey) ) { _applyMoveRight = true; }
            else _applyMoveRight = false;
            if (Input.GetKeyDown(_jumpKey) && _isGrounded) { _jumpBuffer = true; }
            if (Input.GetKeyDown(_meleeKey)) { _meleeBuffer = true; }
            if (Input.GetKey(_fireKey))
            {
                if (_playerCore.HasCharge)
                {
                    _chargeActive = true;
                }
                else _fireBuffer = true;
            }
            else
            {
                _chargeActive = false;
                _fireBuffer = false;
            }

            if (_playerCore.HasSpeedBooster)
            {
                if ((_rb.linearVelocityX >= _maximumSpeed * 0.9f && _isGrounded) || (_rb.linearVelocityX <= _maximumSpeed * -0.9f && _isGrounded))
                {
                    _boosterTimer += Time.deltaTime;
                }
                else { _boosterTimer = 0.0f; }
                if (_boosterTimer >= _boosterActivationTime && !_boosterActive)
                {
                    ActivateBooster();
                }
                if ((_boosterActive && _rb.linearVelocityX > 0.0f && !_applyMoveRight)
                    || (_boosterActive && _rb.linearVelocityX < 0.0f && !_applyMoveLeft)
                    || _boosterActive && _rb.linearVelocityX == 0.0f)
                {
                    _boosterActive = false;
                    _boosterGraceActive = true;
                }
                if (_boosterGraceActive) _boosterGrace += Time.deltaTime;
                if (_boosterGrace >= _boosterGraceTime)
                {
                    _boosterGraceActive = false;
                    DeactivateBooster();
                    _boosterGrace = 0.0f;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_applyMoveLeft && _rb.linearVelocityX >= _maximumSpeed * -1) _rb.AddForce(-_horizontalForce, ForceMode2D.Force);
            if (_applyMoveRight && _rb.linearVelocityX <= _maximumSpeed) _rb.AddForce(_horizontalForce, ForceMode2D.Force);
            if (_jumpBuffer)
            {
                _rb.AddForce(_jumpForce, ForceMode2D.Impulse);
                _coyoteDuration = 0;
                _jumpCooldown = 0.0f;
                _rb.gravityScale = 1.0f;
                _jumpBuffer = false;
            }

            _leftEdge = (Vector2)_transform.position - _edgeOffset;
            _rightEdge = (Vector2)_transform.position + _edgeOffset;

            if (_jumpCooldown > 0.25f && (Physics2D.Raycast(_leftEdge, Vector2.down, _groundCheckRayDist, _envLayerMask)
                | Physics2D.Raycast(_rightEdge, Vector2.down, _groundCheckRayDist, _envLayerMask)))
            {
                StopCoroutine(CoyoteTime());
                _jumpForce = _baseJumpForce;
                _coyoteActive = false;
                _isGrounded = true;
                //_rb.sharedMaterial.friction = _physMatFriction;
                //_coll.enabled = false;
                //_coll.enabled = true;
                _coyoteDuration = _resetCoyoteDuration;
            }
            else if (_isGrounded && !_coyoteActive) StartCoroutine(CoyoteTime());
        }

        private void ActivateBooster()
        {
            _movementSpeed = _boosterImpulseSpeed;
            _maximumSpeed = _boosterSpeed;
            _horizontalForce = new Vector2(_movementSpeed, 0.0f);
            _boosterActive = true;
            _playerSprite.color = Color.cyan;
        }

        private void DeactivateBooster()
        {
            _playerSprite.color = _playerColor;
            _movementSpeed = _originalMoveSpeed;
            _maximumSpeed = _originalMaxSpeed;
            _horizontalForce = new Vector2(_movementSpeed, 0.0f);
        }

        public void UpdateOffsets()
        {
            _edgeOffset = new Vector2((_transform.localScale.x / 2) + _groundCheckRayOffset, _transform.position.y);
            _groundCheckRayDist = (_transform.localScale.y / 2) + 0.05f;
        }

        /// <summary>
        /// Coroutine that implements coyote time, wall jumping and wall sliding on Fixed Update cycle.
        /// </summary>
        private IEnumerator CoyoteTime()
        {
            _coyoteActive = true;
            float coyoteTimeLeft = _coyoteDuration;
            while (coyoteTimeLeft > 0.0f)
            {
                if (Physics2D.Raycast(_leftEdge, Vector2.left, _groundCheckRayOffset * 20, _envLayerMask))
                {
                    _jumpForce = _wallJumpForceRight;
                    if (_playerCore.HasWallHang)
                    {
                        if (_applyMoveLeft && _rb.sharedMaterial.friction != _wallSlideFriction)
                        {
                            _rb.sharedMaterial.friction = _wallSlideFriction;
                            _coll.enabled = false;
                            _coll.enabled = true;
                        }
                        else if (!_applyMoveLeft)
                        {
                            _rb.sharedMaterial.friction = _physMatFriction;
                            _coll.enabled = false;
                            _coll.enabled = true;
                        } 
                    }
                    coyoteTimeLeft += Time.fixedDeltaTime;
                }
                else if (Physics2D.Raycast(_rightEdge, Vector2.right, _groundCheckRayOffset * 20, _envLayerMask))
                {
                    _jumpForce = _wallJumpForceLeft;
                    if (_playerCore.HasWallHang)
                    {
                        if (_applyMoveRight && _rb.sharedMaterial.friction != _wallSlideFriction)
                        {
                            _rb.sharedMaterial.friction = _wallSlideFriction;
                            _coll.enabled = false;
                            _coll.enabled = true;
                        }
                        else if (!_applyMoveRight)
                        {
                            _rb.sharedMaterial.friction = _physMatFriction;
                            _coll.enabled = false;
                            _coll.enabled = true;
                        } 
                    }
                    coyoteTimeLeft += Time.fixedDeltaTime;
                }
                else
                {
                    _jumpForce = _baseJumpForce;
                    _rb.gravityScale = 1.0f;
                    _rb.sharedMaterial.friction = _physMatFriction;
                    _coll.enabled = false;
                    _coll.enabled = true;
                }
                coyoteTimeLeft -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            _isGrounded = false;
            _coyoteActive = false;
        }
    }

}