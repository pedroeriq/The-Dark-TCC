using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conversa : MonoBehaviour
{
    // Diálogos do Player e do NPC
    public string[] dialogoPlayer;
    public string[] dialogoNPC;
    public int dialogoIndex;

    public GameObject dialogoPainel;    // Painel de diálogo compartilhado

    // Player
    public Text dialogoTextPlayer;      // Texto de diálogo do Player
    public Text namePlayer;             // Nome do Player
    public Image imagePlayer;           // Imagem do Player
    public Sprite spritePlayer;

    // NPC
    public Text dialogoTextNPC;         // Texto de diálogo do NPC
    public Text nameNPC;                // Nome do NPC
    public Image imageNPC;              // Imagem do NPC
    public Sprite spriteNPC;

    public bool startDialogo;
    private bool isPlayerTurn = true;   // Variável para controlar de quem é a vez de falar

    // Start é chamado antes do primeiro frame
    void Start()
    {
        dialogoPainel.SetActive(false);
        // Inicializa os elementos como invisíveis
        namePlayer.gameObject.SetActive(false);
        imagePlayer.gameObject.SetActive(false);
        dialogoTextPlayer.text = "";

        nameNPC.gameObject.SetActive(false);
        imageNPC.gameObject.SetActive(false);
        dialogoTextNPC.text = "";
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        if (startDialogo && GetCurrentText().text == GetCurrentDialog())
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ProximoDialogo();
            }
        }
    }

    // Exibe o próximo diálogo alternando entre Player e NPC
    void ProximoDialogo()
    {
        // Alterna a vez de falar antes de incrementar o índice
        if (isPlayerTurn)
        {
            isPlayerTurn = false; // Próxima fala será do NPC
        }
        else
        {
            isPlayerTurn = true; // Próxima fala será do Player
            dialogoIndex++; // Incrementa o índice após a fala do NPC
        }

        // Verifica se o índice está dentro dos limites dos diálogos
        if (dialogoIndex < dialogoPlayer.Length + dialogoNPC.Length)
        {
            StartCoroutine(MostrarDialogo());
        }
        else
        {
            dialogoPainel.SetActive(false);
            startDialogo = false;
            dialogoIndex = 0;

            var player = FindObjectOfType<Player>();
            player.SetMove(true); // Permite o movimento
        }
    }

    // Inicia o diálogo
    void StartDialogo()
    {
        dialogoIndex = 0;
        startDialogo = true;
        dialogoPainel.SetActive(true);
        // Exibe os elementos do Player e oculta os do NPC no início
        MostrarElementosPlayer();
        StartCoroutine(MostrarDialogo());
    }

    // Mostra o diálogo letra por letra
    IEnumerator MostrarDialogo()
    {
        Text currentText = GetCurrentText(); // Pega o texto atual (Player ou NPC)
        currentText.text = ""; // Limpa o texto atual

        // Mostra o nome e a imagem do falante atual
        if (isPlayerTurn)
        {
            MostrarElementosPlayer();
        }
        else
        {
            MostrarElementosNPC();
        }

        // Mostra o diálogo letra por letra
        foreach (char letter in GetCurrentDialog())
        {
            currentText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Alterna entre a fala do Player e do NPC
    string GetCurrentDialog()
    {
        if (isPlayerTurn)
        {
            return dialogoPlayer[dialogoIndex / 2]; // Player fala nos índices pares
        }
        else
        {
            return dialogoNPC[dialogoIndex / 2]; // NPC fala nos índices ímpares
        }
    }

    // Mostra os elementos do Player
    void MostrarElementosPlayer()
    {
        namePlayer.text = "Elias"; // Nome do Player
        imagePlayer.sprite = spritePlayer; // Imagem do Player
        namePlayer.gameObject.SetActive(true); // Mostra o nome do Player
        imagePlayer.gameObject.SetActive(true); // Mostra a imagem do Player
        dialogoTextPlayer.text = ""; // Limpa o texto do diálogo do Player
        dialogoTextNPC.text = ""; // Limpa o texto do diálogo do NPC (caso esteja visível)
        nameNPC.gameObject.SetActive(false); // Oculta o nome do NPC
        imageNPC.gameObject.SetActive(false); // Oculta a imagem do NPC
    }

    // Mostra os elementos do NPC
    void MostrarElementosNPC()
    {
        nameNPC.text = "Yan"; // Nome do NPC
        imageNPC.sprite = spriteNPC; // Imagem do NPC
        nameNPC.gameObject.SetActive(true); // Mostra o nome do NPC
        imageNPC.gameObject.SetActive(true); // Mostra a imagem do NPC
        dialogoTextNPC.text = ""; // Limpa o texto do diálogo do NPC
        dialogoTextPlayer.text = ""; // Limpa o texto do diálogo do Player (caso esteja visível)
        namePlayer.gameObject.SetActive(false); // Oculta o nome do Player
        imagePlayer.gameObject.SetActive(false); // Oculta a imagem do Player
    }

    // Pega o Text atual, dependendo de quem está falando
    Text GetCurrentText()
    {
        return isPlayerTurn ? dialogoTextPlayer : dialogoTextNPC;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = FindObjectOfType<Player>();
            player.SetMove(false);      // Bloqueia o movimento
            player.Anim.SetInteger("transition", 0); // Idle
            StartDialogo();             // Inicia o diálogo
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            startDialogo = false;       // Para o diálogo quando sair do gatilho
        }
    }
}