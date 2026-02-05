using UnityEngine;

public class CarTurntableDrag : MonoBehaviour
{
    [Header("Rotación automática 360°")]
    [SerializeField] private float autoRotationSpeed = 30f; // grados por segundo

    [Header("Arrastre con mouse")]
    [SerializeField] private float dragDegreesPerPixel = 0.5f;
    [SerializeField] private float smooth = 12f;

    float targetYaw; // Ángulo en el eje Y (rotación horizontal)
    bool dragging;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) dragging = true;
        if (Input.GetMouseButtonUp(0)) dragging = false;

        if (dragging)
        {
            // Arrastrar con el mouse horizontalmente
            float dx = Input.GetAxis("Mouse X");
            targetYaw += dx * dragDegreesPerPixel;
            // Sin límites para permitir rotación completa
        }
        else
        {
            // Rotación automática continua de 360 grados en el eje Y
            targetYaw += Time.deltaTime * autoRotationSpeed;
            // Mantener el ángulo en el rango de 0-360 para evitar valores muy grandes
            if (targetYaw >= 360f) targetYaw -= 360f;
        }

        var desired = Quaternion.Euler(0f, targetYaw, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, desired, Time.deltaTime * smooth);
    }
}
