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

    void Update()
    {
        // Verifica se a tecla "E" foi pressionada e pula o vídeo
        if (Input.GetKeyDown(KeyCode.E))
        {
            SkipVideo(); // Chama o método para pular o vídeo
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("FINAL"); // Substitua "FINAL" pela cena que você deseja carregar ao final do vídeo
    }

    void SkipVideo()
    {
        videoPlayer.Stop(); // Interrompe o vídeo imediatamente
        SceneManager.LoadScene("FINAL"); // Substitua "FINAL" pela cena desejada
    }
}