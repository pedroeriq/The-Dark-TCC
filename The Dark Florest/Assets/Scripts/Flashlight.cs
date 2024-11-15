using UnityEngine;
using TMPro;

public class FlashlightController : MonoBehaviour
{
    public GameObject flashlightPrefab; // Prefab da lanterna para a direita
    public GameObject flashlightLeftPrefab; // Prefab da lanterna para a esquerda
    public float flashlightDuration = 30f; // Duração total da lanterna
    public TMP_Text timerText; // Texto para exibir o tempo restante da lanterna
    public int maxUses = 3; // Número máximo de vezes que a lanterna pode ser usada

    public Vector3 flashlightOffsetRight; // Offset para a posição da lanterna quando o jogador olha para a direita
    public Vector3 flashlightOffsetLeft; // Offset para a posição da lanterna quando o jogador olha para a esquerda

    private GameObject currentFlashlight; // Referência à lanterna instanciada
    private Transform playerTransform; // Referência ao Transform do Player
    private Animator playerAnimator; // Referência ao Animator do jogador
    private float remainingDuration; // Duração restante da lanterna
    private bool isFlashlightActive; // Estado da lanterna
    private int remainingUses; // Contador de usos restantes
    private bool hasBattery; // Indica se o jogador coletou a pilha
    private Rigidbody2D playerRigidbody; // Referência ao Rigidbody2D do Player
    private bool isFacingRight = true; // Direção visual do jogador (inicialmente para a direita)

    void Start()
    {
        // Busca o objeto do jogador pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerAnimator = player.GetComponent<Animator>(); // Obtém o Animator do jogador
            playerRigidbody = player.GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D do jogador
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
        // Verifica a direção do jogador e instancia a lanterna corretamente
        if (isFacingRight)
        {
            // Instancia a lanterna para a direita e ajusta o offset
            currentFlashlight = Instantiate(flashlightPrefab, playerTransform.position + flashlightOffsetRight, Quaternion.identity);
        }
        else
        {
            // Instancia a lanterna para a esquerda e ajusta o offset
            currentFlashlight = Instantiate(flashlightLeftPrefab, playerTransform.position + flashlightOffsetLeft, Quaternion.Euler(0f, 180f, 0f));
        }

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

        // Verifica a direção visual do jogador
        if (playerRigidbody.velocity.x > 0)
        {
            isFacingRight = true; // Direita
        }
        else if (playerRigidbody.velocity.x < 0)
        {
            isFacingRight = false; // Esquerda
        }
    }

    void FollowPlayer()
    {
        if (currentFlashlight != null)
        {
            // Atualiza a posição da lanterna para seguir o jogador
            if (isFacingRight)
            {
                currentFlashlight.transform.position = playerTransform.position + flashlightOffsetRight;
                currentFlashlight.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Lanterna para a direita
            }
            else
            {
                currentFlashlight.transform.position = playerTransform.position + flashlightOffsetLeft;
                currentFlashlight.transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Lanterna para a esquerda
            }
        }
    }

    public void CollectBattery()
    {
        hasBattery = true; // O jogador coletou a pilha
    }
}
