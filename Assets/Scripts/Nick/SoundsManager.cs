using UnityEngine;

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
