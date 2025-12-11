using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AG3958
{
    // BreakType and DamageType in Projectile must have the same IDs
    public enum BreakType
    {
        Melee,
        Ranged,
        Charge,
        Enemy,
        Player,
        Speed,
        Indestructible
    }

    [RequireComponent(typeof(Collider2D))]
    public class DestructibleBlock : MonoBehaviour
    {
        [Header("Block Attributes")]
        [SerializeField] private List<BreakType> _breakTypes;
        [Tooltip("Break Time should stay 0 unless using Enemy and/or Player Break")]
        [SerializeField] private float _breakTime = 0f;
        [SerializeField] private bool _isCascading;
        [SerializeField] private bool _isRegenerating;
        [SerializeField] private float _regenTime;
        [Header("Block Sprites")]
        [SerializeField] private SpriteRenderer _fgRenderer;
        [SerializeField] private SpriteRenderer _mainRenderer;
        [SerializeField] private Sprite _mainSprite;
        [SerializeField] private Sprite _breakingSprite;

        private bool _breakActive = false;
        private bool _regenActive = false;
        private float _breakTimer = 0f;
        private float _regenTimer = 0f;
        private Collider2D _blockCollider;
        private LayerMask _cascadeLayers;
        private Color _mainColorOpaque;
        private Color _fgColorOpaque;
        private Color _transparentColor;

        private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private void Awake()
        {
            _blockCollider = GetComponent<Collider2D>();
            _cascadeLayers = LayerMask.GetMask("Block");
            _mainColorOpaque = _mainRenderer.color;
            _fgColorOpaque = _fgRenderer.color;
            _transparentColor = new Color(122f, 122f, 122f, 0f);
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (_breakActive) return;
            if ((coll.collider.CompareTag("Enemy") || coll.collider.CompareTag("EnemyProjectile")) && _breakTypes.Contains(BreakType.Enemy))
                { StartCoroutine(Break()); return; }
            else if (coll.collider.CompareTag("Speed") && _breakTypes.Contains(BreakType.Speed))
                { StartCoroutine(Break()); return; }
            else if (coll.collider.CompareTag("Player") && _breakTypes.Contains(BreakType.Player))
                { StartCoroutine(Break()); return; }
            else if (coll.collider.CompareTag("PlayerProjectile"))
            {
                if (_breakTypes.Contains((BreakType)coll.gameObject.GetComponent<Projectile>().ProjectileDamageType))
                { StartCoroutine(Break()); return; }
                else _fgRenderer.color = _transparentColor;
            }
        }

        public IEnumerator Break()
        {
            if (_breakActive) yield break;
            _breakActive = true;
            _fgRenderer.color = _transparentColor;
            _mainRenderer.sprite = _breakingSprite;
            while (_breakTimer < _breakTime)
            {
                _breakTimer += Time.fixedDeltaTime;
                yield return _waitForFixedUpdate;
            }
            _blockCollider.enabled = false;
            _mainRenderer.color = _transparentColor;

            if (_isCascading)
            {
                RaycastHit2D[] cascadeCast = Physics2D.BoxCastAll((Vector2)transform.position, (Vector2)transform.localScale * 2,
                                            transform.rotation.eulerAngles.x, Vector2.zero, 0f, _cascadeLayers);
                foreach (RaycastHit2D hit in cascadeCast)
                {
                    if (hit.collider.CompareTag("DestBlock"))
                    {
                        if (hit.collider.gameObject.TryGetComponent<DestructibleBlock>(out DestructibleBlock db))
                            { StartCoroutine(db.Break()); }
                    }
                }
            }

            if (_isRegenerating)
            {
                _regenActive = true;
                while (_regenTimer < _regenTime)
                {
                    _regenTimer += Time.fixedDeltaTime;
                    yield return _waitForFixedUpdate;
                }
                _mainRenderer.sprite = _mainSprite;
                _mainRenderer.color = _mainColorOpaque;
                _fgRenderer.color = _fgColorOpaque;
                _blockCollider.enabled = true;
                _regenActive = false;
                _regenTimer = 0f;
            }

            _breakTimer = 0f;
            _breakActive = false;
        }
    }
}