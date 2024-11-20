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
    private bool podeSeguir = false; // Controle para iniciar o movimento após surgir
    public int vida = 5;  // Vida inicial da Sombra Errante
    public int danoNormal = 1;  // Dano causado por bala normal
    public int danoEspecial = 2;  // Dano causado por bala especial
    public int danoPlayer = 10; // Dano causado ao jogador
    public float tempoDePausa = 0.3f;  // Tempo de pausa após levar dano
    public float attackCooldown = 1f; // Tempo de recarga entre ataques

    private Animator animator;  // Referência ao Animator
    private bool estaEmHit = false; // Flag para verificar se o inimigo está na animação de hit
    private bool estaAtacando = false; // Flag para controlar o cooldown de ataque
    private float velocidadeOriginal; // Armazena a velocidade original

    public Transform pontoAtaque;  // Referência ao GameObject do ponto de ataque

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();  // Inicializa o Animator
        spriteRenderer.enabled = false;  // Sombra começa invisível
        velocidadeOriginal = speed;  // Armazena a velocidade original
    }

    void Update()
    {
        distancia = Vector2.Distance(transform.position, playerPos.position);

        if (!sombraRevelada && distancia < campoDeVisao)
        {
            RevelarSombra();
        }

        // Só segue o player se a animação de hit não estiver ativa
        if (podeSeguir && !estaEmHit)
        {
            Seguir();
        }

        // Atualiza a rotação do ponto de ataque para ficar de frente para o jogador
        AtualizarPontoAtaque();

        if (vida <= 0)
        {
            DestruirSombra();
        }
    }

    private void RevelarSombra()
    {
        spriteRenderer.enabled = true;
        sombraRevelada = true;
        podeReceberDano = true;

        // Chama a animação de surgimento (transition 2)
        animator.SetInteger("transition", 2);

        // Espera a animação de surgimento terminar antes de seguir o player
        StartCoroutine(EsperarAnimacaoSurgimento());
    }

    private IEnumerator EsperarAnimacaoSurgimento()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        podeSeguir = true;
    }

    private void Seguir()
    {
        Vector2 direcao = (playerPos.position - transform.position).normalized;

        if (direcao.magnitude > 0.1f)
        {
            animator.SetInteger("transition", 1);
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
        }
        else
        {
            animator.SetInteger("transition", 0);
        }

        if (direcao.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void AtualizarPontoAtaque()
    {
        if (pontoAtaque != null)
        {
            // Faz o ponto de ataque rotacionar para ficar de frente para o jogador
            Vector2 direcaoParaPlayer = (playerPos.position - transform.position).normalized;
        
            // Calcula o ângulo em graus
            float angulo = Mathf.Atan2(direcaoParaPlayer.y, direcaoParaPlayer.x) * Mathf.Rad2Deg;

            // Ajusta a rotação para garantir que o ponto aponte corretamente
            pontoAtaque.rotation = Quaternion.Euler(new Vector3(0, 0, angulo - 90)); // Subtrai 90 graus se necessário
        }
    }

    public void ReceberDano(int dano)
    {
        if (podeReceberDano)
        {
            vida -= dano;
            Debug.Log($"Sombra recebeu {dano} de dano. Vida restante: {vida}");

            animator.SetTrigger("Hit");

            StartCoroutine(PausaMovimentoTemporaria());
        }
    }

    private IEnumerator PausaMovimentoTemporaria()
    {
        estaEmHit = true;
        speed = 0;
        yield return new WaitForSeconds(tempoDePausa);
        speed = velocidadeOriginal;
        estaEmHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            ReceberDano(danoNormal);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            ReceberDano(danoEspecial);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !estaAtacando)
        {
            StartCoroutine(AttackCoroutine(other.GetComponent<Player>()));
        }
    }

    private IEnumerator AttackCoroutine(Player player)
    {
        estaAtacando = true;
        animator.SetTrigger("Attack");

        if (player != null)
        {
            Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
            player.TakeDamage(danoPlayer, knockbackDirection);
        }

        yield return new WaitForSeconds(attackCooldown);
        estaAtacando = false;
    }

    private void DestruirSombra()
    {
        Debug.Log("Sombra destruída!");
        animator.SetInteger("transition", 5);
        StartCoroutine(DestruirAposMorte());
    }

    private IEnumerator DestruirAposMorte()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}