using System;
using UnityEngine;

public static class CheckPointObserver
{
    public static event Action<Vector3> OnCheckpointTriggerEvent;
    public static void TriggerCheckpoint(Vector3 checkpointPosition)
    {
        OnCheckpointTriggerEvent?.Invoke(checkpointPosition);
    }
}