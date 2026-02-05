using UnityEngine;

public class ConstructorDeMuros : MonoBehaviour
{
    public Transform rutaPadre; // Arrastra aquí tu "Ruta_IA"
    public float alturaMuro = 3f; // Qué tan altos quieres los muros
    public float anchoMuro = 1f;  // Qué tan gordos (para que no los atraviesen)

    void Start()
    {
        if (rutaPadre == null) return;

        // Recorremos todos los puntos
        for (int i = 0; i < rutaPadre.childCount; i++)
        {
            // 1. Tomamos el punto A y el punto B (el siguiente)
            Vector3 puntoA = rutaPadre.GetChild(i).position;
            // El % hace que el último punto se una con el primero (cerrar circuito)
            Vector3 puntoB = rutaPadre.GetChild((i + 1) % rutaPadre.childCount).position;

            // 2. Creamos un muro (Cubo)
            GameObject muro = GameObject.CreatePrimitive(PrimitiveType.Cube);
            muro.name = "Muro_Auto_" + i;

            // Lo ponemos hijo de este objeto para no ensuciar la jerarquía
            muro.transform.parent = this.transform;

            // 3. Posición: Justo en el centro entre los dos puntos
            muro.transform.position = (puntoA + puntoB) / 2;

            // 4. Rotación: Que mire hacia el siguiente punto
            muro.transform.LookAt(puntoB);

            // 5. Tamaño: 
            // - Ancho: El que elegimos
            // - Alto: El que elegimos
            // - Largo (Z): La distancia exacta entre los dos puntos
            float distancia = Vector3.Distance(puntoA, puntoB);
            muro.transform.localScale = new Vector3(anchoMuro, alturaMuro, distancia);

            // 6. ¡Hacerlo Invisible!
            // Destruimos el componente que lo hace visible (MeshRenderer)
            // pero dejamos el Collider (la física)
            Destroy(muro.GetComponent<MeshRenderer>());
        }
    }
}