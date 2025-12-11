using UnityEngine;

namespace AG3958
{
    public enum DamageType { Melee, Ranged, Charge, Enemy }

    public class Projectile : MonoBehaviour
    {
        // class is agnostic to projectile source, determined by prefab tag

        public DamageType ProjectileDamageType;
        [SerializeField] private float _damage;
        [SerializeField] private bool _invokesIFrames;
        [SerializeField] private float _lifetime;
        [Tooltip("If true, colliding does not destroy the projectile")]
        [SerializeField] private bool _destroyedByCollision = true;
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
            if (coll.collider.CompareTag("Enemy") && ProjectileDamageType != DamageType.Enemy)
            {
                Enemy e = coll.gameObject.GetComponent<Enemy>();
                if (e.EffectiveDamageTypes.Contains(ProjectileDamageType))
                { 
                    // instantiate vfx/sfx for effective projectile impact
                    e.TakeDamage(_damage);
                }
                else 
                { 
                    // instantiate vfx/sfx for ineffective projectile impact
                }
            }
            if (coll.collider.CompareTag("Player") && ProjectileDamageType == DamageType.Enemy)
            {
                PlayerController pcon = coll.gameObject.GetComponent<PlayerController>();
                PlayerCore pcor = coll.gameObject.GetComponentInParent<PlayerCore>();
                if (!pcor.IsInvincible)
                {
                    if (_knockbackStrength > 0)
                    {
                        Vector2 kbVector = (Vector2)coll.transform.position - (Vector2)this.transform.position;
                        kbVector.Scale(_knockbackForceMultiplier);
                        pcon.Launch(kbVector, _isHeavyKnockback);
                    }
                    PlayerCore.HealthChangeEvent?.Invoke(-_damage, _invokesIFrames);
                }
            }

            if (_destroyedByCollision) { Destroy(this.gameObject); }
        }

        //private void OnDestroy()
        //{
        //    // instantiate vfx/sfx for generic projectile destruction
        //}
    } 
}