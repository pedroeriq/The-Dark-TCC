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
        SceneManager.LoadScene("Introdução"); // Carrega a cena de introdução
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

    // Função chamada quando o jogador morre, que carrega a tela de Game Over
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver"); // Coloque o nome da cena de Game Over
    }

    // Função chamada quando o jogador clica no botão "Continuar" no Game Over
    public void ContinueGame()
    {
        // Aqui chamamos o respawn do jogador no checkpoint salvo
        CheckpointManager.Instance.RespawnPlayer(GameObject.FindWithTag("Player"));
        // Volta para a cena do jogo
    }
}