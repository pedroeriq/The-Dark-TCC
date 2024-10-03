using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public Transform pointA; // Ponto A
    public Transform pointB; // Ponto B
    public float speed = 2f; // Velocidade do inimigo
    private Animator animator; // Referência ao Animator
    private bool isFlipped = false; // Estado do flip

    public int vida = 5; // Vida inicial do inimigo
    public int danoNormal = 1; // Dano causado por bala normal
    public int danoEspecial = 2; // Dano causado por bala especial
	public int danoPlayer = 10;

    private Transform target; // Alvo atual
    private bool podeReceberDano = true; // Flag para verificar se o inimigo pode receber dano

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

    // Coroutine para restaurar a velocidade após um delay
    private IEnumerator RestoreSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speed = 2f; // Restaura a velocidade original do inimigo
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Colidiu com: {collision.gameObject.name}"); // Log para ver o que está colidindo

        // Aplica dano baseado na tag do objeto colidido
        if (collision.gameObject.CompareTag("Bala"))
        {
            ReceberDano(danoNormal); // Aplica o dano normal
            Destroy(collision.gameObject); // Opcional: destrói a bala após o impacto
        }
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            ReceberDano(danoEspecial); // Aplica o dano especial (dano extra)
            Destroy(collision.gameObject); // Opcional: destrói a bala especial após o impacto
        }
    }

    private void DestruirInimigo()
    {
        Debug.Log("Inimigo destruído!");
        Destroy(gameObject, 2f); // Destrói o objeto do inimigo
    }
}