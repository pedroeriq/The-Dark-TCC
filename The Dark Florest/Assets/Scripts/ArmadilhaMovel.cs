using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArmadilhaMovel : MonoBehaviour
{
    public Vector2 intervalo;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        
    }
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(intervalo.x, intervalo.y));
        anim.enabled = true;
    }
}