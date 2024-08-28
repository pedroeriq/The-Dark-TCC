using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
    public float destroyDelay = 0.5f; // Tempo total antes de destruir a plataforma
    public float fallSpeed = 2.0f; // Velocidade com a qual a plataforma cai

    private bool shouldFall = false;
    private float fallStartTime;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (shouldFall)
        {
            // Calcula o tempo decorrido desde o início da queda
            float elapsedTime = Time.time - fallStartTime;
            
            // Move a plataforma para baixo com base na velocidade de queda
            transform.position = initialPosition + Vector3.down * Mathf.Min(fallSpeed * elapsedTime, 1000); // Limite para evitar problemas

            // Verifica se o tempo de queda passou e destrói a plataforma
            if (elapsedTime >= destroyDelay)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o jogador entrou em contato com a plataforma
        if (collision.gameObject.CompareTag("Player"))
        {
            // Inicia a queda
            shouldFall = true;
            fallStartTime = Time.time;
        }
    }
}