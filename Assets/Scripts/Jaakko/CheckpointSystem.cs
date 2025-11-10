using System;
using UnityEngine;

namespace AG3958
{
    public class CheckpointSystem : MonoBehaviour
    {
        private Checkpoint[] _checkpointList;
        public Checkpoint[] CheckpointList { get { return _checkpointList; } }

        private PlayerCore _playerCore;

        private void Start()
        {
            _checkpointList = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            Array.Sort(_checkpointList);
            //foreach (Checkpoint checkpoint in _checkpointList) Debug.Log(checkpoint);
            _playerCore = FindFirstObjectByType<PlayerCore>();
        }


    }

}