using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referência ao Transform do jogador
    public float smoothSpeed = 0.125f; // Velocidade de suavização do movimento
    public Vector3 offset; // Offset da câmera em relação ao jogador

    void LateUpdate()
    {
        // Calcula a posição desejada da câmera
        Vector3 desiredPosition = player.position + offset;
        // Define a posição suavizada da câmera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Atualiza a posição da câmera
        transform.position = smoothedPosition;
    }
}