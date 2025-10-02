using UnityEngine;

namespace AH4063
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformerElevator : MonoBehaviour
    {
        [SerializeField]
        private Transform topPoint, bottomPoint;
        [SerializeField]
        private float speed = 2f;
        [SerializeField]
        private KeyCode interactKey = KeyCode.E;

        private BoxCollider2D _boxCollider;
        private bool _playerInside = false;
        private bool _goingUp = false;
        private bool _isMoving = false;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        void Update()
        {
            if (_playerInside && !_isMoving)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    _isMoving = true;
                    _goingUp = !_goingUp;
                }
            }

            if (_isMoving)
            {
                Transform target = _goingUp ? topPoint : bottomPoint;
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, target.position) < 0.01f)
                {
                    transform.position = target.position;
                    _isMoving = false;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
            }
        }
    }
}
