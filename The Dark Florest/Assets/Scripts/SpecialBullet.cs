using UnityEngine;

public class SpecialBullet : Bullet
{
    public float lifetime = 5f; // Tempo de vida da bala especial em segundos

    void Start()
    {
        // Destrói a bala especial após 'lifetime' segundos
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com um objeto com a tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Destrói a bala especial ao colidir com o inimigo
        }
    }
}