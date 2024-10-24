using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public Transform pointA; // Ponto A
    public Transform pointB; // Ponto B
    public float speed = 2f; // Velocidade do inimigo
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

        if (PlayerInRange())
        {
            isChasingPlayer = true;
        }
        else
        {
            isChasingPlayer = false;
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
        animator.SetInteger("transition", 1);
        Vector3 directionToPlayer = player.position - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

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
        return player.position.x > pointA.position.x && player.position.x < pointB.position.x
            && Vector2.Distance(transform.position, player.position) <= detectionRange;
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
            animator.SetTrigger("attack");
            speed = 0;

            if (player != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                player.TakeDamage(danoPlayer, knockbackDirection);
            }

            yield return new WaitForSeconds(attackCooldown); // Espera o tempo de cooldown entre ataques
        }
    }

    private IEnumerator RestoreSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speed = 2f;
    }

    private void DestruirInimigo()
    {
        Debug.Log("Inimigo destruído!");
        Destroy(gameObject, 2f);
    }
}
