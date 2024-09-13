using UnityEngine;
using UnityEngine.UI; // Necessário para acessar o componente Image
using TMPro; // Necessário para acessar o TextMesh Pro


public class Player : MonoBehaviour
{
    public TMP_Text moedaTXT; // Mudado para TMP_Text
    
    public float speed;
    public float jumpForce;
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab;
    public int specialAmmoLimit = 5;
    public int maxHealth = 100;
    public int enemyDamage = 10;

    public Image healthBarFill; // Referência para a barra de vida
    public Transform spawnPoint; // Referência para o ponto de renascimento
    public Transform checkpointPoint; // Referência para o ponto de checkpoint

    private Rigidbody2D rig;
    private bool isGrounded;
    private bool hasSpecialAmmo;
    private int specialAmmoCount;
    private float currentHealth; // Alterado para float
    private bool checkpointActivated; // Indica se o checkpoint foi ativado
    private Animator Anim; 

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        hasSpecialAmmo = false;
        specialAmmoCount = 0;
        currentHealth = maxHealth;
        TryGetComponent(out Anim);
        UpdateHealthBar(); // Atualiza a barra de saúde na inicialização
        checkpointActivated = false;
        checkpointPoint = null;

        // Atualiza o texto de moedas no início
        moedaTXT.text = CoinManager.instance.GetMoeda().ToString();
    }

    void Update()
    {
        moedaTXT.text = CoinManager.instance.GetMoeda().ToString();
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
        if (collider.gameObject.CompareTag("Moeda"))
        {
            CoinManager.instance.AddMoeda(1); // Usa o CoinManager para adicionar moedas
            moedaTXT.text = CoinManager.instance.GetMoeda().ToString(); // Atualiza o texto
            Destroy(collider.gameObject);
        }
        if (collider.CompareTag("SpecialAmmo"))
        {
            hasSpecialAmmo = true;
            specialAmmoCount = specialAmmoLimit;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("Checkpoint") && !checkpointActivated)
        {
            checkpointActivated = true; // Marca o checkpoint como ativado
            checkpointPoint = collider.transform; // Define o ponto de checkpoint quando o jogador colide com um checkpoint
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(); // Chama o método Die quando a saúde chega a 0
        }
        UpdateHealthBar(); // Atualiza a barra de saúde após receber dano
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth; // Atualiza o preenchimento da barra de saúde
        }
    }

    private void Die()
    {
        // Você pode adicionar aqui qualquer efeito visual ou sonoro para a morte
        Respawn(); // Chama o método Respawn para renascer o jogador
    }

    private void Respawn()
    {
        // Verifica se há um ponto de renascimento definido
        if (checkpointActivated && checkpointPoint != null)
        {
            // Reposiciona o jogador na posição do checkpoint
            transform.position = checkpointPoint.position;
        }
        else
        {
            // Caso não haja checkpoint ativado, usa o ponto de spawn
            transform.position = spawnPoint.position;
        }

        // Restaura a saúde
        currentHealth = maxHealth;
        UpdateHealthBar(); // Atualiza a barra de saúde após renascer
    }
}