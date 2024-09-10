using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // Velocidade da bala
    public int damage = 10; // Adiciona uma variável de dano
    private Vector2 direction; // Direção da bala

    void Update()
    {
        // Move a bala na direção definida
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com um objeto com a tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}