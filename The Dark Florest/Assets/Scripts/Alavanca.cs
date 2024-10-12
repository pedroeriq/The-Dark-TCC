using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverControl : MonoBehaviour
{
    public Sprite redLever;    // Sprite da alavanca na cor vermelha
    public Sprite greenLever;  // Sprite da alavanca na cor verde
    public Sprite blueLever;   // Sprite da alavanca na cor azul
    public Sprite yellowLever; // Sprite da alavanca na cor amarela

    public int GetLeverState()
    {
        return leverState; // Retorna o estado atual da alavanca
    }

    private SpriteRenderer spriteRenderer;
    private bool isNearLever = false; // Verifica se o player está perto da alavanca
    private int leverState = 0; // 0 = Vermelho, 1 = Verde, 2 = Azul, 3 = Amarelo

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = redLever; // Alavanca começa vermelha
    }

    void Update()
    {
        if (isNearLever && Input.GetKeyDown(KeyCode.DownArrow)) // Detecta seta para baixo
        {
            ToggleLever(); // Alterna o estado da alavanca
        }
    }

    void ToggleLever()
    {
        // Alterna entre os estados: Vermelho -> Verde -> Azul -> Amarelo -> Vermelho
        if (leverState == 0)
        {
            leverState = 1; // Vai para Verde
            spriteRenderer.sprite = greenLever;
        }
        else if (leverState == 1)
        {
            leverState = 2; // Vai para Azul
            spriteRenderer.sprite = blueLever;
        }
        else if (leverState == 2)
        {
            leverState = 3; // Vai para Amarelo
            spriteRenderer.sprite = yellowLever;
        }
        else if (leverState == 3)
        {
            leverState = 0; // Volta para Vermelho
            spriteRenderer.sprite = redLever;
        }
    }

    // Detecta se o player entrou na área de colisão da alavanca
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearLever = true;
        }
    }

    // Detecta se o player saiu da área de colisão da alavanca
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearLever = false;
        }
    }
}