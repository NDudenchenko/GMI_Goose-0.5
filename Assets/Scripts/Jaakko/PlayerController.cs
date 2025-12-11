using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using Physics2D = Nomnom.RaycastVisualization.VisualPhysics2D;
#else
using Physics2D = UnityEngine.Physics2D;
#endif

namespace AG3958
{
    /// <summary>
    /// Player control and movement script.
    /// </summary>
    [RequireComponent (typeof(Rigidbody2D), typeof(Collider2D))]
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

        // Script-local state identifiers
        private bool _applyMoveRight = false;
        private bool _applyMoveLeft = false;
        private bool _isCrouched = false;
        private bool _facingRight = true;
        private bool _aimingUp = false;
        private bool _aimingDown = false;
        private bool _isStunned = false;
        private bool _jumpBuffer = false;
        private bool _fireBuffer = false;
        private bool _chargeBuffer = false;
        private bool _meleeBuffer = false;
        private bool _chargeActive = false;
        private bool _chargeReady = false;
        private bool _isGrounded = false;
        private bool _coyoteActive = false;
        private bool _boosterActive = false;
        private bool _boosterGraceActive = false;
        private bool _eruptionActive = false;
        private bool _eruptionReady = false;
        private bool _eruptionGraceActive = false;

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        // Properties for animation state script
        public bool ApplyMoveRight { get { return _applyMoveRight; } }
        public bool ApplyMoveLeft {  get { return _applyMoveLeft; } }
        public bool IsCrouched { get { return _isCrouched; } }
        public bool FacingRight {  get { return _facingRight; } }
        public bool AimingUp {  get { return _aimingUp; } }
        public bool AimingDown {  get { return _aimingDown; } }
        public bool IsStunned { get { return _isStunned; } }
        public bool IsGrounded { get { return _isGrounded; } }
        public bool CoyoteActive { get { return _coyoteActive; } }
        public bool ChargeActive { get { return _chargeActive; } }
        public bool ChargeReady {  get { return _chargeReady; } }
        public bool BoosterActive { get { return _boosterActive; } }
        public bool BoosterGraceActive { get { return _boosterGraceActive; } }
        public bool EruptionActive { get { return _eruptionActive; } }
        public bool EruptionReady {  get { return _eruptionReady; } }
        public bool EruptionGraceActive { get { return _eruptionGraceActive; } }


