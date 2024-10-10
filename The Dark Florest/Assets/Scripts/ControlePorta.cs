using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlePorta : MonoBehaviour
{
    public Sprite portaFechadaSprite;  // Sprite da porta fechada
    public Sprite portaAbertaSprite;   // Sprite da porta aberta

    private SpriteRenderer spriteRenderer;
    private bool portaAberta = false; // Estado da porta (fechada inicialmente)

    // A sequência correta de estados das alavancas (por exemplo: [1, 2, 3, 0])
    private int[] sequenciaCorreta = { 1, 2, 3, 0 };
    private int[] sequenciaJogador = new int[4]; // Sequência que o jogador ativou
    private int indiceAtualAlavanca = 0;         // Índice da alavanca sendo modificada

    // Referência às alavancas
    public LeverControl alavanca1;
    public LeverControl alavanca2;
    public LeverControl alavanca3;
    public LeverControl alavanca4;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = portaFechadaSprite; // Porta começa fechada
    }

    void Update()
    {
        if (!portaAberta)
        {
            // Verifica o estado das alavancas e atualiza a sequência do jogador
            sequenciaJogador[0] = alavanca1.GetLeverState();
            sequenciaJogador[1] = alavanca2.GetLeverState();
            sequenciaJogador[2] = alavanca3.GetLeverState();
            sequenciaJogador[3] = alavanca4.GetLeverState();

            // Checa se a sequência do jogador corresponde à sequência correta
            if (VerificarSequencia())
            {
                AbrirPorta(); // Abre a porta se a sequência estiver correta
            }
        }
    }

    bool VerificarSequencia()
    {
        // Verifica se todas as alavancas estão na posição correta
        for (int i = 0; i < sequenciaCorreta.Length; i++)
        {
            if (sequenciaJogador[i] != sequenciaCorreta[i])
            {
                return false;
            }
        }
        return true; // Sequência correta
    }

    void AbrirPorta()
    {
        portaAberta = true;
        spriteRenderer.sprite = portaAbertaSprite; // Exibe a sprite da porta aberta
        Debug.Log("Porta aberta!"); // Exibe uma mensagem no console
    }
}
