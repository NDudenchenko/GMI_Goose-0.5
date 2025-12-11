using System;
using UnityEngine;

namespace AG3958
{
    public class GameStateManager : MonoBehaviour
    {
        public static event Action WinGameEvent;

        [SerializeField] private Enemy _targetEnemy;
        private bool _winInvoked;

        // in full production version a lot more stuff, passthroughs to dialogue system for flow control through Ink

        private void Awake()
        {
            if (_targetEnemy == null) { Destroy(gameObject); }
        }

        private void Update()
        {
            if (!_winInvoked && (_targetEnemy == null || _targetEnemy.IsDying)) 
            { 
                WinGameEvent?.Invoke(); 
                _winInvoked = true;
            }
        }

        private void OnDestroy()
        {
            WinGameEvent = null;
        }
    } 
}
