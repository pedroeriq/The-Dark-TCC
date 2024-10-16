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

    void Start()
    {
        // Definir o alvo inicial como o ponto A
        target = pointA;
        // Obter a referência ao Animator
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Verifica se o inimigo pode receber dano
        if (vida <= 0)
        {
            DestruirInimigo(); // Se a vida acabar, destrói o inimigo
            return; // Sair do Update se o inimigo estiver morto
        }

        // Verifica se o jogador está dentro da área entre A e B
        if (PlayerInRange())
        {
            isChasingPlayer = true; // Perseguir o jogador
        }
        else
        {
            isChasingPlayer = false; // Voltar ao comportamento normal
        }

        // Se o inimigo está perseguindo o jogador, ir em direção ao jogador
        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            PatrolBetweenPoints();
        }
    }

    // Função para patrulhar entre os pontos A e B
    private void PatrolBetweenPoints()
    {
        // Calcular a distância até o alvo
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Verifica se o inimigo chegou ao alvo
        if (distanceToTarget < 0.1f)
        {
            // Trocar o alvo quando chegar
            target = target == pointA ? pointB : pointA;
        }
        else
        {
            // Inimigo está se movendo, definir animação 'run'
            animator.SetInteger("transition", 1); // Transition para 'run'

            // Mover o inimigo na direção do alvo atual
            MoveTowardsTarget();

            // Atualizar o flip dependendo da direção do movimento
            UpdateFlip();
        }
    }

    // Função para mover o inimigo em direção ao jogador
    private void ChasePlayer()
    {
        // Definir a animação de movimento 'run'
        animator.SetInteger("transition", 1); // Transition para 'run'

        // Mover em direção ao jogador
        Vector3 directionToPlayer = player.position - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // Atualizar o flip dependendo da direção
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
            // Flip ativado quando indo para o ponto A
            Flip();
        }
        else if (target == pointB && isFlipped)
        {
            // Flip desativado quando indo para o ponto B
            Flip();
        }
    }

    private void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Inverter o eixo X para flipar o sprite
        transform.localScale = localScale;
    }

    private bool PlayerInRange()
    {
        // Verifica se o jogador está dentro da área entre os pontos A e B
        return player.position.x > pointA.position.x && player.position.x < pointB.position.x
            && Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    public void ReceberDano(int dano)
    {
        // Verifica se o inimigo pode receber dano
        if (podeReceberDano)
        {
            vida -= dano; // Reduz a vida do inimigo
            Debug.Log($"Inimigo recebeu {dano} de dano. Vida restante: {vida}");

            // Se a vida chega a zero, morrer
            if (vida <= 0)
            {
                vida = 0; // Garantir que a vida não fique negativa
                Die(); // Executa a lógica de morte
            }
            else
            {
                animator.SetTrigger("hit"); // Trigger a animação de hit
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("EnemyDead"); // Trigger a animação de morte
        // Desativar o inimigo ou executar outras lógicas de morte
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Aplica dano baseado na tag do objeto colidido
        if (collision.gameObject.CompareTag("Bala"))
        {
            ReceberDano(danoNormal);  // Aplica o dano normal
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            ReceberDano(danoEspecial);  // Aplica o dano especial (dano extra)
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto colidido é o jogador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Colisão com o jogador detectada!"); // Log adicionado
            // Executar a animação de ataque
            animator.SetTrigger("attack"); // Transition para 'attack'
            speed = 0; // Zerar a velocidade do inimigo ao atacar
            Debug.Log("Inimigo atacou o jogador!");

            // Causa dano ao jogador
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(danoPlayer); // Aplica o dano normal ao jogador
            }

            // Reiniciar a velocidade após 0.4 segundos
            StartCoroutine(RestoreSpeedAfterDelay(0.4f)); // Ajuste o tempo conforme necessário
        }
    }

    private IEnumerator RestoreSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speed = 2f; // Restaura a velocidade original do inimigo
    }

    private void DestruirInimigo()
    {
        Debug.Log("Inimigo destruído!");
        Destroy(gameObject, 2f); // Destrói o objeto do inimigo
    }
}