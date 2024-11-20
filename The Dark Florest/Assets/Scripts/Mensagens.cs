using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Para acessar os componentes Image e UI

public class Mensagens : MonoBehaviour
{
    public GameObject painelMensagem;   // Painel que contém a imagem
    public Image mensagemImagem;        // Referência para a imagem da mensagem
    public Sprite imagem;               // Imagem que será exibida
    public float tempoExibicao = 2f;    // Tempo para a imagem ficar visível
    private bool mensagemExibida = false; // Para controlar se a mensagem já foi exibida

    // Start é chamado antes do primeiro frame
    void Start()
    {
        painelMensagem.SetActive(false);  // Inicializa o painel como invisível
    }

    // Método chamado quando o Player colide com o objeto
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !mensagemExibida)
        {
            // Exibe a imagem e o painel
            ExibirImagem();
        }
    }

    // Exibe a imagem por um tempo determinado
    void ExibirImagem()
    {
        mensagemExibida = true;                   // Marca como exibida
        mensagemImagem.sprite = imagem;          // Define a imagem a ser exibida
        painelMensagem.SetActive(true);          // Torna o painel de imagem visível
        StartCoroutine(OcultarMensagem());       // Começa a ocultar após o tempo
    }

    // Corrotina para ocultar a imagem e o painel após o tempo
    IEnumerator OcultarMensagem()
    {
        yield return new WaitForSeconds(tempoExibicao); // Espera o tempo de exibição
        painelMensagem.SetActive(false);               // Oculta o painel de mensagem
        Destroy(gameObject);                           // Destroi o objeto da mensagem
    }
}