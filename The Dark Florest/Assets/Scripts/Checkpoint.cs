using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Collider2D checkpointCollider;

    private void Awake()
    {
        checkpointCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPointObserver.TriggerCheckpoint(transform.position);
            checkpointCollider.enabled = false;
        }
    }
}