using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TocarMusica : MonoBehaviour
{
    [SerializeField] private AudioSource grito;
    [SerializeField] private AudioSource Musica;
    private BoxCollider2D box;
    private Enemy2 morreu;

    public bool Estartocando;
    // Start is called before the first frame update
    
    void Update()
    {
        morreuu();
        if (Estartocando == true)
        {
            grito.Play();   
            Musica.Play();
            
        }
        else
        {
            grito.Stop();   
            Musica.Stop();
        }
    }
    
    void Start()
    {
        morreu = GetComponent<Enemy2>();
    }

    public void morreuu()
    {
        if (morreu.vida <= 0)
        {
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")

        {
            Estartocando = true;
            box.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    // Update is called once per frame
    
}
