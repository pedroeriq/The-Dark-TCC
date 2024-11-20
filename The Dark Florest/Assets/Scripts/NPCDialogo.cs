using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conversa : MonoBehaviour
{
    public string[] dialogoPlayer;
    public string[] dialogoNPC;
    public int dialogoIndex;

    public GameObject dialogoPainel;
    public Image painelImage; // Referência à Image do painel

    public Text dialogoTextPlayer;
    public Text namePlayer;

    public Text dialogoTextNPC;
    public Text nameNPC;

    public bool startDialogo;
    private bool isPlayerTurn = true;
    private bool isPlayerNearby = false; // Verifica se o jogador está próximo
    private bool skipTyping = false; // Flag para pular a animação de digitação

    void Start()
    {
        dialogoPainel.SetActive(false);
        namePlayer.gameObject.SetActive(false);
        dialogoTextPlayer.text = "";

        nameNPC.gameObject.SetActive(false);
        dialogoTextNPC.text = "";
    }

    void Update()
    {
        // Verifica se o jogador está próximo e pressionou a tecla E
        if (isPlayerNearby && !startDialogo && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogo();
        }

        // Controle do diálogo durante a conversa com E
        if (startDialogo)
        {
            if (GetCurrentText().text == GetCurrentDialog()) // Se o texto está completo
            {
                if (Input.GetKeyDown(KeyCode.E)) // Se pressionar E novamente
                {
                    ProximoDialogo();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E)) // Se pressionar E antes do texto terminar
            {
                skipTyping = true; // Habilita para pular a animação de digitação
                StopAllCoroutines(); // Interrompe qualquer Coroutine de digitação em andamento
                GetCurrentText().text = GetCurrentDialog(); // Exibe o texto completo imediatamente
            }
        }
    }

    void ProximoDialogo()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
        }
        else
        {
            isPlayerTurn = true;
            dialogoIndex++;
        }

        if (dialogoIndex < dialogoPlayer.Length && dialogoIndex < dialogoNPC.Length)
        {
            StartCoroutine(MostrarDialogo());
        }
        else
        {
            dialogoPainel.SetActive(false);
            startDialogo = false;
            dialogoIndex = 0;

            var player = FindObjectOfType<Player>();
            player.SetMove(true);
        }
    }

    void StartDialogo()
    {
        dialogoIndex = 0;
        startDialogo = true;
        dialogoPainel.SetActive(true);
        StartCoroutine(AnimarPainel()); // Inicia a animação de transição do painel
        MostrarElementosPlayer();
        StartCoroutine(MostrarDialogo());
    }

    IEnumerator AnimarPainel()
    {
        // A imagem começa com fillAmount = 0
        painelImage.fillAmount = 0;

        float tempoAnimacao = 1f; // Tempo para a animação, você pode ajustar conforme necessário
        float tempoPassado = 0f;

        // Anima a transição de 0 a 1
        while (tempoPassado < tempoAnimacao)
        {
            tempoPassado += Time.deltaTime;
            painelImage.fillAmount = Mathf.Lerp(0, 1, tempoPassado / tempoAnimacao); // Lerp para suavizar a transição
            yield return null;
        }

        painelImage.fillAmount = 1; // Garantir que o valor final seja exatamente 1
    }

    IEnumerator MostrarDialogo()
    {
        Text currentText = GetCurrentText();
        currentText.text = "";

        if (isPlayerTurn)
        {
            MostrarElementosPlayer();
        }
        else
        {
            MostrarElementosNPC();
        }

        string dialog = GetCurrentDialog();

        // Animação de digitação com a opção de pular
        foreach (char letter in dialog)
        {
            if (skipTyping) // Se for para pular a digitação
            {
                currentText.text = dialog; // Exibe o texto completo
                break;
            }
            currentText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        skipTyping = false; // Reseta a flag após exibir o texto completo
    }

    string GetCurrentDialog()
    {
        return isPlayerTurn ? dialogoPlayer[dialogoIndex] : dialogoNPC[dialogoIndex];
    }

    void MostrarElementosPlayer()
    {
        namePlayer.text = "Elias"; // Nome do player
        namePlayer.gameObject.SetActive(true);
        dialogoTextPlayer.text = "";
        dialogoTextNPC.text = "";
        nameNPC.gameObject.SetActive(false);
    }

    void MostrarElementosNPC()
    {
        nameNPC.text = "Yan"; // Nome do NPC
        nameNPC.gameObject.SetActive(true);
        dialogoTextNPC.text = "";
        dialogoTextPlayer.text = "";
        namePlayer.gameObject.SetActive(false);
    }

    Text GetCurrentText()
    {
        return isPlayerTurn ? dialogoTextPlayer : dialogoTextNPC;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true; // Marca que o jogador está próximo
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false; // Marca que o jogador saiu da área
            startDialogo = false;
        }
    }
}
