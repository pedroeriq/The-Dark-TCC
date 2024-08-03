using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab;
    public int specialAmmoLimit = 5;
    public int maxHealth = 100;
    public int enemyDamage = 10; // Adiciona uma variável pública para o dano que o jogador sofrerá ao colidir com inimigos

    private Rigidbody2D rig;
    private bool isGrounded;
    private bool hasSpecialAmmo;
    private int specialAmmoCount;
    private int currentHealth;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        hasSpecialAmmo = false;
        specialAmmoCount = 0;
        currentHealth = maxHealth;
    }

    void Update()
    {
        Move();
        Jump();
        Shoot();
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
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.C) && hasSpecialAmmo && specialAmmoCount > 0)
        {
            Vector3 spawnPosition = transform.position + transform.right * 1.0f;
            Instantiate(specialBulletPrefab, spawnPosition, Quaternion.identity);
            specialAmmoCount--;

            if (specialAmmoCount <= 0)
            {
                hasSpecialAmmo = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Vector3 spawnPosition = transform.position + transform.right * 1.0f;
            Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage); // Usa a variável pública para aplicar dano
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("SpecialAmmo"))
        {
            hasSpecialAmmo = true;
            specialAmmoCount = specialAmmoLimit;
            Destroy(collider.gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}