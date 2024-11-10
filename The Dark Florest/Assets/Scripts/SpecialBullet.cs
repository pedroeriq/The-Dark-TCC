using UnityEngine;

public class SpecialBullet : Bullet
{
    public float specialLifetime = 5f; // Tempo de vida da bala especial em segundos

    protected override void Start()
    {
        // Destrói a bala especial após 'specialLifetime' segundos
        Destroy(gameObject, specialLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Verifica se a colisão é com um objeto com a tag "Enemy"
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Destrói a bala especial ao colidir com o inimigo
        }
    }
}