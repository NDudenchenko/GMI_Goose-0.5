using UnityEngine;

namespace AG3958
{
    public class Projectile : MonoBehaviour
    {
        // class is agnostic to projectile source, determined by prefab tag
        public enum DamageType { Melee, Ranged, Charge, Enemy }

        [SerializeField] private DamageType _damageType;
        [SerializeField] private float _damage;
        [SerializeField] private bool _invokesIFrames;
        [SerializeField] private float _lifetime;
        [Header("Knockback")]
        [SerializeField] private float _knockbackStrength;
        private Vector2 _knockbackForceMultiplier;
        [Tooltip("Whether the impact resets momentum before applying knockback")]
        [SerializeField] private bool _isHeavyKnockback;

        private void Awake()
        {
            _knockbackForceMultiplier = new Vector2(_knockbackStrength, _knockbackStrength);
            Destroy(this.gameObject, _lifetime);
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.collider.CompareTag("Enemy") && _damageType != DamageType.Enemy)
            {
                Enemy e = coll.gameObject.GetComponent<Enemy>();
                if (e.EffectiveDamageTypes.Contains(_damageType))
                { 
                    // instantiate vfx/sfx for effective projectile impact
                    e.TakeDamage(_damage);
                }
                else 
                { 
                    // instantiate vfx/sfx for ineffective projectile impact
                }
            }
            if (coll.collider.CompareTag("Player") && _damageType == DamageType.Enemy)
            {
                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                if (_knockbackStrength > 0)
                {
                    Vector2 kbVector = (Vector2)coll.transform.position - (Vector2)this.transform.position;
                    kbVector.Scale(_knockbackForceMultiplier);
                    pc.Launch(kbVector, _isHeavyKnockback);
                }
                PlayerCore.HealthChangeEvent?.Invoke(_damage, _invokesIFrames);
            }
            // instantiate vfx/sfx for generic projectile destruction
            Destroy(this.gameObject);
        }
    } 
}