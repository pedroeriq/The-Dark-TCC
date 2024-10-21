using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Arraste o prefab do painel de pause aqui no Inspector

    void Start()
    {
        // Garante que o menu de pause comece desativado
        pauseMenuUI.SetActive(false);
    }

    public void Resume()
    {
        GameController.instance.ResumeGame(); // Chama a função ResumeGame do GameController
    }

    public void ShowPauseMenu()
    {
        pauseMenuUI.SetActive(true); // Ativa o painel de pause
        Time.timeScale = 0f; // Para o tempo
    }

    public void ExitGame()
    {
        Application.Quit(); // Sai do jogo
        Debug.Log("Saindo do jogo...");
    }
}