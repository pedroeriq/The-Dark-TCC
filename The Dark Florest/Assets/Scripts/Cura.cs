using UnityEngine;

public class Cura : MonoBehaviour
{
    public int valorCura = 2; // Quantidade de vida que será restaurada

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Verifica se o objeto que colidiu é o Player
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();

            // Verifica se o Player tem o componente de vida e aplica a cura
            if (player != null)
            {
                player.Curar(valorCura);
                Destroy(gameObject); // Destrói o objeto de cura após a colisão
            }
        }
    }
}