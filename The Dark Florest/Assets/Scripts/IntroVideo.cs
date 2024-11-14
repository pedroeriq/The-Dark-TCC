using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // Associa o fim do vídeo ao método OnVideoEnd
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("FINAL"); // Substitua "PrimeiraFase" pelo nome da sua cena inicial
    }
}