        // Editor parameters
        [Header("Input")]
        [SerializeField] private KeyCode _leftKey = KeyCode.A;
        [SerializeField] private KeyCode _rightKey = KeyCode.D;
        [SerializeField] private KeyCode _upKey = KeyCode.W;
        [SerializeField] private KeyCode _downKey = KeyCode.S;
        [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode _fireKey = KeyCode.L;
        [SerializeField] private KeyCode _meleeKey = KeyCode.K;
        [Header("Basic Movement")]
        [SerializeField] private float _movementSpeed = 3.0f;
        [SerializeField] private float _maximumSpeed = 5.0f;
        [SerializeField] private float _jumpCooldown = 1.0f;
        [SerializeField] private float _jumpPower = 7.5f;
        [SerializeField] private float _coyoteDuration = 0.5f;
        [SerializeField] private float _wallSlideFriction = 1.0f;
        [Header("Hitstun Time")]
        [SerializeField] private float _weakKnockbackStunTime = 0.5f;
        [SerializeField] private float _strongKnockbackStunTime = 1.5f;
        [Header("Melee Attack")]
        [SerializeField] private GameObject _meleeProjectilePrefab;
        [SerializeField] private float _meleeCooldown = 0.5f;
        [Header("Fire Magic")]
        [SerializeField] private GameObject _fireProjectilePrefab;
        [SerializeField] private float _fireCost = 10f;
        [SerializeField] private float _fireCooldown = 1.0f;
        [Tooltip("Projectile speed as velocity units added on instantiation. This is the same for Charge Shots.")]
        [SerializeField] private float _projSpeed = 5.0f;
        [Header("Charge Shot")]
        [SerializeField] private GameObject _chargeProjectilePrefab;
        [SerializeField] private float _chargeTime = 2.5f;
        [SerializeField] private float _chargeCost = 20f;
        [Header("Volcanic Surge")]
        [Tooltip("Movement speed with Volcanic Surge active")]
        [SerializeField] private float _boosterImpulseSpeed;
        [Tooltip("Maximum speed with Volcanic Surge active")]
        [SerializeField] private float _boosterSpeed;
        [SerializeField] private float _boosterActivationTime;
        [SerializeField] private float _boosterGraceTime;
        [SerializeField] private Color _boosterActiveColor;
        [SerializeField] private GameObject _speedActiveAuraObject;
        [Header("Volcanic Eruption")]
        [Tooltip("Horizontal movement force while Volcanic Eruption is active")]
        [SerializeField] private float _eruptionAdjustSpeed = 1.0f;
        [Tooltip("Impulse force")]
        [SerializeField] private float _eruptionImpulseSpeed;
        [SerializeField] private float _eruptionChargeTime;
        [SerializeField] private float _eruptionGraceTime;
        [SerializeField] private Color _eruptionActiveColor;

        // Editor param-derived variables and timers
        private Vector2 _horizontalForce;
        private Vector2 _jumpForce;
        private Vector2 _baseJumpForce;
        private Vector2 _wallJumpForceRight;
        private Vector2 _wallJumpForceLeft;
        private float _resetCoyoteDuration;
        private float _originalMoveSpeed;
        private float _originalMaxSpeed; // store values of normal move/maximum speed in awake
        private float _groundCheckRayDist;
        private float _groundCheckRayOffset;
        private float _physMatFriction;
        private Color _playerColor;
        private Color _playerInvulnerableColor;
        private float _jumpCooldownTimer = 0.0f;
        private float _meleeCooldownTimer = 0.0f;
        private float _fireCooldownTimer = 0.0f;
        private float _chargeTimer = 0.0f;
        private float _boosterTimer = 0.0f;
        private float _boosterGrace = 0.0f;
        private float _eruptionChargeTimer = 0.0f;
        private float _eruptionGrace = 0.0f;
        private Vector2 _eruptionImpulseForce;
        private Vector2 _meleeOffset;
        private Vector2 _rangedOffset;
        private Vector2 _flipVector;
        private Color _boosterAuraColor;
        private Color _eruptionAuraColor;
        private SpriteRenderer _speedAuraSprite;

        private void Awake()
        {
            _transform = transform;
            _playerCore = GetComponentInParent<PlayerCore>();
            _rb = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _envLayerMask = LayerMask.GetMask("Default");
            _groundCheckRayOffset = 0.0125f; // making this very slightly positive instead of negative enables wall jumping with no additional code
            _physMatFriction = _rb.sharedMaterial.friction;
            UpdateOffsets();
            _leftEdge = (Vector2)_transform.position - _edgeOffset;
            _rightEdge = (Vector2)_transform.position + _edgeOffset;
            _playerSprite = GetComponent<SpriteRenderer>();
            _playerColor = _playerSprite.color;
            _playerInvulnerableColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.5f);
            _boosterAuraColor = new Color(_boosterActiveColor.r, _boosterActiveColor.g, _boosterActiveColor.b, 0.3f);
            _eruptionAuraColor = new Color(_eruptionActiveColor.r, _eruptionActiveColor.g, _eruptionActiveColor.b, 0.3f);
            _speedAuraSprite = _speedActiveAuraObject.GetComponent<SpriteRenderer>();
            _resetCoyoteDuration = _coyoteDuration;
            _horizontalForce = new Vector2(_movementSpeed, 0);
            _baseJumpForce = new Vector2(0, _jumpPower);
            _jumpForce = _baseJumpForce;
            _wallJumpForceRight = new Vector2(_jumpPower * 0.75f, _jumpPower);
            _wallJumpForceLeft = new Vector2(-_jumpPower * 0.75f, _jumpPower);
            _eruptionImpulseForce = new Vector2(0.0f, _eruptionImpulseSpeed);
            _originalMoveSpeed = _movementSpeed;
            _originalMaxSpeed = _maximumSpeed;
            _flipVector = new Vector2(0f, 180f);
            _meleeOffset = new Vector2(1.25f, 0f);
            _rangedOffset = new Vector2(0.75f, 0f);
        }

