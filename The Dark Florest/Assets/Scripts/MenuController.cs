using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necessário para manipular os botões

public class MenuController : MonoBehaviour
{
    public GameObject videoPanel; // Painel onde o VideoPlayer está localizado
    public Button[] menuButtons; // Array para armazenar os botões do menu (Opções, Sair, etc.)

    private void Start()
    {
        Cursor.visible = true;
    }

    public void StartGame()
    {
        if (videoPanel != null)
        {
            videoPanel.SetActive(true); // Ativa o painel do vídeo
            DisableMenuButtons(); // Desativa os botões do menu
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
        }
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Opções");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void ContinueGame()
    {
        CheckpointManager.Instance.RespawnPlayer(GameObject.FindWithTag("Player"));
    }

    private void DisableMenuButtons()
    {
        foreach (Button button in menuButtons)
        {
            button.interactable = false; // Desativa a interação com os botões
        }
    }

    private void EnableMenuButtons()
    {
        foreach (Button button in menuButtons)
        {
            button.interactable = true; // Reativa os botões, se necessário no futuro
        }
    }
}