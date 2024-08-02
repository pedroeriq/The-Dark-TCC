using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce; // Adiciona uma variável para a força do pulo

    private Rigidbody2D rig;
    private bool isGrounded; // Adiciona uma variável para verificar se o jogador está no chão

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump(); // Chama a função de pulo na atualização
    }

    void Move()
    {
        float movement = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1;
        }
        else
        {
            movement = 0;
        }

        rig.velocity = new Vector2(movement * speed, rig.velocity.y);
    }

    void Jump()
    {
        // Verifica se o jogador está no chão e a seta para cima é pressionada
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    // Detecta colisão com o chão para determinar se o jogador está no chão
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}