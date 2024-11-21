using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Para acessar os componentes Image e UI

public class Mensagem1 : MonoBehaviour
{
    public GameObject painelMensagem;   // Painel que contém a imagem
    public Image mensagemImagem;        // Referência para a imagem da mensagem
    public Sprite imagem;               // Imagem que será exibida
    private bool mensagemExibida = false; // Para controlar se a mensagem já foi exibida

    private Player player;  // Referência ao script do Player

    // Start é chamado antes do primeiro frame
    void Start()
    {
        painelMensagem.SetActive(false);  // Inicializa o painel como invisível
        player = FindObjectOfType<Player>();  // Obtém a referência do Player
    }

    // Método chamado quando o Player colide com o objeto
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !mensagemExibida)
        {
            // Exibe a imagem e o painel
            ExibirImagem();

            // Desativa o movimento do Player
            if (player != null)
            {
                player.Anim.SetInteger("transition", 0);
                player.SetMove(false);  // Método para desativar o movimento
            }
        }
    }

    // Exibe a imagem
    void ExibirImagem()
    {
        mensagemExibida = true;                   // Marca como exibida
        mensagemImagem.sprite = imagem;          // Define a imagem a ser exibida
        painelMensagem.SetActive(true);          // Torna o painel de imagem visível
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        if (mensagemExibida && Input.GetKeyDown(KeyCode.Escape))  // Verifica se o Esc foi pressionado
        {
            OcultarMensagem();  // Oculta a mensagem quando o jogador apertar Esc

            // Reativa o movimento do Player
            if (player != null)
            {
                player.SetMove(true);  // Método para reativar o movimento
            }
        }
    }

    // Oculta a imagem e o painel
    void OcultarMensagem()
    {
        painelMensagem.SetActive(false);               // Oculta o painel de mensagem
        Destroy(gameObject);                           // Destroi o objeto da mensagem
    }
}
