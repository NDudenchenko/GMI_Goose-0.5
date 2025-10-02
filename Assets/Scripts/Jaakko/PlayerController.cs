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
    /// Basic control and movement script for a 2D player entity. Attached object must have a 2D Rigidbody.
    /// </summary>
    [RequireComponent (typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        // Object references
        private Rigidbody2D _rb;
        private Collider2D _coll;
        private LayerMask _envLayerMask;
        private Vector2 _leftEdge;
        private Vector2 _rightEdge;

        // Script-local state identifiers and helpers
        private bool _applyMoveRight = false;
        private bool _applyMoveLeft = false;
        private bool _jumpBuffer = false;
        private bool _fireBuffer = false;
        private bool _isGrounded;
        private bool _coyoteActive;
        private float _jumpCooldown = 0f;
        private float _groundCheckRayDist;
        private float _groundCheckRayOffset;
        private float _physMatFriction;

        // Editor parameters
        [SerializeField] private float _movementSpeed = 3.0f;
        [SerializeField] private float _maximumSpeed = 5.0f;
        [SerializeField] private float _jumpPower = 7.5f;
        [SerializeField] private float _coyoteDuration = 0.5f;
        [SerializeField] private float _wallSlideFriction = 1.0f;

        // Editor param-derived variables
        private Vector2 _horizontalForce;
        private Vector2 _jumpForce;
        private Vector2 _baseJumpForce;
        private Vector2 _wallJumpForceRight;
        private Vector2 _wallJumpForceLeft;
        private float _resetCoyoteDuration;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
            _envLayerMask = LayerMask.GetMask("Default");
            _groundCheckRayOffset = 0.01f; // making this very slightly positive instead of negative enables wall jumping with no additional code
            _physMatFriction = _rb.sharedMaterial.friction;
            _leftEdge = new Vector2(transform.position.x - (transform.localScale.x / 2) - _groundCheckRayOffset, transform.position.y);
            _rightEdge = new Vector2(transform.position.x + (transform.localScale.x / 2) + _groundCheckRayOffset, transform.position.y);
            _coyoteActive = false;
            _resetCoyoteDuration = _coyoteDuration;
            _groundCheckRayDist = (transform.localScale.y / 2) + 0.05f;
            _horizontalForce = new Vector2(_movementSpeed, 0);
            _baseJumpForce = new Vector2(0, _jumpPower);
            _jumpForce = _baseJumpForce;
            _wallJumpForceRight = new Vector2(_jumpPower * 0.75f, _jumpPower);
            _wallJumpForceLeft = new Vector2(-_jumpPower * 0.75f, _jumpPower);
        }

        private void Update()
        {
            _jumpCooldown += Time.deltaTime;
            if (Input.GetKey(KeyCode.A)) { _applyMoveLeft = true; }
            else _applyMoveLeft = false;
            if (Input.GetKey(KeyCode.D) ) { _applyMoveRight = true; }
            else _applyMoveRight = false;
            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) { _jumpBuffer = true; }
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

            _leftEdge = new Vector2(transform.position.x - (transform.localScale.x / 2) - _groundCheckRayOffset, transform.position.y);
            _rightEdge = new Vector2(transform.position.x + (transform.localScale.x / 2) + _groundCheckRayOffset, transform.position.y);
            _groundCheckRayDist = (transform.localScale.y / 2) + 0.05f;

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
            else if (_isGrounded) StartCoroutine(CoyoteTime());
        }

        private IEnumerator CoyoteTime()
        {
            _coyoteActive = true;
            float coyoteTimeLeft = _coyoteDuration;
            while (coyoteTimeLeft > 0.0f)
            {
                if (Physics2D.Raycast(_leftEdge, Vector2.left, _groundCheckRayOffset * 20, _envLayerMask))
                {
                    _jumpForce = _wallJumpForceRight;
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
                    coyoteTimeLeft += Time.fixedDeltaTime;
                }
                else if (Physics2D.Raycast(_rightEdge, Vector2.right, _groundCheckRayOffset * 20, _envLayerMask))
                {
                    _jumpForce = _wallJumpForceLeft;
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