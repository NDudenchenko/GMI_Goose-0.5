using UnityEngine;
using System.Collections.Generic;

namespace AG3958
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _maxHealth = 1.0f;
        private float _currentHealth;
        [SerializeField] private float _scoreValue = 1.0f;

        [Header("Combat")]
        [SerializeField] private bool _doesContactDamage = true;
        [SerializeField, Tooltip("Field is unused if contact damage is turned off")] private float _contactDamage = 1.0f;
        [SerializeField] private bool _contactInvokesIFrames = true;
        [SerializeField] private bool _knockbackEnabled = true;
        [SerializeField] private float _knockbackStrength = 1.0f;
        private Vector2 _knockbackForceMultiplier;

        [Tooltip("List of projectile types that can deal damage")]
        public List<DamageType> EffectiveDamageTypes;

        [Header("On Death")]
        [Tooltip("Base perccentage chance for this enemy to drop health on death")]
        [SerializeField] private int _healthDropChance = 10;
        [SerializeField] private GameObject _healthDropPrefab;
        private HealthCollectable _healthDrop;
        [SerializeField] private GameObject _normalDeathParticles;
        [SerializeField] private GameObject _instantDeathParticles;
        private bool _isDying = false;
        public bool IsDying { get { return _isDying; } }

        private void Awake()
        {
            _knockbackForceMultiplier = new Vector2(_knockbackStrength, _knockbackStrength);
            _healthDrop = _healthDropPrefab.GetComponent<HealthCollectable>();
            _currentHealth = _maxHealth;
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.collider.CompareTag("Speed")) { Kill(true); }
            if (coll.collider.CompareTag("Player"))
            {
                PlayerController pcon = coll.gameObject.GetComponent<PlayerController>();
                PlayerCore pcor = coll.gameObject.GetComponentInParent<PlayerCore>();
                if (_doesContactDamage && !pcor.IsInvincible)
                {
                    if (_knockbackEnabled)
                    {
                        Vector2 kbVector = (Vector2)coll.transform.position - (Vector2)transform.position;
                        kbVector.Scale(_knockbackForceMultiplier);
                        pcon.Launch(kbVector * _knockbackStrength, true);
                    }
                    PlayerCore.HealthChangeEvent?.Invoke(-_contactDamage, _contactInvokesIFrames);
                }
            }
        }

        public void TakeDamage(float damage) 
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0) { Kill(false); }
            else { } // enemy-specific damaged vfx/sfx
        }

        private void Kill(bool instant)
        {
            int RNGResult = Random.Range(0, 100);
            if (instant)
            { 
                // instant (speed) kill vfx/sfx
                if (RNGResult < _healthDropChance)
                {
                    PlayerCore.HealthChangeEvent?.Invoke(_healthDrop.CValue, false);
                }
            }
            else 
            {
                // normal kill vfx/sfx
                if (RNGResult < _healthDropChance)
                {
                    Instantiate(_healthDropPrefab, transform.position, Quaternion.identity);
                }
            }
            PlayerCore.PointChangeEvent?.Invoke(_scoreValue);
            _isDying = true;
            Destroy(this.gameObject);
        }
    }
}