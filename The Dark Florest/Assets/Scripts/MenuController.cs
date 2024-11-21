using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para gerenciar cenas

public class MenuController : MonoBehaviour
{
    private const string FirstPlayKey = "FirstPlay"; // Chave para identificar se é a primeira vez jogando

    public void MenuScene()
    {
        SceneManager.LoadScene("Menu"); // Coloque o nome da cena do seu menu
    }

    public void StartGame()
    {
        // Verifica se é a primeira vez jogando
        if (PlayerPrefs.GetInt(FirstPlayKey, 1) == 1) // O valor padrão é 1 (primeira vez)
        {
            SceneManager.LoadScene("Introdução"); // Carrega a cena de introdução
            PlayerPrefs.SetInt(FirstPlayKey, 0);  // Define que o jogo já foi iniciado
        }
        else
        {
            SceneManager.LoadScene("FINAL"); // Carrega diretamente a cena do jogo
        }
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Opções"); // Carrega a cena de opções
    }

    public void ExitGame()
    {
        Application.Quit(); // Sai do jogo
        Debug.Log("Saindo do jogo...");
    }
}