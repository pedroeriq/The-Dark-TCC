using UnityEngine;
using System.Collections; // Adicione esta linha

public class PlataformaFall : MonoBehaviour
{
    private Rigidbody2D rb; // Corrigido o tipo e a visibilidade
    public float fallDelay = 2f; // Corrigido o nome da variável e a declaração

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Corrigido o tipo e adicionado o parêntese de fechamento
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.2f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, fallDelay); // Corrigido o nome da variável
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) // Corrigido a comparação de tag
        {
            StartCoroutine(Fall());
        }
    }
}