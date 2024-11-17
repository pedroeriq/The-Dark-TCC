using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Para acessar o componente Text e Image

public class MensagensColetavel : MonoBehaviour
{
    public GameObject painelMensagem;   // Painel que contém o texto
    public Text mensagemText;           // Referência para o texto da mensagem
    public string mensagem;             // Mensagem que será exibida
    public float tempoExibicao = 2f;    // Tempo para a mensagem ficar visível

    // Start é chamado antes do primeiro frame
    void Start()
    {
        painelMensagem.SetActive(false);  // Inicializa o painel como invisível
    }

    // Método chamado quando o Player colide com o objeto (item)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Exibe a mensagem e o painel ao coletar o item
            ExibirMensagem();

            // Destrói o item coletado
            Destroy(gameObject);
        }
    }

    // Exibe a mensagem por um tempo determinado
    void ExibirMensagem()
    {
        mensagemText.text = mensagem;               // Define o texto da mensagem
        painelMensagem.SetActive(true);             // Torna o painel de mensagem visível
        StartCoroutine(OcultarMensagem());          // Começa a ocultar após o tempo
    }

    // Corrotina para ocultar a mensagem e o painel após o tempo
    IEnumerator OcultarMensagem()
    {
        yield return new WaitForSeconds(tempoExibicao); // Espera o tempo de exibição
        painelMensagem.SetActive(false);               // Oculta o painel de mensagem
    }
}