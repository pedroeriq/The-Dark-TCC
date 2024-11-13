using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public PauseMenu pauseMenu; // Arraste o script PauseMenu aqui no Inspector
    public static GameController instance; // Singleton instance
    public TMP_Text moedaTXT; // Texto para mostrar as moedas na tela
    public GameObject playerPrefab; // Prefab do jogador para respawn
    public int moedas; // Contador de moedas

    private bool isPlayerDead = false; // Estado do jogador
    private bool isPaused = false; // Estado de pause

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o GameController entre cenas
        }
        else
        {
            Destroy(gameObject); // Garante que não haverá múltiplas instâncias
        }
    }

    void Start()
    {
        moedas = 0;
        UpdateMoedaTXT(); // Atualiza o texto de moedas na inicialização
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) // Verifica se a tecla espaço foi pressionada e se o jogo não está pausado
        {
            PauseGame(); // Mostra o menu de pause
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Para o tempo
        pauseMenu.pauseMenuUI.SetActive(true); // Ativa o painel de pause
        isPaused = true; // Define o estado como pausado
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Retorna o tempo à normalidade
        pauseMenu.pauseMenuUI.SetActive(false); // Desativa o painel de pause
        isPaused = false; // Define o estado como não pausado
    }

    public void AddMoeda()
    {
        moedas++;
        UpdateMoedaTXT();
    }

    private void UpdateMoedaTXT()
    {
        if (moedaTXT != null)
        {
            moedaTXT.text = "" + moedas.ToString();
        }
    }

    public void RespawnPlayer()
    {
        // Função para reaparecer o jogador em caso de morte
        Vector3 spawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    public void CoinCollected()
    {
        AddMoeda(); // Atualiza o contador de moedas
    }
}
