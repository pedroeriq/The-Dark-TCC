using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform SpecialFirePoint;
    public Transform FirePoint;
    public bool isJumping;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float speed;
    public float jumpForce;
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab;
    public int specialAmmoLimit = 5;
    public int maxHealth = 100;
    public int enemyDamage = 10;
    public float attackCooldown = 1f; // Tempo de espera entre os ataques
    public float meleeAttackRange = 1f; // Alcance do ataque corpo a corpo
    public LayerMask enemyLayers; // Camada dos inimigos

    public Image healthBarFill;

    private Rigidbody2D rig;
    private bool isGrounded;
    private bool hasSpecialAmmo;
    private int specialAmmoCount;
    private float currentHealth;
    public Animator Anim;
    private float movement;
    private bool move = true;
    private bool isAttacking = false;
    private bool canAttack = true; // Controle de cooldown

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
        if (move && !isAttacking) // Bloqueia movimento enquanto ataca
        {
            Move();
            Jump();
            Attack(); // Ataque à distância
            MeleeAttack(); // Ataque corpo a corpo
            CheckGround();

            if (!isGrounded)
            {
                Anim.SetInteger("transition", 2); // Pulo
            }
            else if (movement != 0)
            {
                Anim.SetInteger("transition", 1); // Correndo
            }
            else
            {
                Anim.SetInteger("transition", 0); // Idle
            }
        }
    }

    void Move()
    {
        movement = Input.GetAxis("Horizontal");

        if (movement < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (movement > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        rig.velocity = new Vector2(movement * speed, rig.velocity.y);
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            ParticleObserver.OnParticleSpawEvent(transform.position);
            isJumping = true;
            Anim.SetInteger("transition", 2);

            if (AudioObserver.instance != null)
            {
                AudioObserver.TriggerPlaySfx("pulo");
            }
        }
    }

    void Attack()
    {
        if (canAttack && Input.GetKeyDown(KeyCode.Z)) // Ataque com a tecla Z
        {
            if (hasSpecialAmmo && specialAmmoCount > 0)
            {
                StartCoroutine(PerformSpecialAttack()); // Ataque especial
            }
            else
            {
                StartCoroutine(PerformNormalAttack()); // Ataque normal
            }
        }
    }

    // Novo método para o ataque corpo a corpo
    void MeleeAttack()
    {
        if (canAttack && Input.GetKeyDown(KeyCode.C)) // Ataque corpo a corpo com a tecla C
        {
            StartCoroutine(PerformMeleeAttack());
        }
    }

// Coroutine para realizar o ataque corpo a corpo com delay e cooldown
    IEnumerator PerformMeleeAttack()
    {
        move = false;
        rig.velocity = Vector2.zero;
        isAttacking = true;
        canAttack = false;
        Anim.SetTrigger("Attack"); // Animação de ataque corpo a corpo

        yield return new WaitForSeconds(0.2f); // Delay para sincronizar com a animação

        // Detectar inimigos no alcance do ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(FirePoint.position, meleeAttackRange, enemyLayers);

        // Causar dano aos inimigos atingidos
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Inimigo atingido: " + enemy.name);
        
            // Verifica se o objeto atingido possui o componente SombraErrante
            SombraErrante sombra = enemy.GetComponent<SombraErrante>();
            if (sombra != null)
            {
                sombra.ReceberDano(1); // Aplica 1 de dano (ou a quantidade que você quiser)
            }
        
            // Verifica se o objeto atingido possui o componente Enemy2
            Enemy2 enemy2 = enemy.GetComponent<Enemy2>();
            if (enemy2 != null)
            {
                enemy2.ReceberDano(1); // Aplica 1 de dano (ou a quantidade que você quiser)
            }
        }

        yield return new WaitForSeconds(0.5f); // Tempo adicional para completar o ataque

        isAttacking = false;
        move = true;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; // Libera novos ataques
    }

    // Coroutine para realizar o ataque normal com delay e cooldown
    IEnumerator PerformNormalAttack()
    {
        move = false;
        rig.velocity = Vector2.zero;
        isAttacking = true;
        canAttack = false;
        Anim.SetTrigger("Shot"); // Animação de ataque normal

        yield return new WaitForSeconds(0.2f); // Delay para sincronizar com a animação

        FireBullet(bulletPrefab); // Dispara o ataque normal

        yield return new WaitForSeconds(0.5f); // Tempo adicional para completar o ataque

        isAttacking = false;
        move = true;

        // Aguarda o cooldown antes de permitir outro ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; // Libera novos ataques
    }

    // Coroutine para realizar o ataque especial com delay e cooldown
    IEnumerator PerformSpecialAttack()
    {
        move = false;
        rig.velocity = Vector2.zero;
        isAttacking = true;
        canAttack = false;
        Anim.SetTrigger("Shot Especial"); // Animação de ataque especial

        yield return new WaitForSeconds(0.2f); // Delay para sincronizar com a animação

        SpecialFireBullet(specialBulletPrefab); // Dispara o ataque especial
        specialAmmoCount--; // Reduz a contagem de munição especial

        if (specialAmmoCount <= 0)
        {
            hasSpecialAmmo = false; // Acabou a munição especial
        }

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        move = true;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; // Libera novos ataques
    }

    void FireBullet(GameObject bullet)
    {
        GameObject newBullet = Instantiate(bullet, FirePoint.position, Quaternion.identity);

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(transform.right);
        }
    }

    void SpecialFireBullet(GameObject bullet)
    {
        GameObject newBullet = Instantiate(bullet, SpecialFirePoint.position, Quaternion.identity);

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(transform.right);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage);
        }
        else if (collision.gameObject.CompareTag("Bloco"))
        {
            TakeDamage(enemyDamage);
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
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        Anim.SetTrigger("Dead");
        move = false;
        yield return new WaitForSeconds(2.0f);
        move = true;

        Respawn();
    }

    private void Respawn()
    {
        Vector3 respawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        transform.position = respawnPosition;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void SetMove(bool canMove)
    {
        move = canMove;
    }

    public bool GetMove()
    {
        return move;
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            isJumping = false;
        }
    }

    // Visualizar o alcance do ataque corpo a corpo no editor
    private void OnDrawGizmosSelected()
    {
        if (FirePoint == null)
            return;

        Gizmos.DrawWireSphere(FirePoint.position, meleeAttackRange);
    }
}
