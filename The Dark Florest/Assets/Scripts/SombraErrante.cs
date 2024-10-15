using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SombraErrante : MonoBehaviour
{
    public float distancia;
    public float speed;
    public Transform playerPos;
    public Rigidbody2D flyRB;
    public float campoDeVisao = 4f;  // Distância em que a sombra fica visível
    private SpriteRenderer spriteRenderer;
    private bool sombraRevelada = false;  // Flag para verificar se a sombra já foi revelada
    private bool podeReceberDano = false;  // Flag para verificar se a sombra pode receber dano

    public int vida = 5;  // Vida inicial da Sombra Errante
    public int danoNormal = 1;  // Dano causado por bala normal
    public int danoEspecial = 2;  // Dano causado por bala especial

    // Start is called before the first frame update
    void Start()
    {
        // Inicializa o componente SpriteRenderer para controlar a visibilidade
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;  // Sombra começa invisível
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula a distância entre a sombra e o player
        distancia = Vector2.Distance(transform.position, playerPos.position);

        // Se a sombra ainda não foi revelada e o player está dentro do campo de visão
        if (!sombraRevelada && distancia < campoDeVisao)
        {
            RevelarSombra();
        }

        // Se a sombra foi revelada, ela segue o player
        if (sombraRevelada)
        {
            Seguir();  // Faz a sombra seguir o jogador
        }

        // Verifica se a vida acabou e destrói a sombra
        if (vida <= 0)
        {
            DestruirSombra();
        }
    }

    private void RevelarSombra()
    {
        spriteRenderer.enabled = true;  // Torna a sombra visível
        sombraRevelada = true;  // Marca que a sombra foi revelada
        podeReceberDano = true;  // Agora a sombra pode receber dano
    }

    private void Seguir()
    {
        // Movimento em direção ao player
        Vector2 direcao = (playerPos.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);

        // Altera o flip do sprite com base na direção do movimento
        if (direcao.x > 0)
        {
            spriteRenderer.flipX = false;  // Inimigo se movendo para a direita
        }
        else if (direcao.x < 0)
        {
            spriteRenderer.flipX = true;   // Inimigo se movendo para a esquerda
        }
    }

    // Função para aplicar dano à sombra
    public void ReceberDano(int dano)
    {
        // Verifica se a sombra está visível e pode receber dano
        if (podeReceberDano)
        {
            vida -= dano;  // Reduz a vida da sombra
            Debug.Log($"Sombra recebeu {dano} de dano. Vida restante: {vida}");
        }
    }

    // Lida com colisões
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Aplica dano baseado na tag do objeto colidido
        if (collision.gameObject.CompareTag("Bala"))
        {
            ReceberDano(danoNormal);  // Aplica o dano normal
        }
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            ReceberDano(danoEspecial);  // Aplica o dano especial (dano extra)
        }
    }

    // Destrói a sombra
    private void DestruirSombra()
    {
        Debug.Log("Sombra destruída!");
        Destroy(gameObject);  // Destrói o objeto da sombra
    }
}