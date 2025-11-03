using UnityEngine;

namespace AH4063
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TimerPlatform : GlobalTimerBehaviour
    {
        [SerializeField] private float speed = 5.0f;
        
        private Vector3 _currentEndpoint;
        private bool IsAtRight = true;
        
        void Start()
        {
            _currentEndpoint = transform.position;
        }

        void FixedUpdate()
        {
            transform.position = Vector2.MoveTowards(transform.position, _currentEndpoint, speed * Time.deltaTime);
        }

        void GoToEndpoint()
        {
            _currentEndpoint = IsAtRight == true ?
                new Vector3(_currentEndpoint.x - 10.0f, _currentEndpoint.y, _currentEndpoint.z): // to the left
                new Vector3(_currentEndpoint.x + 10.0f, _currentEndpoint.y, _currentEndpoint.z); // to the right
        }

        protected override void OnGlobalCycle()
        {
            GoToEndpoint();
            IsAtRight = IsAtRight != true;
        }
    }
}
