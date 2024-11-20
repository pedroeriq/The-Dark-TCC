using UnityEngine;
using System.Collections;

public class PlataformaFall : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 initialPosition; // Guarda a posição inicial
    public float fallDelay = 2f;
    public float resetDelay = 3f; // Tempo para retornar à posição inicial

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; // Armazena a posição inicial
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.5f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(fallDelay); // Espera o tempo de queda
        rb.bodyType = RigidbodyType2D.Static; // Congela a plataforma
        StartCoroutine(ResetPlatform()); // Inicia a rotina de reset
    }

    IEnumerator ResetPlatform()
    {
        yield return new WaitForSeconds(resetDelay);
        rb.bodyType = RigidbodyType2D.Static; // Define o Rigidbody como estático
        rb.velocity = Vector2.zero; // Zera a velocidade para evitar movimento residual
        transform.position = initialPosition; // Retorna à posição inicial
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }
}