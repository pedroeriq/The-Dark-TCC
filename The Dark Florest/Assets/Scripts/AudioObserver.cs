using System;
using UnityEngine;

public class AudioObserver : MonoBehaviour
{
    public static AudioObserver instance;

    public static event Action PlayMusicEvent;
    public static event Action StopMusicEvent;
    public static event Action<string> PlaySfxEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void TriggerPlayMusic()
    {
        PlayMusicEvent?.Invoke();
    }

    public static void TriggerStopMusic()
    {
        StopMusicEvent?.Invoke();
    }

    public static void TriggerPlaySfx(string nomeDoClip)
    {
        PlaySfxEvent?.Invoke(nomeDoClip);
    }
}