using UnityEngine;

namespace AG3958
{
    [RequireComponent (typeof(Collider2D))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private PhysicsButton btn;
        [SerializeField] private Transform target;
        [SerializeField] private float moveSpeed = 1f;
        private float moveStep;
        private bool moving = false;
        private Vector2 originalPosition;

        private void Awake()
        {
            originalPosition = transform.position;
        }

        private void Update()
        {
            if (moving)
            {
                moveStep = moveSpeed * Time.deltaTime;
                if (Vector2.Distance (transform.position, target.position) > 0.001f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.position, moveStep);
                }
                else
                {
                    moving = false;
                    target.position = originalPosition;
                    if (!btn.IsOneShot) btn.Reenable();
                }
            }
        }

        public void ToggleMove()
        {
            originalPosition = transform.position;
            moving = true;
        }
    } 
}