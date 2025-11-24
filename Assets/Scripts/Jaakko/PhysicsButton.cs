using UnityEngine;
using EditorAttributes;

namespace AG3958
{
    [RequireComponent(typeof(Collider2D),typeof(SpriteRenderer))]
    public class PhysicsButton : MonoBehaviour
    {
        [SerializeField] private GameObject _editorConnectedInteractable;
        private IPhysicsInteractable _connectedInteractable;
        [SerializeField] private bool _isOneShot;
        public bool IsOneShot { get { return _isOneShot; } }
        [SerializeField] private bool _isEnabled;
        [SerializeField] private Sprite _buttonSpriteEnabled;
        [SerializeField] private Sprite _buttonSpriteDisabled;
        private SpriteRenderer _buttonSpriteR;


        private void Awake()
        {
            _buttonSpriteR = this.gameObject.GetComponent<SpriteRenderer>();
            if (_editorConnectedInteractable.TryGetComponent<IPhysicsInteractable>(out IPhysicsInteractable interactable))
            {
                _connectedInteractable = interactable;
            }
            else Debug.LogWarning("GameObject " + _editorConnectedInteractable + " attached to " + this.gameObject + " does not have an IPhysicsInteractable component!");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isEnabled && (collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")))
            {
                _connectedInteractable.Interact();
                _isEnabled = false;
                _buttonSpriteR.sprite = _buttonSpriteDisabled;
            }
        }

        public void Reenable()
        {
            _isEnabled = true;
            _buttonSpriteR.sprite = _buttonSpriteEnabled;
        }
    }

}