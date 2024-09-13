using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public int moeda = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Faz o objeto persistir entre cenas
        }
        else
        {
            Destroy(gameObject); // Garante que não haja mais de uma instância
        }
    }

    public void AddMoeda(int amount)
    {
        moeda += amount;
    }

    public int GetMoeda()
    {
        return moeda;
    }
}