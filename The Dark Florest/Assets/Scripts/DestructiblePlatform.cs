using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
    public float destroyDelay = 0.5f; // Tempo antes de destruir a plataforma

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o jogador entrou em contato com a plataforma
        if (collision.gameObject.CompareTag("Player"))
        {
            // Inicia a contagem regressiva para destruir a plataforma
            Destroy(gameObject, destroyDelay);
        }
    }
}