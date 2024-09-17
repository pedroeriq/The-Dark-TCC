using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObserver : MonoBehaviour
{
    public static event System.Action CoinCollected; // Declara um evento estático

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void TriggerCoinCollected()
    {
        CoinCollected?.Invoke(); // Invoca o evento se houver assinantes
    }
}