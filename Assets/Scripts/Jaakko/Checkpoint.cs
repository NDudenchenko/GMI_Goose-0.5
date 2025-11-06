using System;
using UnityEngine;

namespace AG3958
{
    public class Checkpoint : MonoBehaviour, IComparable<Checkpoint>
    {
        [SerializeField] private string _checkpointID;
        public string CheckpointID { get { return _checkpointID; } }
        private Vector2 _checkpointTarget;
        public Vector2 CheckpointTarget { get { return _checkpointTarget; } }

        private PlayerCore _playerCore;

        private void Start()
        {
            _checkpointTarget = transform.position;
            _playerCore = FindFirstObjectByType<PlayerCore>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _playerCore.PreviousCheckpoint != this)
            {
                _playerCore.SetCheckpoint(this);
            }
        }

        public int CompareTo(Checkpoint other)
        {
            return string.Compare(this._checkpointID, other._checkpointID);
        }
    }

}