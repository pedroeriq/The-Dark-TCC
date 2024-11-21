using UnityEngine;
using UnityEngine.SceneManagement; // Para gerenciar cenas
using UnityEngine.UI; // Para UI

public class Opções : MonoBehaviour
{
    // Método para carregar a cena do menu
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
}