using System;
using UnityEngine;
using UnityEngine.Events;

public class InvisibleWallDetection : MonoBehaviour
{
    BoxCollider2D collision;

    [SerializeField]
    private UnityEvent OnWallRevealed;
    
    private void Awake()
    {
        collision = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            OnWallRevealed.Invoke();
            Destroy(this.gameObject);
        }
    }
}
