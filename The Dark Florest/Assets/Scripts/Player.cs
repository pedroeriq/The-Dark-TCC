using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    
    public TextMeshProUGUI specialAmmoText; // Adiciona a referência ao TextMeshPro para mostrar a munição especial
    public AudioSource audioTiro;
    public AudioSource audioPulo;
    public AudioSource audioCorrendo;
    private bool isHit = false;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.5f;
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
    public int enemyDamage = 10;
    public float attackCooldown = 1f;
    public float meleeAttackRange = 1f;
    public LayerMask enemyLayers;

    public Image[] heartImages; // Array para armazenar as imagens dos corações
    public Sprite fullHeart, halfHeart, emptyHeart; // Sprites para cada estado do coração
    
    private Rigidbody2D rig;
    private bool isGrounded;
    private bool hasSpecialAmmo;
    private int specialAmmoCount;
    public int currentHealth; // 6 pontos de vida (2 pontos para cada coração)
    public int maxHealth = 6;
    public Animator Anim;
    private float movement;
    private bool move = true;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isDead = false; // Variável para controlar se o Player já morreu

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        hasSpecialAmmo = false;
        specialAmmoCount = 0;
        currentHealth = maxHealth;
        Anim = GetComponent<Animator>();
        UpdateHearts();
        CheckpointManager.Instance.lastCheckpointPosition = transform.position;
        UpdateSpecialAmmoText(); 
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && specialAmmoCount > 0 && !isAttacking)
        {
            if (audioTiro != null)
            {
                audioTiro.Play();
            }
            StartCoroutine(PerformSpecialAttack());
        }
        if (move && !isAttacking)
        {
            Move();
            Jump();
            Attack();
            MeleeAttack();
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
        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.Z)) // Ataque normal com a tecla Z
            {
                StartCoroutine(PerformNormalAttack()); // Ataque normal

                // Reproduz o som de tiro
                if (audioTiro != null)
                {
                    audioTiro.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.V)) // Ataque especial com a tecla V
            {
                if (hasSpecialAmmo && specialAmmoCount > 0)
                {
                    StartCoroutine(PerformSpecialAttack()); // Ataque especial
                    if (audioTiro != null)
                    {
                        audioTiro.Play();
                    }
                }
            }
        }
    }



    // Novo método para o ataque corpo a corpo
    void MeleeAttack()
    {
        // Verifica se a cena atual é a "FINALBOOS"
        if (SceneManager.GetActiveScene().name == "FINALBOSS") 
        {
            return; // Sai do método sem executar o ataque
        }

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
        if (specialAmmoCount <= 0 || isAttacking)
        {
            yield break; // Se não tiver munição ou já estiver atacando, não faz nada
        }

        isAttacking = true;
        move = false;
        rig.velocity = Vector2.zero;
        Anim.SetTrigger("Shot Especial"); // Chama a animação do tiro especial

        yield return new WaitForSeconds(0.2f); // Sincroniza com a animação

        // Instancia a bala especial
        SpecialFireBullet(specialBulletPrefab); 

        // Diminui a quantidade de munição
        specialAmmoCount--; 
        UpdateSpecialAmmoText(); // Atualiza a UI

        if (specialAmmoCount <= 0)
        {
            hasSpecialAmmo = false; // Se a munição acabou
        }

        yield return new WaitForSeconds(0.5f); // Tempo de cooldown do ataque

        isAttacking = false;
        move = true;
        yield return new WaitForSeconds(attackCooldown); // Cooldown entre ataques
        canAttack = true; // Permite atacar novamente
    }
    void UpdateSpecialAmmoText()
    {
        specialAmmoText.text = "" + specialAmmoCount.ToString();
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
        
        else if (collision.gameObject.CompareTag("Espinho"))
        {
            TakeDamage(enemyDamage = 10); // Apenas aplica dano sem knockback
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Checkpoint"))
        {
            CheckpointManager.Instance.lastCheckpointPosition = transform.position; // Salva a posição do checkpoint
            Debug.Log("Checkpoint alcançado: " + transform.position);
        }

        // Outros casos de colisão, como a coleta de munição especial ou cartas
        else if (collider.CompareTag("SpecialAmmo"))
        {
            // Aumenta a quantidade de munição especial sem limite
            specialAmmoCount += 5; // Soma 5 à munição atual

            // Atualiza o texto na tela para mostrar a nova quantidade
            UpdateSpecialAmmoText();

            // Destroi o item de munição especial coletado
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("Carta"))
        {
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.CompareTag("Bloco"))
        {
            TakeDamage(1); // Apenas aplica dano
        }
    }
    
    public void TakeDamage(int damage, Vector2? knockbackDirection = null)
    {
        if (isHit || isDead) return; // Impede que o jogador receba dano se já estiver morto ou em hit

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            isHit = true;
            Anim.SetTrigger("Hit");
            SetMove(false); // Desativa a movimentação

            if (knockbackDirection.HasValue)
            {
                ApplyKnockback(knockbackDirection.Value);
                StartCoroutine(HandleKnockback());
            }
            else
            {
                // Se não houver knockback, reative o movimento após a animação
                StartCoroutine(HandleHitOnly());
            }
        }

        UpdateHearts();
    }
    private IEnumerator HandleHitOnly()
    {
        yield return new WaitForSeconds(0.5f); // Aguarda a duração da animação de hit
        isHit = false; // Reseta o estado de hit
        SetMove(true); // Habilita a movimentação
    }
    
    private void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (currentHealth >= (i + 1) * 2)
            {
                heartImages[i].sprite = fullHeart; // Coração cheio
            }
            else if (currentHealth == (i * 2) + 1)
            {
                heartImages[i].sprite = halfHeart; // Coração pela metade
            }
            else
            {
                heartImages[i].sprite = emptyHeart; // Coração vazio
            }
        }
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
    

    private void Die()
    {
        if (isDead) return; // Impede que o jogador morra várias vezes

        isDead = true; // Marca que o jogador está morto
        StartCoroutine(HandleDeath());

        // Espera um pouco antes de reiniciar a cena ou restaurar o checkpoint
    }

    private IEnumerator RestoreCheckpoint()
    {

        // Restaura a posição do jogador para o último checkpoint
        transform.position = CheckpointManager.Instance.lastCheckpointPosition;

        // Restaura a vida
        currentHealth = maxHealth;
        UpdateHearts();

        // Reseta o estado do jogador
        isDead = false;
        Anim.SetInteger("transition", 0);
        yield return new WaitForSeconds(0.2f); // Duração da animação de morte
        isDead = false; // Permite que o Player receba dano novamente
        move = true;
        rig.isKinematic = false; // Reativa a física do Rigidbody
        GameController.instance.gameOver.SetActive(false);
        
        
    }

    public void VoltarVida()
    {
        StartCoroutine(RestoreCheckpoint());
    }
    

    private IEnumerator HandleDeath()
    {
        Anim.SetTrigger("Dead");
        move = false;
        rig.velocity = Vector2.zero; // Para o movimento do Rigidbody
        rig.isKinematic = true; // Desativa a física para evitar que o Player seja empurrado
        yield return new WaitForSeconds(1.0f);
        
        if (SceneManager.GetActiveScene().name == "FINALBOSS")
        {
            StartCoroutine(ReloadBossScene());
        }
        else
        {
            // Caso contrário, carrega a tela de game over
            StartCoroutine(GameOver());
            
        }
        
    }
    private IEnumerator ReloadBossScene()
    {
        yield return new WaitForSeconds(1f); // Aguarda a duração da animação de morte ou outro tempo desejado
        SceneManager.LoadScene("FINALBOSS"); // Recarrega a cena do boss final
    }
    // Coroutine para exibir a tela de game over
    private IEnumerator GameOver()
{
    yield return new WaitForSeconds(1f); // Aguarda a duração da animação de morte ou outro tempo desejado

    // Exibe a tela de Game Over
    if (GameController.instance.gameOver != null)
    {
        GameController.instance.gameOver.SetActive(true);
    }

    // Habilita o cursor para permitir interação com os botões
    
}


    

    private void Respawn()
    {
        isDead = false; // Permite que o Player receba dano novamente
        rig.isKinematic = false; // Reativa a física do Rigidbody
        Vector3 respawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
        transform.position = respawnPosition;
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void SetMove(bool canMove)
    {
        move = canMove;
        rig.velocity = Vector2.zero;
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

        UpdateHearts(); // Atualiza a barra de vida na interface
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