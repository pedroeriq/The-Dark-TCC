using UnityEngine;

public class CircleAttack : MonoBehaviour
{
    public float speed = 5f; // Velocidade do círculo
    private Vector3 moveDirection; // Direção do movimento
    public int damage = 2; // Dano causado ao jogador

    // Define a direção do movimento
    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    private void Update()
    {
        // Move o círculo na direção definida
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Verifica se o jogador possui um script com o método TakeDamage
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); // Aplica dano ao jogador
                Debug.Log("Jogador atingido! Dano causado: " + damage);
            }

            // Destroi o CircleAttack após a colisão
            Destroy(gameObject);
        }
    }
}