using UnityEngine;

namespace AG3958
{
    [RequireComponent(typeof(Collider2D),typeof(SpriteRenderer))]
    public class PhysicsButton : MonoBehaviour
    {
        [SerializeField] private Door door;
        [SerializeField] private bool isOneShot;
        public bool IsOneShot { get { return isOneShot; } }
        [SerializeField] private bool isEnabled;
        [SerializeField] private Sprite buttonSpriteEnabled;
        [SerializeField] private Sprite buttonSpriteDisabled;
        private SpriteRenderer buttonSpriteR;


        private void Awake()
        {
            buttonSpriteR = this.gameObject.GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isEnabled)
            {
                door.ToggleMove();
                isEnabled = false;
                buttonSpriteR.sprite = buttonSpriteDisabled;
            }
        }

        public void Reenable()
        {
            isEnabled = true;
            buttonSpriteR.sprite = buttonSpriteEnabled;
        }
    }

}