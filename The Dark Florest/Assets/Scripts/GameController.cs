using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int totalCartas; // Total de cartas no jogo
    private int cartasColetadas = 0; // Contador de cartas coletadas
    public GameObject gameOver; // Referência para o GameOver
    public PauseMenu pauseMenu; 
    public static GameController instance;
    public TMP_Text moedaTXT; 
    public GameObject playerPrefab; 
    public int moedas; 

    private bool isPlayerDead = false;
    private bool isPaused = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Inscreve o método OnSceneLoaded para quando a cena for carregada
    }

    void Start()
    {
        moedas = 0;
        UpdateMoedaTXT();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Corrigido
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            PauseGame(); 
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; 
        pauseMenu.pauseMenuUI.SetActive(true); 
        isPaused = true; 

        // Torna o cursor visível e desbloqueado ao pausar o jogo
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f; 
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.HidePauseMenu(); 
        }

        // Oculta e bloqueia o cursor ao retornar ao jog
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
        Vector3 spawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity); 
    }

    public void CoinCollected()
    {
        AddMoeda(); 
    }

    // Método chamado após a cena ser carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameOver == null)
        {
            gameOver = GameObject.Find("GameOver (1)"); // Reatribui o objeto GameOver caso não tenha sido atribuído
        }
    }


    // Lembre-se de desinscrever do evento quando não for mais necessário
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void ColetarCarta()
    {
        cartasColetadas++;
    }

    public bool TodasCartasColetadas()
    {
        return cartasColetadas >= totalCartas;
    }
}
