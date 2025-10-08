using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SignPuzzlePart : MonoBehaviour
{
    [SerializeField]
    private KeyCode interactKey = KeyCode.N;
    
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private List<Sprite> partSprites = new List<Sprite>();

    [SerializeField] 
    private int correctPartSpriteIndex;
    
    [SerializeField] 
    private Sprite correctPartSprite;
    
    // Return true is correct sprite and false is not
    public event Action<bool, SignPuzzlePart> OnSpriteChangedStatus;
    
    private int _currentPartSpriteIndex;

    private void Awake()
    {
        _currentPartSpriteIndex = 0;
        spriteRenderer.sprite = partSprites[_currentPartSpriteIndex];
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            ChangeSprite();
        }
    }

    public void ChangeSprite()
    {
        _currentPartSpriteIndex = _currentPartSpriteIndex == partSprites.Count - 1 ? _currentPartSpriteIndex = 0 : ++_currentPartSpriteIndex;
        spriteRenderer.sprite = partSprites[_currentPartSpriteIndex];
        
        CheckIsNewSpriteCorrect(_currentPartSpriteIndex);
    }

    private bool CheckIsNewSpriteCorrect(int newSpriteIndex)
    {
        //bool result = newSpriteIndex == correctPartSpriteIndex ? true : false;
        bool result = spriteRenderer.sprite == correctPartSprite ? true : false;
        OnSpriteChangedStatus?.Invoke(result, this);
        
        return result;
    }
}
