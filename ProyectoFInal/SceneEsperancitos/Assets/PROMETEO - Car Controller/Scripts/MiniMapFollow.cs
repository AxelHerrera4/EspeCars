using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;   // El jugador
    public float height = 80f; // Altura fija

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPosition = target.position;
        newPosition.y = height;

        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
