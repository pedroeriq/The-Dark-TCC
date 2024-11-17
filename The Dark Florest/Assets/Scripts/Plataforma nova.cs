using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataformanova : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Define a plataforma como pai do jogador para que ele acompanhe o movimento
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D (Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Remove o jogador como filho da plataforma ao sair
            other.transform.SetParent(null);
        }
    }
}
