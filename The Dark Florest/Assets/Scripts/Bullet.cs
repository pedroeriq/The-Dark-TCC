using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // Velocidade da bala

    void Update()
    {
        // Move a bala para frente
        transform.Translate(Vector2.right * speed * Time.deltaTime);
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