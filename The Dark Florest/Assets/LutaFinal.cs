using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para gerenciar cenas

public class LutaFinal : MonoBehaviour
{
    public void MenuScene()
    {
        SceneManager.LoadScene("FINALBOSS"); // Coloque o nome da cena do seu menu
    }
}
