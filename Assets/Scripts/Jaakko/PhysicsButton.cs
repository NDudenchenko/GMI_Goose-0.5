using UnityEngine;

namespace AG3958
{
    [RequireComponent(typeof(Collider2D),typeof(SpriteRenderer))]
    public class PhysicsButton : MonoBehaviour
    {
        [SerializeField] private Door _door;
        [SerializeField] private bool _isOneShot;
        public bool IsOneShot { get { return _isOneShot; } }
        [SerializeField] private bool _isEnabled;
        [SerializeField] private Sprite _buttonSpriteEnabled;
        [SerializeField] private Sprite _buttonSpriteDisabled;
        private SpriteRenderer _buttonSpriteR;


        private void Awake()
        {
            _buttonSpriteR = this.gameObject.GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isEnabled)
            {
                _door.ToggleMove();
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