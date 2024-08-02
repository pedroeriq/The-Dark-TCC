using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Transform player; // Referência ao Transform do Player
    public Vector3 offset;   // Offset da posição relativa ao Player
    public GameObject flashlightPrefab; // Prefab da lanterna
    public float flashlightDuration = 5f; // Duração da lanterna no jogo

    private GameObject currentFlashlight; // Referência à lanterna instanciada

    void Update()
    {
        // Verifica se a tecla X foi pressionada e se não há uma lanterna existente
        if (Input.GetKeyDown(KeyCode.X) && currentFlashlight == null)
        {
            // Instancia a lanterna
            currentFlashlight = Instantiate(flashlightPrefab, player.position + offset, Quaternion.identity);
            StartCoroutine(DestroyFlashlightAfterTime(flashlightDuration));
        }

        // Atualiza a lanterna se ela estiver ativa
        if (currentFlashlight != null)
        {
            FollowPlayer();
        }
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

    void FollowPlayer()
    {
        // Atualiza a posição da lanterna para acompanhar o Player com um offset
        if (currentFlashlight != null)
        {
            currentFlashlight.transform.position = player.position + offset;
        }
    }
}