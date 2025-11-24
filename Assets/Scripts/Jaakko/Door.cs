using UnityEngine;
using EditorAttributes;

namespace AG3958
{
    [RequireComponent (typeof(Collider2D))]
    public class Door : MonoBehaviour, IPhysicsInteractable
    {
        [SerializeField] private PhysicsButton _btn;
        [SerializeField] private Transform _target;
        private Transform _transform;
        [SerializeField] private float _moveSpeed = 1f;
        private float _moveStep;
        private bool _moving = false;
        private Vector2 _originalPosition;

        private void Awake()
        {
            _transform = transform;
            _originalPosition = _transform.position;
        }

        private void Update()
        {
            if (_moving)
            {
                _moveStep = _moveSpeed * Time.deltaTime;
                if (Vector2.Distance (_transform.position, _target.position) > 0.001f)
                {
                    _transform.position = Vector2.MoveTowards(_transform.position, _target.position, _moveStep);
                }
                else
                {
                    _moving = false;
                    _target.position = _originalPosition;
                    if (_btn != null && !_btn.IsOneShot) _btn.Reenable();
                }
            }
        }

        public void Interact()
        {
            _originalPosition = _transform.position;
            _moving = true;
        }
    } 
}