using UnityEngine;

public class AttackObject : MonoBehaviour
{
    public float lifetime = 1f; // Tempo antes de o objeto ser destruído

    private void Start()
    {
        // Destroi o objeto após o tempo de vida
        Destroy(gameObject, lifetime);
    }
}