using UnityEngine;
using EditorAttributes;
using System;

namespace AG3958
{
    [Serializable]
    public class PlayerCore : MonoBehaviour
    {
        public static Action<float> HealthChangeEvent;
        public static Action<float> ManaChangeEvent;
        public static Action<float> PointChangeEvent;

        [Header("Basic Stats")]
        [SerializeField, Clamp(1, Single.MaxValue)] private float _maxHealth;
        private float _currentHealth;
        public float PlayerHealth { get { return _currentHealth; } }

        [SerializeField, Clamp(1, Single.MaxValue)] private float _maxMana;
        private float _currentMana;
        public float PlayerMana { get { return _currentMana; } }

        [SerializeField, Clamp(0, Single.MaxValue)] private float _points = 0f;
        public float PlayerPoints { get { return _points; } }

        [Header("Progression Checks")]
        [SerializeField] private bool _hasSpeedBooster;
        public bool HasSpeedBooster { get { return _hasSpeedBooster; } }

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _currentMana = _maxMana;

            HealthChangeEvent += OnHealthChanged;
            ManaChangeEvent += OnManaChanged;
            PointChangeEvent += OnPointsChanged;
        }

        private void OnHealthChanged(float value)
        {
            _currentHealth += value;
        }

        private void OnManaChanged(float value)
        {
            _currentMana += value;
        }

        private void OnPointsChanged(float value)
        {
            _points += value;
        }
    }
}