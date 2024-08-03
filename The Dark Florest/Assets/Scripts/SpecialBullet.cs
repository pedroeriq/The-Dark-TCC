using UnityEngine;

public class SpecialBullet : Bullet
{
    // Se quiser adicionar comportamento específico da bala especial no futuro, você pode adicionar aqui
    // Por enquanto, ela usará a lógica da bala normal

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com um objeto com a tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}