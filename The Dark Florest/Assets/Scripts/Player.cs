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
            transform.eulerAngles = new Vector3(0, 180, 0); // Vira o jogador para a esquerda
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1;
            transform.eulerAngles = new Vector3(0, 0, 0); // Vira o jogador para a direita
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
            FireBullet(specialBulletPrefab);
            specialAmmoCount--;

            if (specialAmmoCount <= 0)
            {
                hasSpecialAmmo = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            FireBullet(bulletPrefab);
        }
    }

    void FireBullet(GameObject bullet)
    {
        Vector3 spawnPosition = transform.position + transform.right * 1.0f;
        GameObject newBullet = Instantiate(bullet, spawnPosition, Quaternion.identity);

        // Define a direção da bala de acordo com a orientação do jogador
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(transform.right); // Passa a direção para o script da bala
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