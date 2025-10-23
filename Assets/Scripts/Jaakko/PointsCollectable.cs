using UnityEngine;

namespace AG3958
{
    [RequireComponent (typeof(Collider2D))]
    public class PointsCollectable : MonoBehaviour, ICollectable
    {
        [SerializeField] private ICollectable.CollectableType _cType;
        public ICollectable.CollectableType CType { get { return _cType; } }
        [SerializeField] private float _value;
        public float CValue { get { return _value; } }

        private PlayerCore _pc;

        private void Awake()
        {
            _pc = FindFirstObjectByType<PlayerCore>();
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            
        }
    }

}