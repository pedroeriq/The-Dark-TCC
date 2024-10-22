using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class Player : MonoBehaviour
{
    public AudioSource audioTiro;
    public AudioSource audioPulo;  // Para o som de pulo
    public AudioSource audioCorrendo;  // Para o som de corrida
    private bool isHit = false; // Para controlar se o player está recebendo dano
    public float knockbackForce = 5f; // Força do knockback
    public float knockbackDuration = 0.5f; // Duração do knockback
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

        // Tocar o som de corrida enquanto o jogador está se movendo
        if (movement != 0 && isGrounded)
        {
            if (audioCorrendo != null && !audioCorrendo.isPlaying)
            {
                audioCorrendo.Play();  // Toca o som de correndo
            }
        }
        else if (audioCorrendo != null && audioCorrendo.isPlaying)
        {
            audioCorrendo.Stop();  // Para o som quando o jogador para de correr
        }
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

            if (audioPulo != null)
            {
                audioPulo.Play();  // Toca o som de pulo
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
                if (audioTiro != null)
                {
                    audioTiro.Play();
                }
            }
            else
            {
                StartCoroutine(PerformNormalAttack()); // Ataque normal

                // Reproduz o som de tiro
                if (audioTiro != null)
                {
                    audioTiro.Play();
                }
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
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized; // Direção oposta ao inimigo
            TakeDamage(enemyDamage, knockbackDirection); // Aplica o dano e o knockback
        }
        else if (collision.gameObject.CompareTag("Bloco"))
        {
            TakeDamage(enemyDamage); // Apenas aplica dano sem knockback
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

    public void TakeDamage(int damage, Vector2? knockbackDirection = null)
    {
        if (isHit) return; // Se já está em estado de "hit", não faça nada

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            isHit = true; // Marca que o player está em estado de "hit"
            Anim.SetTrigger("Hit"); // Chama a animação de hit
            SetMove(false); // Desabilita a movimentação

            if (knockbackDirection.HasValue)
            {
                ApplyKnockback(knockbackDirection.Value);
                StartCoroutine(HandleKnockback());
            }
        }

        UpdateHealthBar();
    }
    private IEnumerator HandleKnockback()
    {
        // Aguarde a duração do knockback (ajuste conforme necessário)
        yield return new WaitForSeconds(0.5f); // Duração do knockback

        // Aguarde o tempo da animação de "hit" (ajuste conforme necessário)
        yield return new WaitForSeconds(0.5f); // Duração da animação de hit

        isHit = false; // Reseta o estado de hit
        SetMove(true); // Habilita a movimentação
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
    public void Curar(int valorCura)
    {
        currentHealth += valorCura;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Garante que a vida não ultrapasse o máximo
        }

        UpdateHealthBar(); // Atualiza a barra de vida na interface
    }
    private void ApplyKnockback(Vector2 direction)
    {
        rig.velocity = Vector2.zero; // Para garantir que o jogador não se mova
        rig.AddForce(direction * knockbackForce, ForceMode2D.Impulse); // Aplica a força de knockback
        StartCoroutine(ResetMovementAfterKnockback());
    }

    private IEnumerator ResetMovementAfterKnockback()
    {
        move = false; // Bloqueia movimento
        yield return new WaitForSeconds(knockbackDuration); // Espera a duração do knockback
        move = true; // Libera movimento
    }

}
