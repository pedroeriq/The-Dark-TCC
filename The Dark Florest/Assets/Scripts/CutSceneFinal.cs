using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutSceneFinal : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // Associa o fim do vídeo ao método OnVideoEnd
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("Menu"); // Substitua "FINAL" pela cena que você deseja carregar ao final do vídeo
    }
    
}