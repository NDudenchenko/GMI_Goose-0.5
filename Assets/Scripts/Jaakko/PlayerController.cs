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

        // Editor parameters
        [SerializeField] private float movementSpeed = 3.0f;
        [SerializeField] private float maximumSpeed = 5.0f;
        [SerializeField] private float jumpPower = 7.5f;
        [SerializeField] private float coyoteDuration = 0.5f;

        // Editor param-derived helper variables
        private Vector2 horizontalForce;
        private Vector2 jumpForce;
        private float resetCoyoteDuration;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _envLayerMask = LayerMask.GetMask("Default");
            _leftEdge = new Vector2(transform.position.x - (transform.localScale.x / 2) + 0.01f, transform.position.y);
            _rightEdge = new Vector2(transform.position.x + (transform.localScale.x / 2) - 0.01f, transform.position.y);
            coyoteActive = false;
            resetCoyoteDuration = coyoteDuration;
            groundCheckRayDist = (transform.localScale.y / 2) + 0.05f;
            horizontalForce = new Vector2(movementSpeed, 0);
            jumpForce = new Vector2(0, jumpPower);
        }

        private void Update()
        {
            jumpCooldown += Time.deltaTime;
            if (Input.GetKey(KeyCode.A) && _rb.linearVelocity.x <= maximumSpeed) { _rb.AddForce(-horizontalForce, ForceMode2D.Force); }
            if (Input.GetKey(KeyCode.D) && _rb.linearVelocity.x <= maximumSpeed) { _rb.AddForce(horizontalForce, ForceMode2D.Force); }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded) { _rb.AddForce(jumpForce, ForceMode2D.Impulse); coyoteDuration = 0; jumpCooldown = 0.0f; }

            _leftEdge = new Vector2(transform.position.x - (transform.localScale.x / 2) + 0.01f, transform.position.y);
            _rightEdge = new Vector2(transform.position.x + (transform.localScale.x / 2) - 0.01f, transform.position.y);
            groundCheckRayDist = (transform.localScale.y / 2) + 0.05f;

            if (jumpCooldown > 0.25f && (Physics2D.Raycast(_leftEdge, Vector2.down, groundCheckRayDist, _envLayerMask) | Physics2D.Raycast(_rightEdge, Vector2.down, groundCheckRayDist, _envLayerMask)))
            {
                StopCoroutine(CoyoteTime());
                coyoteActive = false;
                isGrounded = true;
                coyoteDuration = resetCoyoteDuration;
            }
            else if (isGrounded) StartCoroutine(CoyoteTime());
        }

        private IEnumerator CoyoteTime()
        {
            coyoteActive = true;
            yield return new WaitForSeconds(coyoteDuration);
            isGrounded = false;
            coyoteActive = false;
        }
    }

}