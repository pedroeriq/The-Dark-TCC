using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // Referência para as 4 alavancas
    public LeverControl[] levers; // Array que conterá as alavancas

    // Padrão correto: aqui, por exemplo, o padrão é Direita (1), Reta (0), Esquerda (-1), Direita (1)
    public int[] correctPattern = { 1, 1, 1, 1 };

    void Update()
    {
        CheckPuzzle(); // Verifica o puzzle a cada frame
    }

    void CheckPuzzle()
    {
        bool isCorrect = true; // Supondo que o padrão esteja correto até que se prove o contrário

        for (int i = 0; i < levers.Length; i++)
        {
            //Debug.Log("Estado da alavanca " + (i + 1) + ": " +
                      //levers[i].GetLeverState()); // Adiciona logs do estado das alavancas

            //if (levers[i].GetLeverState() != correctPattern[i])
            {
                isCorrect = false;
                break; // Sai do loop se algum estado não estiver correto
            }
        }

        if (isCorrect)
        {
            Debug.Log("Parabéns! Você acertou o padrão e a porta abriu.");
        }
        else
        {
            Debug.Log("O padrão ainda está errado.");
        }
    }
}