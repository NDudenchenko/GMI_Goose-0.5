using UnityEngine;
using EditorAttributes;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace AG3958
{
    [Serializable]
    public class PlayerCore : MonoBehaviour
    {
        public static Action<float, bool> HealthChangeEvent;
        public static Action<float> ManaChangeEvent;
        public static Action<float> PointChangeEvent;
        public static event Action PlayerDeathEvent;
        public static event Action PlayerRespawnEvent;

        [Header("Basic Stats")]
        [SerializeField, Clamp(1, Single.MaxValue)] private float _maxHealth;
        public float MaxHealth { get { return _maxHealth; } }
        private float _currentHealth;
        public float PlayerHealth { get { return _currentHealth; } }

        [Tooltip("Invincibility frames (based on Fixed Update framerate)")]
        [SerializeField] private int _iFrames = 100;
        private bool _invincible = false;
        public bool IsInvincible { get { return _invincible; } }
        private readonly WaitForFixedUpdate _waitForFixedUpdate;

        [SerializeField, Clamp(1, Single.MaxValue)] private float _maxMana;
        public float MaxMana {  get { return _maxMana; } }
        private float _currentMana;
        public float PlayerMana { get { return _currentMana; } }

        [Tooltip("Mana regeneration speed while regen is active (per 1/50th of second)")]
        [SerializeField] private float _manaRegenSpeed = 1.0f;
        private WaitForSeconds _manaRegenWait = new WaitForSeconds(0.02f);
        [SerializeField] private float _manaRegenTime = 3.0f;
        private float _manaRegenTimer = 0.0f;
        private bool _manaRegenActive = false;
        public bool ManaRegenActive { get { return _manaRegenActive; } }

        [SerializeField, Clamp(0, Single.MaxValue)] private float _points = 0f;
        public float PlayerPoints { get { return _points; } }

        [SerializeField] private float _respawnTime = 5f;
        private float _respawnTimer;

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
            PlayerDeathEvent += Die;
            PlayerRespawnEvent += Respawn;
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
            else if (_currentHealth + value <= 0)
            { 
                _currentHealth = 0;
                PlayerDeathEvent?.Invoke();
            }
            else { _currentHealth += value; }
            if (useIFrames) StartCoroutine(InvincibilityFrames());
        }

        private void OnManaChanged(float value)
        {
            if (_currentMana + value > _maxMana)
            {
                _currentMana = _maxMana;
                StopRecharge();
            }
            else if (_currentMana + value < 0) { _currentMana = 0; }
            else { _currentMana += value; }
            if (value < 0) { StopRecharge(); }
        }

        private void OnPointsChanged(float value)
        {
            if (_points + value < 0) { _points = 0; }
            else { _points += value; }
        }

        private IEnumerator ManaRecharge()
        {
            _manaRegenActive = true;
            while (_currentMana < _maxMana && _manaRegenActive)
            {
                ManaChangeEvent?.Invoke(_manaRegenSpeed);
                yield return _manaRegenWait;
            }
            _manaRegenActive = false;
        }

        public void StopRecharge()
        {
            _manaRegenTimer = 0.0f;
            StopCoroutine(ManaRecharge());
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

        private void Die()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(RespawnTimer());
        }

        private IEnumerator RespawnTimer()
        {
            while (_respawnTimer < _respawnTime)
            {
                _respawnTimer += Time.fixedDeltaTime;
                yield return _waitForFixedUpdate;
            }
            PlayerRespawnEvent?.Invoke();
            _respawnTimer = 0.0f;
        }

        private void Respawn()
        {
            SceneLoader.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            PointChangeEvent = null;
            HealthChangeEvent = null;
            ManaChangeEvent = null;
            PlayerDeathEvent = null;
            PlayerRespawnEvent = null;
        }
    }
}