using UnityEngine;
using UnityEngine.SceneManagement; // Para gerenciar cenas
using UnityEngine.UI; // Para UI

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Fase1"); // Coloque o nome da cena do seu jogo
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