using UnityEngine;

namespace AG3958
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Transform _playerCollider;
        private Transform _transform;
        private Vector3 _zOffset;
        private bool _following = true;

        private void Awake()
        {
            if (_playerCollider == null)
            {
                _playerCollider = FindObjectsByType<PlayerController>(FindObjectsSortMode.None)[0].transform;
            }
            _transform = transform;
            _zOffset = new Vector3(0f, 0f, -1f);
        }

        private void LateUpdate()
        {
            if (_following) _transform.position = _playerCollider.position + _zOffset;
        }

        public void ToggleFollowing()
        {
            _following = !_following;
        }
    }
}