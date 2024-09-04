using Unity.VisualScripting;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public int nextLevelIndex; // Índice da próxima fase

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<LevelManager>().LoadLevelByIndex(nextLevelIndex);
        }
    }
    
}