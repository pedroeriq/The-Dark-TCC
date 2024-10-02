using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource, sfxSource;
    public AudioClip clipPulo;

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

    private void OnEnable()
    {
        AudioObserver.PlayMusicEvent += TocarMusica;
        AudioObserver.StopMusicEvent += PararMusica;
        AudioObserver.PlaySfxEvent += TocarEfeitoSonoro;
    }

    private void OnDisable()
    {
        AudioObserver.PlayMusicEvent -= TocarMusica;
        AudioObserver.StopMusicEvent -= PararMusica;
        AudioObserver.PlaySfxEvent -= TocarEfeitoSonoro;
    }

    void TocarEfeitoSonoro(string nomeDoClip)
    {
        switch (nomeDoClip)
        {
            case "pulo":
                sfxSource.PlayOneShot(clipPulo);
                break;
            default:
                Debug.LogError($"Efeito sonoro {nomeDoClip} não encontrado");
                break;
        }  
    }

    void TocarMusica()
    {
        musicSource.Play();
    }

    void PararMusica()
    {
        musicSource.Stop();
    }
}