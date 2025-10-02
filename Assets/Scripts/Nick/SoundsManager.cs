using UnityEngine;

namespace AH4063
{
    public class SoundsManager : MonoBehaviour
    {
        public AudioSource invisibleWallRevealedSound;
    
        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public void PlayInvisibleWallRevealedSound()
        {
            invisibleWallRevealedSound?.Play();
        }
    }
}
