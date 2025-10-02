using UnityEngine;
using UnityEngine.Events;

namespace AH4063
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PhysicsButton : MonoBehaviour
    {
        public UnityEvent onButtonPressed;
        
        [SerializeField] private float pressDepth = 0.2f;
        [SerializeField] private float pressSpeed = 3f;
    
        private BoxCollider2D _boxCollider;
        private Vector3 _initialPosition;
        private Vector3 _targetPosition;
        private bool _isPressed = false;
        private bool _wasTriggered = false;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _initialPosition = transform.localPosition;
            _targetPosition = _initialPosition;
        }

        private void Update()
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, pressSpeed * Time.deltaTime);

            if (_isPressed && !_wasTriggered && transform.localPosition == _initialPosition + Vector3.down * pressDepth)
            {
                _wasTriggered = true;
                onButtonPressed.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPressed = true;
                _wasTriggered = false;
                _targetPosition = _initialPosition + Vector3.down * pressDepth;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("Exit");
        
            _isPressed = false;
            _targetPosition = _initialPosition;
        }
    }
}
