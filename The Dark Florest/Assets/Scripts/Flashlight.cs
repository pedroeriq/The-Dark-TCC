using UnityEngine;
using TMPro;

public class FlashlightController : MonoBehaviour
{
    public GameObject flashlightPrefab; // Prefab da lanterna
    public float flashlightDuration = 30f; // Duração total da lanterna
    public Vector3 offset = new Vector3(0, 2.6f, 0); // Offset padrão da posição relativa ao Player
    public Vector3 runOffset; // Offset ao correr para a direita
    public Vector3 runOffsetLeft; // Offset ao correr para a esquerda
    public Vector3 jumpOffset; // Offset durante o pulo
    public TMP_Text timerText; // Texto para exibir o tempo restante da lanterna
    public int maxUses = 3; // Número máximo de vezes que a lanterna pode ser usada

    private GameObject currentFlashlight; // Referência à lanterna instanciada
    private Transform playerTransform; // Referência ao Transform do Player
    private Animator playerAnimator; // Referência ao Animator do jogador
    private float remainingDuration; // Duração restante da lanterna
    private bool isFlashlightActive; // Estado da lanterna
    private int remainingUses; // Contador de usos restantes
    private bool hasBattery; // Indica se o jogador coletou a pilha

    void Start()
    {
        // Busca o objeto do jogador pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerAnimator = player.GetComponent<Animator>(); // Obtém o Animator do jogador
        }
        else
        {
            Debug.LogError("Player com a tag 'Player' não encontrado na cena.");
        }

        // Inicializa o texto do temporizador
        UpdateTimerText();
        remainingUses = maxUses; // Inicializa o número de usos
        hasBattery = false; // Inicializa como não tendo pilha
    }

    void Update()
    {
        // Verifica se a tecla X foi pressionada
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentFlashlight == null && remainingUses > 0 && hasBattery) // Verifica se tem pilha
            {
                // Instancia a lanterna se não houver uma ativa e ainda houver usos
                InstantiateFlashlight();
            }
            else
            {
                // Alterna a lanterna
                ToggleFlashlight();
            }
        }
    }

    private void InstantiateFlashlight()
    {
        // Instancia a lanterna e ajusta sua rotação para apontar para baixo
        currentFlashlight = Instantiate(flashlightPrefab, playerTransform.position + offset, Quaternion.Euler(90f, 0f, 0f));
        remainingDuration = flashlightDuration; // Define a duração inicial
        isFlashlightActive = true; // Define a lanterna como ativa
        StartCoroutine(HandleFlashlightDuration());
    }

    private System.Collections.IEnumerator HandleFlashlightDuration()
    {
        while (remainingDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingDuration--; // Decrementa a duração restante a cada segundo
            UpdateTimerText(); // Atualiza o texto do temporizador
        }

        // Destrói a lanterna quando o tempo acabar e desconta uma vida
        DestroyFlashlight();
        remainingUses--; // Desconta uma vida ao destruir a lanterna
    }

    private void ToggleFlashlight()
    {
        if (isFlashlightActive)
        {
            // Desativa a lanterna
            isFlashlightActive = false;
            StopAllCoroutines(); // Para a contagem do tempo, mas não altera remainingDuration
            UpdateTimerText(); // Atualiza o texto do temporizador
            currentFlashlight.SetActive(false); // Desativa a lanterna na cena
        }
        else
        {
            // Ativa a lanterna, se ainda houver usos
            if (remainingUses > 0)
            {
                isFlashlightActive = true;
                currentFlashlight.SetActive(true); // Reativa a lanterna na cena
                StartCoroutine(HandleFlashlightDuration()); // Continua o tempo da lanterna
            }
        }
    }

    private void DestroyFlashlight()
    {
        if (currentFlashlight != null)
        {
            Destroy(currentFlashlight);
            currentFlashlight = null;
        }

        // Reseta o texto do temporizador
        timerText.text = "0";
    }

    void UpdateTimerText()
    {
        // Atualiza o texto do temporizador na UI
        if (timerText != null)
        {
            timerText.text = remainingDuration.ToString();
        }
    }

    void FixedUpdate()
    {
        if (currentFlashlight != null && isFlashlightActive)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        if (currentFlashlight != null)
        {
            Vector3 currentOffset;

            // Verifica se a animação atual é "Player Run" para ajustar o offset
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player Run"))
            {
                // Se a velocidade horizontal do jogador for maior que 0, significa que ele está indo para a direita
                if (playerTransform.GetComponent<Rigidbody2D>().velocity.x < 0) // Correndo para a esquerda
                {
                    currentOffset = runOffsetLeft;
                }
                else // Correndo para a direita
                {
                    currentOffset = runOffset;
                }
            }
            // Verifica se o jogador está pulando
            else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player Jump"))
            {
                currentOffset = jumpOffset; // Offset durante o pulo
            }
            else
            {
                currentOffset = offset; // Offset padrão quando não está correndo ou pulando
            }

            currentFlashlight.transform.position = playerTransform.position + currentOffset;
            currentFlashlight.transform.rotation = playerTransform.rotation * Quaternion.Euler(0f, 0f, 338f);
        }
    }

    public void CollectBattery()
    {
        hasBattery = true; // O jogador coletou a pilha
    }

    public void SetFlashlightOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
