using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SombraErrante : MonoBehaviour
{
    public float distancia;
    public float speed;
    public Transform playerPos;
    public Rigidbody2D flyRB; // Corrigido de Rigibody2D para Rigidbody2D

    // Start is called before the first frame update
    void Start()
    {
        // Inicializações se necessário
    }

    // Update is called once per frame
    void Update()
    {
        distancia = Vector2.Distance(transform.position, playerPos.position);
        if(distancia < 4)
        {
            Seguir();
        }
    }

    private void Seguir()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
    }
}
