using UnityEngine;

public class Coin : MonoBehaviour
{
    private Collider2D coinCollider;

    private void Awake()
    {
        coinCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameController.instance != null)
            {
                GameController.instance.CoinCollected(); // Notifica o GameController sobre a coleta
            }
            Destroy(gameObject); // Destrói a moeda após coleta
        }
    }
}