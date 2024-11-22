using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    public Vector3 lastCheckpointPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Inscreve-se no evento para capturar a posição do checkpoint
        CheckPointObserver.OnCheckpointTriggerEvent += SaveCheckpointPosition;
    }

    private void OnDisable()
    {
        CheckPointObserver.OnCheckpointTriggerEvent -= SaveCheckpointPosition;
    }

    private void SaveCheckpointPosition(Vector3 checkpointPosition)
    {
        lastCheckpointPosition = checkpointPosition;
        Debug.Log("Checkpoint salvo na posição: " + lastCheckpointPosition);
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition;
    }

    // Função para respawn no último checkpoint salvo
    public void RespawnPlayer(GameObject player)
    {
        if (lastCheckpointPosition != null)
        {
            player.transform.position = lastCheckpointPosition;  // Move o jogador para o checkpoint
            Debug.Log("Jogador respawnado na posição: " + lastCheckpointPosition);
        }
        else
        {
            Debug.Log("Nenhum checkpoint foi salvo!");
        }
    }
}
