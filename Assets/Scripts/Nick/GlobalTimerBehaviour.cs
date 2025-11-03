using AH4063;
using UnityEngine;

namespace AH4063
{
    public abstract class GlobalTimerBehaviour : MonoBehaviour
    {
        void Start()
        {
        
        }

        void Update()
        {
        
        }
    
        protected virtual void OnEnable()
        {
            GlobalEventTimer.OnGlobalTimerCycle += HandleCycleEvent;
        }

        protected virtual void OnDisable()
        {
            GlobalEventTimer.OnGlobalTimerCycle -= HandleCycleEvent;
        }
    
        private void HandleCycleEvent()
        {
            OnGlobalCycle();
        }

        protected abstract void OnGlobalCycle();
    }
}
