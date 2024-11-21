using UnityEngine;

public class BlocoCair : MonoBehaviour
{
    public GameObject bloco; // Referência para o bloco
    private Rigidbody2D rb;  // Referência para o Rigidbody2D do bloco

    void Start()
    {
        if (bloco != null)
        {
            rb = bloco.GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D do bloco
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica se o que entrou na área de trigger é o Player
        {
            if (rb != null)
            {
                rb.isKinematic = false; // Faz o bloco cair (removendo o kinematic)
            }
        }
    }
}