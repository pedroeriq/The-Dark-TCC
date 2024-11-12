using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public Transform pointA; // Ponto A
    public Transform pointB; // Ponto B
    public float speed = 2f; // Velocidade do inimigo em patrulha
    public float chaseSpeed = 4f; // Velocidade do inimigo ao perseguir o jogador
    public float detectionRange = 5f; // Distância de detecção do jogador
    public Transform player; // Referência ao jogador
    private Animator animator; // Referência ao Animator
    private bool isFlipped = false; // Estado do flip

    public int vida = 5; // Vida inicial do inimigo
    public int danoNormal = 1; // Dano causado por bala normal
    public int danoEspecial = 2; // Dano causado por bala especial
    public int danoPlayer = 10;

    private Transform target; // Alvo atual (usado para patrulha)
    private bool podeReceberDano = true; // Flag para verificar se o inimigo pode receber dano
    private bool isChasingPlayer = false; // Flag para verificar se o inimigo está perseguindo o jogador

    private Coroutine attackCoroutine; // Referência à coroutine de ataque
    public float attackCooldown = 1f; // Tempo de recarga entre ataques
    
    private Rigidbody2D rb2d; // Referência ao Rigidbody2D
    [SerializeField] private AudioSource Musica;
    [SerializeField] private AudioSource Grito;
    public bool tocar = true;

    private bool isDead = false; // Flag para verificar se o inimigo está morto

    // Nova variável pública para ajustar a força do knockback
    public float knockbackForce = 5f; // Força do knockback (ajustável no Inspector)
    public float knockbackDuration = 0.2f; // Duração do efeito de knockback

    void Start()
    {
        target = pointA;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>(); // Obtenha a referência ao Rigidbody2D
    }

    void Update()
    {
        if (isDead) return; // Se o inimigo estiver morto, não faz mais nada

        if (vida <= 0)
        {
            Die();
            return;
        }

        // Ativa perseguição ao jogador para sempre ao entrar no campo de visão
        if (PlayerInRange())
        {
            isChasingPlayer = true;
        }

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            PatrolBetweenPoints();
        }
    }

    private void PatrolBetweenPoints()
    {
        if (isDead) return; // Se o inimigo estiver morto, não faz mais nada

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < 0.1f)
        {
            target = target == pointA ? pointB : pointA;
        }
        else
        {
            animator.SetInteger("transition", 1);
            MoveTowardsTarget();
            UpdateFlip();
        }
    }

    private void ChasePlayer()
    {
        if (isDead) return; // Se o inimigo estiver morto, não faz mais nada

        // Aumenta a velocidade para perseguição
        animator.SetInteger("transition", 1);
        Vector3 directionToPlayer = player.position - transform.position;

        // Toca a música de perseguição
        if (tocar && !Musica.isPlaying) // Verifica se a música não está tocando
        {
            Grito.Play();   // Toca o grito
            Musica.Play();  // Toca a música
            tocar = false;  // Para não tocar a música repetidamente
        }

        // Usar a velocidade de perseguição
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        
        if (directionToPlayer.x < 0 && !isFlipped)
        {
            Flip();
        }
        else if (directionToPlayer.x > 0 && isFlipped)
        {
            Flip();
        }
    }

    private void MoveTowardsTarget()
    {
        if (isDead) return; // Se o inimigo estiver morto, não faz mais nada

        // Usar a velocidade de patrulha
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void UpdateFlip()
    {
        if (isDead) return; // Se o inimigo estiver morto, não faz mais nada

        if (target == pointA && !isFlipped)
        {
            Flip();
        }
        else if (target == pointB && isFlipped)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool PlayerInRange()
    {
        // Verifica se o jogador está no campo de visão e dentro do alcance de detecção
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    public void ReceberDano(int dano)
    {
        if (podeReceberDano && !isDead)
        {
            vida -= dano;
            Debug.Log($"Inimigo recebeu {dano} de dano. Vida restante: {vida}");

            // Aplica knockback se o inimigo não estiver morto
            if (vida > 0)
            {
                // Calcula a direção do knockback baseada na posição do jogador
                Vector2 knockbackDirection = (transform.position - player.position).normalized;

                // Aplica a força de knockback (ajuste a força para um valor adequado)
                rb2d.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse); // Adiciona a força de impulso

                // Pausa o movimento do inimigo por um breve momento
                StartCoroutine(HandleKnockback());
            }

            if (vida <= 0)
            {
                vida = 0;
                Die();
            }
            else
            {
                animator.SetTrigger("hit");
            }
        }
    }

    private IEnumerator HandleKnockback()
    {
        // Pausa o movimento do inimigo por um tempo curto (durante o knockback)
        float originalSpeed = speed;
        speed = 0f; // Para o inimigo de se mover enquanto aplica o knockback

        // Duração do efeito de knockback
        yield return new WaitForSeconds(knockbackDuration);

        // Retorna à velocidade de perseguição original
        speed = originalSpeed;
    }

    private void Die()
    {
        if (isDead) return; // Se já estiver morto, não faz nada
        
        Musica.Stop();
        Grito.Stop();

        isDead = true; // Marca o inimigo como morto
        animator.SetTrigger("EnemyDead");

        // Desativa a física para parar o movimento
        rb2d.velocity = Vector2.zero; // Para qualquer movimento em andamento
        rb2d.isKinematic = true; // Torna o Rigidbody2D cinemático para parar de ser afetado pela física

        // Destrói o inimigo após a animação de morte (2 segundos de delay para a animação terminar)
        Destroy(gameObject, 1.5f); // O inimigo será destruído após 2 segundos
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
        if (isDead) return; // Se o inimigo já estiver morto, não realiza ataques

        if (other.CompareTag("Player"))
        {
            if (attackCoroutine == null) // Iniciar a coroutine de ataque se já não estiver em execução
            {
                attackCoroutine = StartCoroutine(AttackCoroutine(other.GetComponent<Player>()));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (attackCoroutine != null) // Parar a coroutine de ataque quando o jogador sair da colisão
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator AttackCoroutine(Player player)
    {
        while (true)
        {
            // Toca a animação de ataque
            animator.SetTrigger("attack");

            // Aplica o dano ao jogador, se ele ainda estiver lá
            if (player != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                player.TakeDamage(danoPlayer, knockbackDirection);
            }

            // Aguarda o tempo da animação de ataque para continuar
            yield return new WaitForSeconds(attackCooldown);

            // Após o cooldown, restaura a velocidade original do inimigo
            speed = 2f; // Garantir que a velocidade seja restaurada
        }
    }
}
