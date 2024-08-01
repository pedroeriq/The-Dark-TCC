using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    private bool isJumping;
    private Animator anim;
    private Rigidbody2D rig;

    void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
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

        if (movement != 0)
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 1);
            }
        }

        if (movement == 0 && !isJumping)
        {
            anim.SetInteger("transition", 0);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isJumping && Mathf.Abs(rig.velocity.y) < 0.01f)
            {
                anim.SetInteger("transition", 2);
                rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && Mathf.Abs(rig.velocity.y) < 0.01f)
        {
            isJumping = false;
        }
    }
}