using UnityEngine;

public class CartaUI : MonoBehaviour
{
    public GameObject[] cartasGrandesUI; // Array de painéis para cada carta grande

    private void Start()
    {
        // Garante que todos os painéis de cartas grandes comecem desativados
        foreach (GameObject carta in cartasGrandesUI)
        {
            carta.SetActive(false);
        }
    }

    public void ShowCarta(int cartaIndex)
    {
        if (cartaIndex >= 0 && cartaIndex < cartasGrandesUI.Length)
        {
            cartasGrandesUI[cartaIndex].SetActive(true); // Ativa o painel da carta específica
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Fecha todos os painéis de cartas grandes ao pressionar ESC
            foreach (GameObject carta in cartasGrandesUI)
            {
                carta.SetActive(false);
            }
            GameController.instance.PauseGame(); // Retorna ao menu de pausa
        }
    }
}