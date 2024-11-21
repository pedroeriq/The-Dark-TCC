using UnityEngine;

public class BatteryController : MonoBehaviour
{
    private FlashlightController flashlightController; // Referência ao controlador da lanterna
    public float additionalTime; // Tempo extra em segundos que a pilha fornece

    void Start()
    {
        // Busca o controlador da lanterna na cena
        flashlightController = FindObjectOfType<FlashlightController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Notifica o controlador da lanterna e passa o tempo adicional
            flashlightController.CollectBattery();
            Destroy(gameObject); // Destrói a pilha
        }
    }
}