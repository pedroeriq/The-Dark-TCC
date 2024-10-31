using UnityEngine;

public class CartaColetavel : MonoBehaviour
{
    public GameObject cartaSprite; // Referência à sprite da carta no menu de pausa
    public int cartaIndex; // Índice da carta no menu de pausa

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que colidiu é o Player
        if (collision.CompareTag("Player"))
        {
            // Ativa a sprite da carta no menu de pausa
            if (cartaSprite != null)
            {
                cartaSprite.SetActive(true);
            }

            // Habilita a carta no menu de pausa
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.EnableCartaSprite(cartaIndex);
            }

            // Destrói a carta coletável ao colidir com o Player
            Destroy(gameObject);
        }
    }
}