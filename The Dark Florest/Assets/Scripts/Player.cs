using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections; // Necessário para usar Coroutines

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
    public Animator Anim;
    private float movement;
    private bool move = true;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        hasSpecialAmmo = false;
        specialAmmoCount = 0;
        currentHealth = maxHealth;
        Anim = GetComponent<Animator>();
        UpdateHealthBar();
        CheckpointManager.Instance.lastCheckpointPosition = transform.position;
    }

    void Update()
    {
        if (move == true)
        {
            Move();
            Jump();
            Shoot();

            // Controle das animações por transições
            if (!isGrounded) 
            {
                // Se não estiver no chão, define a animação de pulo
                Anim.SetInteger("transition", 2);
            }
            else if (movement != 0) 
            {
                // Se estiver se movendo e no chão, define a animação de corrida
                Anim.SetInteger("transition", 1);
            }
            else
            {
                // Se não estiver se movendo, define a animação de idle
                Anim.SetInteger("transition", 0);
            }
        }
        
        
    }

    void Move()
    {
        movement = Input.GetAxis("Horizontal");

        if (movement < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // Virando para esquerda
        }
        else if (movement > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // Virando para direita
        }

        rig.velocity = new Vector2(movement * speed, rig.velocity.y);
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            ParticleObserver.OnParticleSpawEvent(transform.position);
        
            if (AudioObserver.instance != null)
            {
                AudioObserver.TriggerPlaySfx("pulo");
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
        // Inicia a rotina de morte, que inclui a animação e o respawn
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Define a animação de morte (transition 3)
        Anim.SetTrigger("Dead");
        move = false;
        // Espera alguns segundos (ajuste conforme necessário)
        yield return new WaitForSeconds(2.0f);
        move = true;

        // Respawn após a animação de morte
        Respawn();
    }

    private void Respawn()
    {
        // Usa o método para obter a última posição de checkpoint
        Vector3 respawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        transform.position = respawnPosition;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}