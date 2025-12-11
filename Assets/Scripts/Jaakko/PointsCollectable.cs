using System;
using UnityEngine;

namespace AG3958
{
    [Serializable, RequireComponent (typeof(Collider2D))]
    public class PointsCollectable : MonoBehaviour, ICollectable
    {
        private readonly ICollectable.CollectableType _cType = ICollectable.CollectableType.Points;
        public ICollectable.CollectableType CType { get { return _cType; } }
        [SerializeField] private float _value;
        public float CValue { get { return _value; } }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) CollectObject();
            else return;
        }

        public void CollectObject()
        {
            PlayerCore.PointChangeEvent?.Invoke(_value);
            Destroy(this.gameObject);
        }
    }

}