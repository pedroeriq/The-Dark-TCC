using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public GameObject flashlightPrefab; // Prefab da lanterna
    public float flashlightDuration = 5f; // Duração da lanterna no jogo
    public Vector3 offset = new Vector3(0, 2.6f, 0); // Offset da posição relativa ao Player

    private GameObject currentFlashlight; // Referência à lanterna instanciada
    private Transform playerTransform; // Referência ao Transform do Player

    void Start()
    {
        // Busca o objeto do jogador pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player com a tag 'Player' não encontrado na cena.");
        }
    }

    void Update()
    {
        // Verifica se a tecla X foi pressionada e se não há uma lanterna existente
        if (Input.GetKeyDown(KeyCode.X) && currentFlashlight == null)
        {
            // Instancia a lanterna
            InstantiateFlashlight();
        }
    }

    private void InstantiateFlashlight()
    {
        currentFlashlight = Instantiate(flashlightPrefab, playerTransform.position + offset, Quaternion.identity);
        StartCoroutine(DestroyFlashlightAfterTime(flashlightDuration));
    }

    private System.Collections.IEnumerator DestroyFlashlightAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (currentFlashlight != null)
        {
            Destroy(currentFlashlight);
            currentFlashlight = null;
        }
    }

    void FixedUpdate()
    {
        if (currentFlashlight != null)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Atualiza a posição e rotação da lanterna para acompanhar o Player com um offset
        if (currentFlashlight != null)
        {
            currentFlashlight.transform.position = playerTransform.position + offset;
            currentFlashlight.transform.rotation = playerTransform.rotation; // Faz a lanterna acompanhar a rotação do jogador
        }
    }

    public void SetFlashlightOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
