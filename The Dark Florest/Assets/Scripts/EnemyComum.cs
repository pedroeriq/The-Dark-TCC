using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComum : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocidade do movimento do inimigo
    public float changeDirectionTime = 3f; // Tempo para mudar a direção
    public int health = 3; // Vida do inimigo
    public int normalDamage = 1; // Dano causado por balas normais
    public int specialDamage = 3; // Dano causado por special ammo
    public Transform player; // Referência ao jogador
    public float detectionRange = 5f; // Distância máxima para detectar o jogador

    private bool movingRight = true; // Direção atual do movimento
    private float timer = 0f; // Temporizador para a mudança de direção

    void Update()
    {
        if (PlayerInRange())
        {
            MoveTowardsPlayer();
        }
        else
        {
            MoveEnemy();
        }
    }

    bool PlayerInRange()
    {
        // Verifica a distância entre o inimigo e o jogador
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    void MoveEnemy()
    {
        timer += Time.deltaTime;

        if (timer >= changeDirectionTime)
        {
            timer = 0f;
            movingRight = !movingRight; // Troca a direção
            Flip(); // Vira o rosto do inimigo
        }

        float moveDirection = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);
    }

    void MoveTowardsPlayer()
    {
        // Calcula a direção para o jogador
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        
        // Vira o inimigo para o lado do jogador
        if (direction.x > 0 && !movingRight || direction.x < 0 && movingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Inverte a escala no eixo X para virar o rosto
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        movingRight = !movingRight;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            TakeDamage(normalDamage); // Aplica dano normal
            Destroy(collision.gameObject); // Destrói o objeto da bala
        }
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            TakeDamage(specialDamage); // Aplica dano especial
            Destroy(collision.gameObject); // Destrói o objeto do special ammo
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject); // Destrói o inimigo quando a vida chega a 0
        }
    }
}