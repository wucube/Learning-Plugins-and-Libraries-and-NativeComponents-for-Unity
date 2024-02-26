using System;
using UnityEngine;

[Serializable]
public class Runner
{
    public Transform Target;
    public float Speed = 5f;
    public Vector3 StartPos;
    
    /// <summary>
    /// 达到目标
    /// </summary>
    public bool ReachGoal = false;

    public void Reset()
    {
        ReachGoal = false;
        Target.position = StartPos;
    }
}
