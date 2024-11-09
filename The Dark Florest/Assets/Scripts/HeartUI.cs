using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    public Sprite heartFull;   // Sprite do coração cheio
    public Sprite heartHalf;   // Sprite do coração pela metade
    public Sprite heartEmpty;  // Sprite do coração vazio

    public Image[] hearts;     // Array para os três corações

    public void UpdateHearts(int playerLife)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (playerLife > (i * 2) + 1)         // Vida cheia para o coração
                hearts[i].sprite = heartFull;
            else if (playerLife == (i * 2) + 1)   // Vida pela metade
                hearts[i].sprite = heartHalf;
            else                                  // Vida vazia
                hearts[i].sprite = heartEmpty;
        }
    }
}