        private void Update()
        {
            _jumpCooldownTimer += Time.deltaTime;
            _meleeCooldownTimer += Time.deltaTime;
            _fireCooldownTimer += Time.deltaTime;

            if (_playerCore.IsInvincible) _playerSprite.color = _playerInvulnerableColor;
            else if (_eruptionReady || _eruptionActive || _boosterActive || _boosterGraceActive) { }
            else _playerSprite.color = _playerColor;

            if (Input.GetKey(_leftKey)) { _applyMoveLeft = true; }
            else { _applyMoveLeft = false; }
           
            if (Input.GetKey(_rightKey)) { _applyMoveRight = true; }
            else { _applyMoveRight = false; }
            
            if (!_coyoteActive)
            {
                if (_applyMoveLeft && !_applyMoveRight && _facingRight)
                {
                    _transform.Rotate(_flipVector);
                    _facingRight = false; 
                }
                else if (_applyMoveRight && !_applyMoveLeft && !_facingRight)
                {
                    _transform.Rotate(_flipVector);
                    _facingRight = true;
                }
                if (Input.GetKey(_downKey) && _isGrounded) { _isCrouched = true; }
                else { _isCrouched = false; }
            }
           
            if (Input.GetKey(_downKey) && !_isCrouched) { _aimingDown = true; }
            else { _aimingDown = false; }
            
            if (Input.GetKey(_upKey)) { _aimingUp = true; }
            else { _aimingUp = false; }
            
            if (Input.GetKeyDown(_jumpKey) && _isGrounded) { _jumpBuffer = true; }
           
            if (Input.GetKeyDown(_meleeKey) && _playerCore.HasWeapon && _meleeCooldownTimer >= _meleeCooldown) { _meleeBuffer = true; }
           
            if (Input.GetKeyUp(_fireKey) && (!_eruptionActive && !_eruptionReady))
            {
                if (_chargeReady) { _chargeBuffer = true; }
                else if (_fireCooldownTimer >= _fireCooldown && _playerCore.PlayerMana >= _fireCost) { _fireBuffer = true; }
                _chargeActive = false;
            }
           
            if (Input.GetKey(_fireKey) && _playerCore.HasMagic && (!_eruptionActive && !_eruptionReady) && _fireCooldownTimer >= _fireCooldown)
            {
                if (_playerCore.HasCharge && !_chargeBuffer && _playerCore.PlayerMana >= _chargeCost)
                {
                    if (!_chargeReady)
                    {
                        _chargeActive = true;
                        _chargeTimer += Time.deltaTime;
                        if (_chargeTimer >= _chargeTime) { _chargeReady = true; }
                    }
                    if (_playerCore.ManaRegenActive) _playerCore.StopRecharge();
                }
                else if (_playerCore.PlayerMana >= _fireCost) _fireBuffer = true;
            }
            else
            {
                _chargeActive = false;
            }

            if (_playerCore.HasSpeedBooster)
            {
                if ((_isGrounded && _rb.linearVelocityX >= _maximumSpeed * 0.9f) || (_isGrounded && _rb.linearVelocityX <= _maximumSpeed * -0.9f)
                    && !_isCrouched)
                {
                    _boosterTimer += Time.deltaTime;
                }
                else { _boosterTimer = 0.0f; }
                if (!_boosterActive && _boosterTimer >= _boosterActivationTime)
                {
                    ActivateBooster();
                }
                if (_boosterActive && _playerCore.HasVolcanicEruption && Input.GetKey(_downKey))
                {
                    _rb.linearVelocity = Vector2.zero;
                    DeactivateBooster();
                    _playerSprite.color = _eruptionActiveColor;
                    _eruptionChargeTimer = _eruptionChargeTime;
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

            if (_playerCore.HasVolcanicEruption)
            {
                if (!_eruptionReady && !_eruptionGraceActive && _isCrouched && _rb.linearVelocity.magnitude < 1.0f)
                {
                    if (_eruptionChargeTimer < _eruptionChargeTime) { _eruptionChargeTimer += Time.deltaTime; }
                    else
                    {
                        _eruptionReady = true;
                        _playerSprite.color = _eruptionActiveColor;
                    }
                }
                else { _eruptionChargeTimer = 0.0f; }
                if (_eruptionReady && _rb.linearVelocity.magnitude > 1.0f)
                { 
                    _eruptionReady = false;
                    _playerSprite.color = _playerColor;
                }
                if (_eruptionReady && Input.GetKeyDown(_jumpKey)) { ActivateEruption(); }
                if (_eruptionActive && _rb.linearVelocityY < 0.1f) { DeactivateEruption(); }
                if (_eruptionGraceActive) { _eruptionGrace += Time.deltaTime; }
                if (_eruptionGrace >= _eruptionGraceTime)
                {
                    _eruptionGraceActive = false;
                    _rb.gravityScale = 1.0f;
                    _eruptionGrace = 0.0f;
                } 
            }
        }

        private void FixedUpdate()
        {
            _leftEdge = (Vector2)_transform.position - _edgeOffset;
            _rightEdge = (Vector2)_transform.position + _edgeOffset;

            if (_jumpCooldownTimer > _resetCoyoteDuration && (Physics2D.Raycast(_leftEdge, Vector2.down, _groundCheckRayDist, _envLayerMask)
                | Physics2D.Raycast(_rightEdge, Vector2.down, _groundCheckRayDist, _envLayerMask)))
            {
                if (_eruptionReady || _eruptionActive) _jumpForce = _eruptionImpulseForce;
                else _jumpForce = _baseJumpForce;
                _isGrounded = true;
                //_rb.sharedMaterial.friction = _physMatFriction;
                //_coll.enabled = false;
                //_coll.enabled = true;
                _coyoteDuration = _resetCoyoteDuration;
            }
            else if (_isGrounded && !_coyoteActive) StartCoroutine(CoyoteTime());
            else if (_coyoteActive) _isGrounded = true;
            else _isGrounded = false;

            if (_applyMoveLeft && _rb.linearVelocityX >= _maximumSpeed * -1 && !_isStunned) { _rb.AddForce(-_horizontalForce, ForceMode2D.Force); }
            if (_applyMoveRight && _rb.linearVelocityX <= _maximumSpeed && !_isStunned) { _rb.AddForce(_horizontalForce, ForceMode2D.Force); }
            if (_jumpBuffer && !_isStunned)
            {
                _rb.AddForce(_jumpForce, ForceMode2D.Impulse);
                if (_eruptionReady)
                { 
                    _eruptionReady = false;
                    _eruptionActive = true;
                }
                StopCoroutine(CoyoteTime());
                _coyoteActive = false;
                _isGrounded = false;
                _coyoteDuration = 0;
                _jumpCooldownTimer = 0.0f;
                _jumpBuffer = false;
            }

            if (_meleeBuffer && !_isStunned)
            {
                if (_facingRight) Instantiate(_meleeProjectilePrefab, (Vector2)_transform.position + _meleeOffset, Quaternion.identity);
                else Instantiate(_meleeProjectilePrefab, (Vector2)_transform.position - _meleeOffset, Quaternion.identity);
                _meleeCooldownTimer = 0.0f;
                _meleeBuffer = false;
            }
            if (_fireBuffer && !_isStunned)
            {
                FireProjectile(_fireProjectilePrefab);
                PlayerCore.ManaChangeEvent?.Invoke(-_fireCost);
                _fireBuffer = false;
            }
            if (_chargeBuffer && !_isStunned)
            { 
                FireProjectile(_chargeProjectilePrefab);
                PlayerCore.ManaChangeEvent?.Invoke(-_chargeCost);
                _chargeBuffer = false;
            }
        }

        private void FireProjectile(GameObject projectile)
        {
            Vector2 aimVector;
            if (_aimingUp && (!_applyMoveLeft && !_applyMoveRight)) aimVector = _transform.up;
            else if (_aimingDown && !_isGrounded && (!_applyMoveLeft && !_applyMoveRight)) aimVector = _transform.up * -1;
            else if (_aimingUp) aimVector = _transform.right + _transform.up;
            else if (_aimingDown) aimVector = _transform.right - _transform.up;
            else aimVector = _transform.right;
            aimVector.Normalize();
            Rigidbody2D pRB = Instantiate(projectile, (Vector2)_transform.position + (_rangedOffset * aimVector), _transform.rotation).GetComponent<Rigidbody2D>();
            pRB.linearVelocity = aimVector * _projSpeed;
            _fireCooldownTimer = 0.0f;
        }

        private void ActivateBooster()
        {
            _movementSpeed = _boosterImpulseSpeed;
            _maximumSpeed = _boosterSpeed;
            _horizontalForce = new Vector2(_movementSpeed, 0.0f);
            _boosterActive = true;
            _playerSprite.color = _boosterActiveColor;
            _speedAuraSprite.color = _boosterAuraColor;
            _speedActiveAuraObject.SetActive(true);
        }

        private void DeactivateBooster()
        {
            _boosterActive = false;
            _playerSprite.color = _playerColor;
            _movementSpeed = _originalMoveSpeed;
            _maximumSpeed = _originalMaxSpeed;
            _horizontalForce = new Vector2(_movementSpeed, 0.0f);
            _speedActiveAuraObject.SetActive(false);
        }

        private void ActivateEruption()
        {
            _jumpForce = _eruptionImpulseForce;
            _movementSpeed = _eruptionAdjustSpeed;
            _maximumSpeed = _eruptionAdjustSpeed;
            _rb.gravityScale = 0.0f;
            _speedAuraSprite.color = _eruptionAuraColor;
            _speedActiveAuraObject.SetActive(true);
        }

        private void DeactivateEruption()
        {
            _eruptionActive = false;
            _playerSprite.color = _playerColor;
            _movementSpeed = _originalMoveSpeed;
            _maximumSpeed = _originalMaxSpeed;
            _eruptionGraceActive = true;
            _speedActiveAuraObject.SetActive(false);
        }

        /// <summary>
        /// Updates the edge and ground ray offsets of the player controller. Call when anything changes the player's localScale.
        /// </summary>
        public void UpdateOffsets()
        {
            _edgeOffset = new Vector2((_transform.localScale.x / 2) + _groundCheckRayOffset, _transform.position.y);
            _groundCheckRayDist = (_transform.localScale.y / 2) + 0.05f;
        }

        /// <summary>
        /// Applies an outside impulse force to the player.
        /// </summary>
        /// <param name="launchForce">Force vector to apply</param>
        /// <param name="resetMomentum">Whether the player's velocity is reset before the impulse is applied</param>
        public void Launch(Vector2 launchForce, bool resetMomentum)
        {
            if (resetMomentum)
            { 
                _rb.linearVelocity = Vector2.zero;
                StartCoroutine(HitStun(_strongKnockbackStunTime));
            }
            else StartCoroutine(HitStun(_weakKnockbackStunTime));
            
            _rb.AddForce(launchForce, ForceMode2D.Impulse);
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
                    if (!_facingRight)
                    {
                        _transform.Rotate(_flipVector);
                        _facingRight = true;
                    }
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
                    if (_facingRight)
                    {
                        _transform.Rotate(_flipVector);
                        _facingRight = false;
                    }
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
                    // _rb.gravityScale = 1.0f;
                    _rb.sharedMaterial.friction = _physMatFriction;
                    _coll.enabled = false;
                    _coll.enabled = true;
                }
                coyoteTimeLeft -= Time.fixedDeltaTime;
                yield return _waitForFixedUpdate;
            }
            _isGrounded = false;
            _coyoteActive = false;
        }

        private IEnumerator HitStun(float time)
        {
            _isStunned = true;
            _chargeReady = false;
            _eruptionReady = false;
            yield return new WaitForSeconds(time);
            _isStunned = false;
        }

        // Flush states on enable
        private void OnEnable()
        {
            _applyMoveRight = false;
            _applyMoveLeft = false;
            _isCrouched = false;
            _facingRight = true;
            _aimingUp = false;
            _aimingDown = false;
            _isStunned = false;
            _jumpBuffer = false;
            _fireBuffer = false;
            _chargeBuffer = false;
            _meleeBuffer = false;
            _chargeActive = false;
            _chargeReady = false;
            _isGrounded = false;
            _coyoteActive = false;
            _boosterActive = false;
            _boosterGraceActive = false;
            _eruptionActive = false;
            _eruptionReady = false;
            _eruptionGraceActive = false;
        }
    }
}