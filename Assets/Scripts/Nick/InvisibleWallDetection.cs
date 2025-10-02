using UnityEngine;
using UnityEngine.Events;

namespace AH4063
{
    public class InvisibleWallDetection : MonoBehaviour
    {
        public UnityEvent onWallRevealed;
    
        private BoxCollider2D _collision;
    
        private void Awake()
        {
            _collision = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject.name);
            if (other.CompareTag("Player"))
            {
                onWallRevealed.Invoke();
                Destroy(this.gameObject);
            }
        }
    }
}
