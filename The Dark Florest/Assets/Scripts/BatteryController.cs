using UnityEngine;

public class BatteryController : MonoBehaviour
{
    private FlashlightController flashlightController; // Referência ao controlador da lanterna

    void Start()
    {
        // Busca o controlador da lanterna na cena
        flashlightController = FindObjectOfType<FlashlightController>();
    }

    // Método chamado quando outro Collider2D entra em contato com o Collider2D deste objeto
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto colidido tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Coleta a pilha e notifica o controlador da lanterna
            flashlightController.CollectBattery();
            Destroy(gameObject); // Destrói a pilha
        }
    }
}