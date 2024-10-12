using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // Referência para as 4 alavancas
    public LeverControl[] levers; // Array que conterá as alavancas
    public SpriteRenderer doorSpriteRenderer; // Referência ao SpriteRenderer da porta
    public Sprite doorOpen;   // Sprite da porta aberta
    public Sprite doorClosed; // Sprite da porta fechada

    // Padrão correto: por exemplo, Vermelho (0), Verde (1), Azul (2), Amarelo (3)
    public int[] correctPattern = { 0, 1, 2, 3 };

    void Start()
    {
        // Porta começa fechada
        doorSpriteRenderer.sprite = doorClosed;
    }

    void Update()
    {
        CheckPuzzle(); // Verifica o puzzle a cada frame
    }

    void CheckPuzzle()
    {
        bool isCorrect = true; // Supondo que o padrão esteja correto até que se prove o contrário

        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i].GetLeverState() != correctPattern[i])
            {
                isCorrect = false;
                break; // Sai do loop se algum estado não estiver correto
            }
        }

        if (isCorrect)
        {
            Debug.Log("Parabéns! Você acertou o padrão e a porta abriu.");
            doorSpriteRenderer.sprite = doorOpen; // Abre a porta
        }
        else
        {
            doorSpriteRenderer.sprite = doorClosed; // Mantém a porta fechada
        }
    }
}