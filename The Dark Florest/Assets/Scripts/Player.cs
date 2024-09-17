using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab;
    public int specialAmmoLimit = 5;
    public int maxHealth = 100;
    public int enemyDamage = 10;

    public Image healthBarFill;

    private Rigidbody2D rig;
    private bool isGrounded;
    private bool hasSpecialAmmo;
    private int specialAmmoCount;
    private float currentHealth;
    private Animator Anim;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        hasSpecialAmmo = false;
        specialAmmoCount = 0;
        currentHealth = maxHealth;
        TryGetComponent(out Anim);
        UpdateHealthBar();
        CheckpointManager.Instance.lastCheckpointPosition = transform.position;

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
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1;
            transform.eulerAngles = new Vector3(0, 0, 0);
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
            ParticleObserver.OnParticleSpawEvent(transform.position); // Correção para o evento de partículas
        
            // Chama o evento para tocar o som de pulo
            if (AudioObserver.instance != null)
            {
                AudioObserver.TriggerPlaySfx("pulo"); // Assume que o som de pulo é identificado por "pulo"
            }
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

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(transform.right);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Anim.SetBool("isGrounded", isGrounded);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage);
        }
        else if (collision.gameObject.CompareTag("Bloco"))
        {
            TakeDamage(enemyDamage);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Anim.SetBool("isGrounded", isGrounded);
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Respawn();
    }

    private void Respawn()
    {
        // Use o método público para obter a última posição de checkpoint
        Vector3 respawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        transform.position = respawnPosition;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}