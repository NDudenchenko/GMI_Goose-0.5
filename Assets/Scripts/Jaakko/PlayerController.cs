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
    [RequireComponent (typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        // Object references
        private Rigidbody2D _rb;
        private LayerMask _envLayerMask;
        private Vector2 _leftEdge;
        private Vector2 _rightEdge;

        // Script-local state identifiers and helpers
        private bool isGrounded;
        private bool coyoteActive;
        private float jumpCooldown = 0f;
        private float groundCheckRayDist;
        private float groundCheckRayOffset;

        // Editor parameters
        [SerializeField] private float movementSpeed = 3.0f;
        [SerializeField] private float maximumSpeed = 5.0f;
        [SerializeField] private float jumpPower = 7.5f;
        [SerializeField] private float coyoteDuration = 0.5f;

        // Editor param-derived helper variables
        private Vector2 horizontalForce;
        private Vector2 jumpForce;
        private Vector2 baseJumpForce;
        private Vector2 wallJumpForceRight;
        private Vector2 wallJumpForceLeft;
        private float resetCoyoteDuration;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _envLayerMask = LayerMask.GetMask("Default");
            groundCheckRayOffset = 0.01f; // making this very slightly positive instead of negative enables wall jumping with no additional code
            _leftEdge = new Vector2(transform.position.x - (transform.localScale.x / 2) - groundCheckRayOffset, transform.position.y);
            _rightEdge = new Vector2(transform.position.x + (transform.localScale.x / 2) + groundCheckRayOffset, transform.position.y);
            coyoteActive = false;
            resetCoyoteDuration = coyoteDuration;
            groundCheckRayDist = (transform.localScale.y / 2) + 0.05f;
            horizontalForce = new Vector2(movementSpeed, 0);
            baseJumpForce = new Vector2(0, jumpPower);
            jumpForce = baseJumpForce;
            wallJumpForceRight = new Vector2(jumpPower * 0.75f, jumpPower);
            wallJumpForceLeft = new Vector2(-jumpPower * 0.75f, jumpPower);
        }

        private void Update()
        {
            jumpCooldown += Time.deltaTime;
            if (Input.GetKey(KeyCode.A) && _rb.linearVelocity.x >= maximumSpeed * -1) { _rb.AddForce(-horizontalForce, ForceMode2D.Force); }
            if (Input.GetKey(KeyCode.D) && _rb.linearVelocity.x <= maximumSpeed) { _rb.AddForce(horizontalForce, ForceMode2D.Force); }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded) { _rb.AddForce(jumpForce, ForceMode2D.Impulse); coyoteDuration = 0; jumpCooldown = 0.0f; }

            _leftEdge = new Vector2(transform.position.x - (transform.localScale.x / 2) - groundCheckRayOffset, transform.position.y);
            _rightEdge = new Vector2(transform.position.x + (transform.localScale.x / 2) + groundCheckRayOffset, transform.position.y);
            groundCheckRayDist = (transform.localScale.y / 2) + 0.05f;

            if (jumpCooldown > 0.25f && (Physics2D.Raycast(_leftEdge, Vector2.down, groundCheckRayDist, _envLayerMask) | Physics2D.Raycast(_rightEdge, Vector2.down, groundCheckRayDist, _envLayerMask)))
            {
                StopCoroutine(CoyoteTime());
                jumpForce = baseJumpForce;
                coyoteActive = false;
                isGrounded = true;
                coyoteDuration = resetCoyoteDuration;
            }
            else if (isGrounded) StartCoroutine(CoyoteTime());
        }

        private IEnumerator CoyoteTime()
        {
            coyoteActive = true;
            float coyoteTimeLeft = coyoteDuration;
            while (coyoteTimeLeft > 0.0f)
            {
                if (Physics2D.Raycast(_leftEdge, Vector2.left, groundCheckRayOffset * 20, _envLayerMask))
                    jumpForce = wallJumpForceRight;
                else if (Physics2D.Raycast(_rightEdge, Vector2.right, groundCheckRayOffset * 20, _envLayerMask))
                    jumpForce = wallJumpForceLeft;
                else jumpForce = baseJumpForce;
                    coyoteTimeLeft -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            isGrounded = false;
            coyoteActive = false;
        }
    }

}