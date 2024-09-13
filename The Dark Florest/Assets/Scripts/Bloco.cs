using System.Collections;
using UnityEngine;

public class Bloco : MonoBehaviour
{
    public GameObject blocoPrefab;
    public GameObject pontoA;
    public float speedMove;
    public float returnDelay = 2.0f; // Tempo para esperar antes de voltar
    public float returnSpeed = 1.0f; // Velocidade de retorno para a posição inicial

    private bool isMove;
    private Vector2 initialPosition;

    void Start()
    {
        // Armazena a posição inicial do bloco
        initialPosition = blocoPrefab.transform.position;
    }

    void Update()
    {
        if (isMove)
        {
            blocoPrefab.transform.position = Vector2.MoveTowards(blocoPrefab.transform.position, pontoA.transform.position, speedMove * Time.deltaTime);

            // Verifica se o bloco chegou ao ponto A
            if ((Vector2)blocoPrefab.transform.position == (Vector2)pontoA.transform.position)
            {
                StartCoroutine(ReturnToInitialPosition());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = blocoPrefab.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 7;
            rb.mass = 400;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
        {
            Rigidbody2D rb = blocoPrefab.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.mass = 0;

            isMove = true;
        }
    }

    private IEnumerator ReturnToInitialPosition()
    {
        yield return new WaitForSeconds(returnDelay); // Espera pelo tempo especificado

        // Smoothly moves the block back to the initial position
        Vector2 startPosition = blocoPrefab.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < returnDelay)
        {
            blocoPrefab.transform.position = Vector2.Lerp(startPosition, initialPosition, (elapsedTime / returnDelay));
            elapsedTime += Time.deltaTime * returnSpeed;
            yield return null;
        }

        // Ensures the final position is exactly the initial position
        blocoPrefab.transform.position = initialPosition;
        isMove = false; // Para o movimento
    }
}