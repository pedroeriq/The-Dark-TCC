using UnityEngine;
using UnityEngine.SceneManagement; // Para gerenciar cenas
using UnityEngine.UI; // Para UI

public class MenuController : MonoBehaviour
{
    public void MenuScene()
    {
        SceneManager.LoadScene("Menu"); // Coloque o nome da cena do seu jogo
    }
    public void StartGame()
    {
        SceneManager.LoadScene("FINAL"); // Coloque o nome da cena do seu jogo
    }

    public void OpenOptions()
    {
        // Aqui você pode carregar a cena de opções ou abrir um painel
        Debug.Log("Abrindo opções...");
    }

    public void ExitGame()
    {
        Application.Quit(); // Sai do jogo
        Debug.Log("Saindo do jogo...");
    }
}