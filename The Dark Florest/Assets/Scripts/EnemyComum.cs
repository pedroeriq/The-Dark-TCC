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

    private bool movingRight = true; // Direção atual do movimento
    private float timer = 0f; // Temporizador para a mudança de direção

    void Update()
    {
        MoveEnemy();
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

    void Flip()
    {
        // Inverte a escala no eixo X para virar o rosto
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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