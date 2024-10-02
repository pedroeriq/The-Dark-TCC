using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject prefabFumaca;
    
    private void OnEnable()
    {
        ParticleObserver.ParticleSpawEvent += SpawnarParticulas;
    }
    
    private void OnDisable()
    {
        ParticleObserver.ParticleSpawEvent -= SpawnarParticulas;
    }

    public void SpawnarParticulas(Vector3 posicao)
    {
        Instantiate(prefabFumaca, posicao, Quaternion.identity);
    }
}