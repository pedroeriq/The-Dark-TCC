using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObserver : MonoBehaviour
{
    public static event Action<Vector3> ParticleSpawEvent;

    public static void OnParticleSpawEvent(Vector3 obj)
    {
        ParticleSpawEvent?.Invoke(obj);
    }
}