using UnityEngine;
using EditorAttributes;
using System;
using System.Collections;

namespace AG3958
{
    [Serializable]
    public class PlayerCore : MonoBehaviour
    {
        public static Action<float, bool> HealthChangeEvent;
        public static Action<float> ManaChangeEvent;
        public static Action<float> PointChangeEvent;

        [Header("Basic Stats")]
        [SerializeField, Clamp(1, Single.MaxValue)] private float _maxHealth;
        private float _currentHealth;
        public float PlayerHealth { get { return _currentHealth; } }

        [Tooltip("Invincibility frames (based on Fixed Update framerate)")]
        [SerializeField] private int _iFrames = 100;
        private bool _invincible = false;
        public bool IsInvincible { get { return _invincible; } }
        private WaitForFixedUpdate _waitForFixedUpdate;

        [SerializeField, Clamp(1, Single.MaxValue)] private float _maxMana;
        private float _currentMana;
        public float PlayerMana { get { return _currentMana; } }

        [Tooltip("Mana regenerates 10x this much per second while regen is active")]
        [SerializeField] private float _manaRegenSpeed = 1.0f;
        private WaitForSeconds _manaRegenWait = new WaitForSeconds(0.1f);
        [SerializeField] private float _manaRegenTime = 3.0f;
        private float _manaRegenTimer = 0.0f;
        private bool _manaRegenActive = false;

        [SerializeField, Clamp(0, Single.MaxValue)] private float _points = 0f;
        public float PlayerPoints { get { return _points; } }

        [Header("Progression Checks")]
        [SerializeField] private bool _hasWeapon;
        public bool HasWeapon { get { return _hasWeapon; } }

        [SerializeField] private bool _hasMagic;
        public bool HasMagic { get { return _hasMagic; } }

        [SerializeField] private bool _hasWallHang;
        public bool HasWallHang { get { return _hasWallHang; } }

        [SerializeField] private bool _hasCharge;
        public bool HasCharge { get { return _hasCharge; } }

        [SerializeField] private bool _hasVolcanicEruption;
        public bool HasVolcanicEruption { get { return _hasVolcanicEruption; } }

        [SerializeField] private bool _hasSpeedBooster;
        public bool HasSpeedBooster { get { return _hasSpeedBooster; } }

        [Header("Debugging")]
        [SerializeField] private Checkpoint _initialCheckpoint;
        [SerializeField, ReadOnly] private Checkpoint _previousCheckpoint;
        private Collider2D _pc;
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

        private void Update()
        {
            _manaRegenTimer += Time.deltaTime;
            if (_currentMana < _maxMana && !_manaRegenActive && _manaRegenTimer >= _manaRegenTime)
                { StartCoroutine(ManaRecharge()); }
        }

        private void OnHealthChanged(float value, bool useIFrames)
        {
            if (_currentHealth + value > _maxHealth) { _currentHealth = _maxHealth; }
            else if (_currentHealth + value < 0) { _currentHealth = 0; }
            else { _currentHealth += value; }
            if (useIFrames) StartCoroutine(InvincibilityFrames());
        }

        private void OnManaChanged(float value)
        {
            if (_currentMana + value > _maxMana)
            {
                _currentMana = _maxMana;
                StopCoroutine(ManaRecharge());
                _manaRegenActive = false;
            }
            else if (_currentMana + value < 0) { _currentMana = 0; }
            else { _currentMana += value; }
            if (value < 0) { _manaRegenTimer = 0.0f; }
        }

        private void OnPointsChanged(float value)
        {
            if (_points + value < 0) { _points = 0; }
            else { _points += value; }
        }

        private IEnumerator ManaRecharge()
        {
            _manaRegenActive = true;
            while (_currentMana < _maxMana)
            {
                ManaChangeEvent?.Invoke(_manaRegenSpeed);
                yield return _manaRegenWait;
            }
            _manaRegenActive = false;
        }

        private IEnumerator InvincibilityFrames()
        {
            _invincible = true;
            int iterator = 0;
            while (iterator < _iFrames)
            {
                iterator++;
                yield return _waitForFixedUpdate;
            }
            _invincible = false;
        }

        public void SetCheckpoint(Checkpoint point)
        {
            _previousCheckpoint = point;
        }
    }
}