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

        [Header("Debugging")]
        [SerializeField] private Checkpoint _initialCheckpoint;
        [SerializeField, ReadOnly] private Checkpoint _previousCheckpoint;
        public Checkpoint PreviousCheckpoint { get { return _previousCheckpoint; } }

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _currentMana = _maxMana;
            _previousCheckpoint = _initialCheckpoint;

            HealthChangeEvent += OnHealthChanged;
            ManaChangeEvent += OnManaChanged;
            PointChangeEvent += OnPointsChanged;
        }

        private void OnHealthChanged(float value)
        {
            if (_currentHealth + value > _maxHealth) _currentHealth = _maxHealth;
            else if (_currentHealth + value < 0) _currentHealth = 0;
            else _currentHealth += value;
        }

        private void OnManaChanged(float value)
        {
            if (_currentMana + value > _maxMana) _currentMana = _maxMana;
            else if (_currentMana + value < 0) _currentMana = 0;
            else _currentMana += value;
        }

        private void OnPointsChanged(float value)
        {
            if (_points + value < 0) _points = 0;
            else _points += value;
        }

        public void SetCheckpoint(Checkpoint point)
        {
            _previousCheckpoint = point;
        }
    }
}