using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public PauseMenu pauseMenu; // Arraste o script PauseMenu aqui no Inspector
    public static GameController instance; // Singleton instance
    public TMP_Text moedaTXT; // Texto para mostrar as moedas na tela
    public GameObject playerPrefab; // Prefab do jogador para respawn
    public int playerLife = 3; // Vida do jogador
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
        if (Input.GetKeyDown(KeyCode.Space) && !isPaused) // Verifica se a tecla espaço foi pressionada e se o jogo não está pausado
        {
            PauseGame(); // Mostra o menu de pause
        }

        if (playerLife <= 0 && !isPlayerDead)
        {
            Die();
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

    private void Die()
    {
        if (isPlayerDead) return;
        isPlayerDead = true;

        // Movendo o jogador para o último checkpoint salvo
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        playerTransform.position = CheckpointManager.Instance.GetLastCheckpointPosition();

        // Restaurar a vida do jogador
        playerLife = 3;

        isPlayerDead = false;
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
