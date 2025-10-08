using System;
using System.Collections.Generic;
using UnityEngine;

namespace AH4063
{
    public class SignPuzzle : MonoBehaviour
    {
        public event Action OnPuzzleCompleted;
        
        [SerializeField]
        private List<SignPuzzlePart> puzzleParts;

        private Dictionary<SignPuzzlePart, bool> _puzzlePartsResults;

        private void Awake()
        {
            OnPuzzleCompleted += PuzzleCompleted;
        }

        void Start()
        {
            _puzzlePartsResults = new Dictionary<SignPuzzlePart, bool>();
            
            // Dictionary false result initialization
            foreach (SignPuzzlePart puzzlePart in puzzleParts)
            {
                puzzlePart.OnSpriteChangedStatus += CheckIsPuzzleCompleted;
                _puzzlePartsResults[puzzlePart] = false;
            }
        }

        void Update()
        {
        
        }

        void CheckIsPuzzleCompleted(bool partResult, SignPuzzlePart puzzlePart)
        {
            _puzzlePartsResults[puzzlePart] = true;
            bool result = true;
            
            foreach (bool partsResults in _puzzlePartsResults.Values)
            {
                if (!partsResults)
                    result = false;
            }
            
            if (result)
                OnPuzzleCompleted?.Invoke();
        }

        void PuzzleCompleted()
        {
            Debug.Log("Puzzle Completed");
        }
    }
}
