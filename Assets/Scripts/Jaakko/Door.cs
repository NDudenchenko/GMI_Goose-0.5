using UnityEngine;

namespace AG3958
{
    [RequireComponent (typeof(Collider2D))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private PhysicsButton _btn;
        [SerializeField] private Transform _target;
        [SerializeField] private float _moveSpeed = 1f;
        private float _moveStep;
        private bool _moving = false;
        private Vector2 _originalPosition;

        private void Awake()
        {
            _originalPosition = transform.position;
        }

        private void Update()
        {
            if (_moving)
            {
                _moveStep = _moveSpeed * Time.deltaTime;
                if (Vector2.Distance (transform.position, _target.position) > 0.001f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, _target.position, _moveStep);
                }
                else
                {
                    _moving = false;
                    _target.position = _originalPosition;
                    if (!_btn.IsOneShot) _btn.Reenable();
                }
            }
        }

        public void ToggleMove()
        {
            _originalPosition = transform.position;
            _moving = true;
        }
    } 
}