using System;
using UnityEngine;

namespace AG3958
{
    [Serializable, RequireComponent (typeof(Collider2D))]
    public class HealthCollectable : MonoBehaviour, ICollectable
    {
        private readonly ICollectable.CollectableType _cType = ICollectable.CollectableType.Health;
        public ICollectable.CollectableType CType { get { return _cType; } }
        [SerializeField] private float _value;
        public float CValue { get { return _value; } }

        [Tooltip("Does collecting the item invoke player iframes?")]
        [SerializeField] private bool _invokesIFrames = false;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) CollectObject();
            else return;
        }

        public void CollectObject()
        {
            PlayerCore.HealthChangeEvent?.Invoke(_value, _invokesIFrames);
            Destroy(this.gameObject);
        }
    }

}