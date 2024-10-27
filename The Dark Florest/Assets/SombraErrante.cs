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
    public float tempoDePausa = 0.3f;  // Tempo de pausa após levar dano

    private Animator animator;  // Referência ao Animator
    private bool estaEmHit = false; // Flag para verificar se o inimigo está na animação de hit
    private float velocidadeOriginal; // Armazena a velocidade original

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
        // Aguarda o tempo da animação de surgimento
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        // Permite que o inimigo comece a seguir o player
        podeSeguir = true;
    }

    private void Seguir()
    {
        Vector2 direcao = (playerPos.position - transform.position).normalized;

        if (direcao.magnitude > 0.1f)
        {
            // Chama a animação de corrida (transition 1) se estiver se movendo
            animator.SetInteger("transition", 1);
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
        }
        else
        {
            // Chama a animação de idle (transition 0) se estiver parado
            animator.SetInteger("transition", 0);
        }

        // Altera o flip do sprite com base na direção do movimento
        if (direcao.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void ReceberDano(int dano)
    {
        if (podeReceberDano)
        {
            vida -= dano;
            Debug.Log($"Sombra recebeu {dano} de dano. Vida restante: {vida}");

            // Chama a animação de hit
            animator.SetTrigger("Hit");

            // Interrompe a movimentação e zera a velocidade temporariamente
            StartCoroutine(PausaMovimentoTemporaria());
        }
    }

    private IEnumerator PausaMovimentoTemporaria()
    {
        estaEmHit = true;

        // Zera a velocidade enquanto estiver em hit
        speed = 0;

        // Espera o tempo configurável antes de retornar a velocidade original
        yield return new WaitForSeconds(tempoDePausa);

        // Restaura a velocidade original e permite seguir novamente
        speed = velocidadeOriginal;
        estaEmHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            ReceberDano(danoNormal);
        }
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            ReceberDano(danoEspecial);
        }
    }

    private void DestruirSombra()
    {
        Debug.Log("Sombra destruída!");
        Destroy(gameObject);
    }
}
