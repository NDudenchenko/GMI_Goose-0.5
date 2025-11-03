using AH4063;
using UnityEngine;

public class TestTimerObject : GlobalTimerBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected override void OnGlobalCycle()
    {
        Debug.Log($"Timer ticked at {Time.time}");
    }
}
