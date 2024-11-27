using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Arraste o prefab do painel de pause aqui no Inspector
    public CartaUI cartaUI; // Referência ao componente CartaUI
    public GameObject[] cartaSprites; // Array de sprites das cartas

    void Start()
    {
        
        // Desativa todas as cartas no início
        foreach (var carta in cartaSprites)
        {
            carta.SetActive(false);
        }

        // Verifica se alguma carta já foi coletada e ativa as sprites correspondentes
        foreach (var carta in cartaSprites)
        {
            if (carta.activeSelf) // Se já foi coletada e ativada, deixa visível
            {
                carta.SetActive(true);
            }
        }
    }

    public void ShowPauseMenu()
    {
        pauseMenuUI.SetActive(true); // Ativa o painel de pause
        Time.timeScale = 0f; // Para o tempo
    }

    public void HidePauseMenu()
    {
        pauseMenuUI.SetActive(false); // Desativa o painel de pause
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCarta(int cartaIndex)
    {
        cartaUI.ShowCarta(cartaIndex); // Exibe o painel específico da carta
        HidePauseMenu(); // Oculta o menu de pausa
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f; // Restaura o tempo
        pauseMenuUI.SetActive(false); // Desativa o menu de pausa
        
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }

    public void EnableCartaSprite(int cartaIndex)
    {
        if (cartaIndex > 0 && cartaIndex < cartaSprites.Length)
        {
            cartaSprites[cartaIndex].SetActive(true); // Ativa a sprite da carta específica
        }
    }
}
