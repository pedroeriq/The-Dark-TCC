using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Rigidbody2D rb;
    public Transform groundCheck; 
    public LayerMask groundLayer; 

    private Vector2 movement;
    private bool isGrounded;

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

       
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}