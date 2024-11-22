using UnityEngine;

public class CartaColetavel : MonoBehaviour
{
    public GameObject cartaSprite; // Referência à sprite da carta no menu de pausa
    public int cartaIndex; // Índice da carta no menu de pausa
    public AudioSource audioSource; // Referência ao componente de áudio

    private void Start()
    {
        // Certifica-se de que o áudio está configurado corretamente
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1.0f; // Configura para som 3D
            audioSource.loop = true; // Faz o som tocar em loop
            audioSource.Play(); // Inicia o som
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Habilita a carta no menu de pausa
            if (cartaSprite != null)
            {
                cartaSprite.SetActive(true);
            }

            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.ShowPauseMenu();
                pauseMenu.EnableCartaSprite(cartaIndex);
            }

            // Para o som e destrói o objeto
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            Destroy(gameObject);
        }
    }
}
