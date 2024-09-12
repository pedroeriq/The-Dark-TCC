using UnityEngine;

public class EnemyComum : MonoBehaviour
{
    public Transform player;                     // Referência ao jogador
    public float moveSpeed = 2f;                 // Velocidade do movimento do inimigo
    public float detectionRange = 5f;            // Distância máxima para detectar o jogador
    public float stopDistance = 2f;              // Distância mínima para parar de se mover
    public float attackRange = 1f;               // Distância de ataque (não usada aqui, mas para referência futura)
    public float attackCooldown = 1f;            // Tempo entre ataques (não usado aqui, mas para referência futura)
    public Transform areaLimit;                  // Área que limita o movimento do inimigo (não usado aqui, mas para referência futura)

    private float lastAttackTime;                // Tempo do último ataque (não usado aqui, mas para referência futura)
    private SpriteRenderer spriteRenderer;       // Referência ao SpriteRenderer para controlar a aparência do inimigo

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (PlayerInRange())
        {
            Vector3 directionToPlayer = player.position - transform.position; // Direção em relação ao jogador
            float distanceToPlayer = directionToPlayer.magnitude;            // Distância até o jogador

            if (distanceToPlayer > stopDistance)
            {
                // Move o inimigo em direção ao jogador
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }

            // Atualiza a rotação para sempre olhar para o jogador
            UpdateRotation(directionToPlayer);

            // Inverte a direção do sprite do inimigo com base na direção em relação ao jogador
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }
    }

    bool PlayerInRange()
    {
        // Verifica a distância entre o inimigo e o jogador
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    void UpdateRotation(Vector3 direction)
    {
        // Calcula o ângulo necessário para que o inimigo olhe na direção do jogador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        // Ajuste a subtração de 90 graus conforme necessário para alinhar corretamente o sprite
    }

    private void OnDrawGizmos()
    {
        // Desenha a área de detecção com Gizmos
        Gizmos.color = Color.green; // Cor para a área de detecção
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área de detecção quando o objeto está selecionado
        Gizmos.color = Color.yellow; // Cor para a área de detecção quando selecionado
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}