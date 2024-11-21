using UnityEngine;

public class CartaColetavel : MonoBehaviour
{
    public GameObject cartaSprite; // Referência à sprite da carta no menu de pausa
    public int cartaIndex; // Índice da carta no menu de pausa

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (cartaSprite != null)
            {
                cartaSprite.SetActive(true);
            }

            // Habilita a carta no menu de pausa
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu != null)
            {
                // Exibe o menu de pausa para garantir que a carta será visível
                pauseMenu.ShowPauseMenu();
                pauseMenu.EnableCartaSprite(cartaIndex);
            }

            // Destrói a carta coletável
            Destroy(gameObject);
        }
    }
}