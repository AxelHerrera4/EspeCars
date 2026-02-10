using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform carTransform;
    
    [Header("Ajustes de Visión")]
    public float distance = 20.0f;  // Qué tan atrás está
    public float height = 7.0f;     // Qué tan alta está
    
    [Header("Suavizado")]
    [Range(1, 15)]
    public float followSpeed = 5;
    [Range(1, 15)]
    public float lookSpeed = 10;

    void LateUpdate() // Usamos LateUpdate para evitar tirones (jitter)
    {
        if (!carTransform) return;

        // 1. Calculamos la posición deseada DETRÁS del coche (usando su rotación actual)
        // carTransform.up * height nos da la altura
        // carTransform.forward * -distance nos pone detrás
        Vector3 targetPos = carTransform.position + (carTransform.up * height) - (carTransform.forward * distance);

        // 2. Movemos la cámara suavemente a esa posición
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // 3. Hacemos que la cámara mire hacia el coche
        // Tip: carTransform.position + carTransform.forward * 2 hace que mire un poco hacia adelante del coche
        Vector3 lookTarget = carTransform.position + (carTransform.up * 1.5f); 
        Vector3 _lookDirection = lookTarget - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
    }
}