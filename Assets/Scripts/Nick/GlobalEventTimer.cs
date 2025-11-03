using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AH4063
{
    public class GlobalEventTimer : MonoBehaviour
    {
        public delegate void GlobalTimerCycle(); 
        public static event GlobalTimerCycle OnGlobalTimerCycle;
    
        [SerializeField]
        private float cycleFrequency = 3;
        private float _timer;
        
        void Start()
        {
        }
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= cycleFrequency)
            {
                _timer = 0f;
                OnGlobalTimerCycle?.Invoke();
            }
        }
    }
}
