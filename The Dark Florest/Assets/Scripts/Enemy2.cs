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
    
    

    void Start()
    {
        target = pointA;
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        if (vida <= 0)
        {
            DestruirInimigo();
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
        // Aumenta a velocidade para perseguição
        animator.SetInteger("transition", 1);
        Vector3 directionToPlayer = player.position - transform.position;

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
        // Usar a velocidade de patrulha
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void UpdateFlip()
    {
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
        if (podeReceberDano)
        {
            vida -= dano;
            Debug.Log($"Inimigo recebeu {dano} de dano. Vida restante: {vida}");

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

    private void Die()
    {
        animator.SetTrigger("EnemyDead");
        TocarMusica T = null;
        T.GetComponent<TocarMusica>().Estartocando = false;

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

    private void DestruirInimigo()
    {
        Debug.Log("Inimigo destruído!");
        Destroy(gameObject, 2f);
    }
}
