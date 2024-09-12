using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverControl : MonoBehaviour
{
    public Sprite leverUp1;  // Primeira posição reta (inicial)
    public Sprite leverUp2;  // Segunda posição reta (para evitar conflito)
    public Sprite leverRight; // Sprite da alavanca para a direita
    public Sprite leverLeft;  // Sprite da alavanca para a esquerda
    
    public int GetLeverState()
    {
        return leverState; // Retorna o estado atual da alavanca
    }
    

    private SpriteRenderer spriteRenderer;
    private bool isNearLever = false; // Verifica se o player está perto da alavanca
    private int leverState = 0; // 0 = reta (inicial), 1 = direita, 2 = reta (após direita), 3 = esquerda

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = leverUp1; // Alavanca começa reta
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
        // Alterna entre os estados: reta (1) -> direita -> reta (2) -> esquerda -> reta (1)
        if (leverState == 0) 
        {
            leverState = 1; // Vai para a direita
            spriteRenderer.sprite = leverRight;
        }
        else if (leverState == 1) 
        {
            leverState = 2; // Volta para a posição reta (segunda versão)
            spriteRenderer.sprite = leverUp2;
        }
        else if (leverState == 2)
        {
            leverState = 3; // Vai para a esquerda
            spriteRenderer.sprite = leverLeft;
        }
        else if (leverState == 3)
        {
            leverState = 0; // Volta para a posição reta (primeira versão)
            spriteRenderer.sprite = leverUp1;
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