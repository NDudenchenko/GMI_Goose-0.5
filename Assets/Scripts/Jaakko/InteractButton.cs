using UnityEngine;
using UnityEngine.Events;
// using FMODUnity;

namespace AG3958
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractButton : MonoBehaviour
    {
        public UnityEvent OnButtonInteract;

        private bool _playerInside = false;
        [SerializeField] private bool _isUsable = true;
        [SerializeField] private bool _isOneShot = true;
        [SerializeField] private KeyCode _interactKey = KeyCode.E;
        // [SerializeField] private EventReference _interactSound;
        // [SerializeField] private EventReference _inactiveSound;
        [SerializeField] private SpriteRenderer _buttonRenderer;
        [SerializeField] private Sprite _buttonActiveSprite, _buttonInactiveSprite;

        private void Awake()
        {
            OnButtonInteract ??= new UnityEvent();

            if (_isUsable) _buttonRenderer.sprite = _buttonActiveSprite;
            else _buttonRenderer.sprite = _buttonInactiveSprite;
        }

        private void Update()
        {
            if (_playerInside && _isUsable)
            {
                if (Input.GetKeyDown(_interactKey))
                {
                    OnButtonInteract?.Invoke();
                    // AudioManager.FMODPlayOneShot(this.gameObject, _interactSound);
                    if (_isOneShot) _isUsable = false;
                }
            }
            else if (_playerInside && !_isUsable)
            {
                if (Input.GetKeyDown(_interactKey))
                {
                    // AudioManager.FMODPlayOneShot(this.gameObject, _inactiveSound);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _playerInside = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _playerInside = false;
            }
        }

        public void ToggleUsable()
        {
            _isUsable = !_isUsable;
            if (_isUsable) _buttonRenderer.sprite = _buttonActiveSprite;
            else _buttonRenderer.sprite = _buttonInactiveSprite;
        }
    }

}