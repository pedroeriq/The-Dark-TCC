using UnityEngine;

public class AttackObject : MonoBehaviour
{
    public float lifetime = 1f; // Tempo antes de o objeto ser destruído
    public int damage = 1; // Valor do dano que o objeto causará
    private Vector3 spawnPosition; // Posição onde o ataque foi instanciado
    public float damageRange = 3f; // Distância máxima para o dano ser aplicado
    public float knockbackForceVertical = 5f; // Força do knockback para cima (ajustável)
    public float knockbackForceHorizontal = 2f; // Força do knockback lateral (ajustável)

    private void Start()
    {
        // Armazena a posição do ataque ao ser instanciado
        spawnPosition = transform.position;

        // Destroi o objeto após o tempo de vida
        Destroy(gameObject, lifetime);
    }

    // Detecta a colisão com outros objetos
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Verifica se o objeto colidido é o Player
        if (collider.CompareTag("Player"))
        {
            // Verifica a distância entre o jogador e a posição do ataque
            float distance = Vector3.Distance(collider.transform.position, spawnPosition);

            // Aplica o dano apenas se o jogador estiver dentro da área de dano
            if (distance <= damageRange)
            {
                Player player = collider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(damage); // Chama o método que aplica dano no Player

                    // Aplica o knockback
                    ApplyKnockback(player);
                }
            }

            // Destrói o objeto de ataque após causar dano (se houver)
            Destroy(gameObject,1f);
        }
    }

    // Método para aplicar o knockback no jogador (vertical e lateral)
    private void ApplyKnockback(Player player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Aplica a força de knockback para cima
            rb.AddForce(Vector2.up * knockbackForceVertical, ForceMode2D.Impulse);

            // Aplica uma leve força para o lado (positivo ou negativo, dependendo de onde o ataque veio)
            // Aqui estamos aplicando uma força horizontal positiva, mas você pode variar isso conforme o lado do ataque.
            rb.AddForce(new Vector2(knockbackForceHorizontal, 0), ForceMode2D.Impulse);
        }
    }
}
