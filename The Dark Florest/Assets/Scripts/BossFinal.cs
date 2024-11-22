using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Para atualizar a barra de vida na UI

public class BossFinal : MonoBehaviour
{
    
    public Transform player; // Referência ao jogador
    public float chaseSpeed = 4f; // Velocidade ao perseguir o jogador
    public float detectionRange = 5f; // Distância de detecção do jogador

    public GameObject attackObjectPrefab; // Prefab do objeto de ataque
    public GameObject circleAttackPrefab; // Prefab do CircleAttack
    public float attackInterval = 3f; // Intervalo entre ataques
    public float attackDelay = 1f; // Tempo entre o início da animação e o aparecimento do ataque
    public float circleAttackInterval = 5f; // Intervalo entre os CircleAttacks

    private Animator animator; // Referência ao Animator
    private bool isFlipped = false; // O Boss começa flipado para a direita
    private bool isChasingPlayer = false; // Flag para verificar se está perseguindo o jogador
    private Coroutine attackRoutine; // Referência à rotina de ataque

    private float originalSpeed; // Velocidade original do Boss (para retomar após o ataque)

    // Adicionando variáveis para vida do Boss e barra de vida
    public int vida = 10; // Vida inicial do Boss
    public int danoNormal = 1; // Dano causado por bala normal
    public int danoEspecial = 2; // Dano causado por bala especial
    public Slider vidaSlider; // Referência ao Slider da UI para barra de vida

    // Variável para controlar se o Boss está morto
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Garante que o Boss começa flipado
        if (!isFlipped)
        {
            Flip();
        }

        // Ignorar colisão com o Player
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        // Armazena a velocidade original do Boss
        originalSpeed = chaseSpeed;

        // Configura a barra de vida inicial, mas começa oculta
        if (vidaSlider != null)
        {
            vidaSlider.maxValue = vida;
            vidaSlider.value = vida;
            vidaSlider.gameObject.SetActive(false); // Esconde a barra de vida inicialmente
        }
    }


    private void Update()
    {
        if (isDead)
            return; // Se o Boss estiver morto, ele não faz mais nada

        // Após começar a seguir o jogador, ele continua perseguindo independentemente do campo de detecção
        if (!isChasingPlayer && PlayerInRange())
        {
            isChasingPlayer = true;
            animator.SetInteger("transition", 1); // Inicia a animação de corrida (transition 1)
            StartCoroutine(AttackRoutine()); // Inicia a rotina de ataques

            // Ativa a barra de vida quando o Boss começar a seguir o jogador
            if (vidaSlider != null)
            {
                vidaSlider.gameObject.SetActive(true); // Exibe a barra de vida
            }
        }

        if (isChasingPlayer)
        {
            // Ativa a animação de corrida (transition 1) enquanto segue o jogador
            animator.SetInteger("transition", 1);
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        // Movimento em direção ao jogador
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        // Verifica a direção para flipar o Boss
        if (directionToPlayer.x < 0 && !isFlipped)
        {
            Flip();
        }
        else if (directionToPlayer.x > 0 && isFlipped)
        {
            Flip();
        }
    }

    private bool PlayerInRange()
    {
        // Verifica se o jogador está dentro do alcance de detecção
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private IEnumerator AttackRoutine()
    {
        while (isChasingPlayer && !isDead)
        {
            yield return new WaitForSeconds(attackInterval); // Espera até o próximo ataque

            // Desacelera o movimento durante o ataque
            chaseSpeed = 0f; // O Boss para de se mover

            // Inicia a animação de ataque
            animator.SetTrigger("Attack");

            // Aguarda o delay antes de instanciar o objeto de ataque
            yield return new WaitForSeconds(attackDelay);

            // Cria o objeto de ataque na posição do jogador
            if (attackObjectPrefab != null && player != null)
            {
                Vector3 spawnPosition = new Vector3(player.position.x, player.position.y, 0);
                Instantiate(attackObjectPrefab, spawnPosition, Quaternion.identity);
            }

            // Instancia um CircleAttack a cada `circleAttackInterval` segundos
            SpawnCircleAttack();

            // Retorna a velocidade ao normal após o ataque
            chaseSpeed = originalSpeed;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return; // Se o Boss já está morto, ignora qualquer colisão

        // Verifica se colidiu com a tag "Bala"
        if (collision.gameObject.CompareTag("Bala"))
        {
            TomarDano(danoNormal); // Aplica o dano normal
            Destroy(collision.gameObject); // Destroi o objeto da bala
        }
        // Verifica se colidiu com a tag "SpecialAmmo"
        else if (collision.gameObject.CompareTag("SpecialAmmo"))
        {
            TomarDano(danoEspecial); // Aplica o dano especial
            Destroy(collision.gameObject); // Destroi o objeto da bala especial
        }
    }

    private void TomarDano(int dano)
    {
        if (isDead)
            return; // Se o Boss estiver morto, não faz nada

        vida -= dano;
        if (vida < 0) vida = 0;

        // Atualiza a barra de vida
        if (vidaSlider != null)
        {
            vidaSlider.value = vida; // Atualiza o valor da barra de vida
        }

        Debug.Log($"Boss Final recebeu {dano} de dano. Vida restante: {vida}");

        // Verifica se a vida está na metade para aumentar a dificuldade
        if (vida <= vidaSlider.maxValue / 2 && vida > 0)
        {
            chaseSpeed = originalSpeed * 1.5f; // Aumenta a velocidade em 50%
            attackInterval = attackInterval / 2; // Reduz o intervalo de ataques pela metade
            Debug.Log("Boss está mais rápido e atacando com mais frequência!");
        }

        // Verifica se o Boss morreu
        if (vida <= 0)
        {
            Morrer();
        }
        else
        {
            animator.SetTrigger("Hit"); // Animação de dano
            chaseSpeed = 0f; // Zera a velocidade ao ser atingido
        }
    }

    private void Morrer()
    {
        // Marca o Boss como morto
        isDead = true;

        // Zera a velocidade
        chaseSpeed = 0f;

        // Animação de morte
        animator.SetTrigger("Dead");

        // Destrói o Boss após a animação de morte
    }

    

    private void SpawnCircleAttack()
    {
        if (circleAttackPrefab != null && player != null)
        {
            int numberOfAttacks = 2; // Número de CircleAttacks a serem instanciados
            float spawnRadius = 3f; // Raio ao redor do Boss para spawnar os ataques

            for (int i = 0; i < numberOfAttacks; i++)
            {
                // Gera uma posição aleatória dentro do raio definido
                Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = transform.position + randomOffset;

                // Calcula a direção do jogador
                Vector3 directionToPlayer = (player.position - spawnPosition).normalized;

                // Instancia o CircleAttack
                GameObject circleAttack = Instantiate(circleAttackPrefab, spawnPosition, Quaternion.identity);

                // Passa a direção para o script do CircleAttack
                CircleAttack circleAttackScript = circleAttack.GetComponent<CircleAttack>();
                if (circleAttackScript != null)
                {
                    circleAttackScript.SetDirection(directionToPlayer);
                }
            }
        }
        
    }

}
