using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo de la Cámara")]
    public Transform target; // El objeto que la cámara va a mirar (la grúa)

    [Header("Ajustes")]
    public float distancia = 18f; // Qué tan lejos está la cámara
    public float sensibilidadRaton = 3f; // Velocidad al girar

    private float rotacionX = 25f; // Ángulo vertical inicial
    private float rotacionY = 45f; // Ángulo horizontal inicial

    void Update()
    {
        // Solo rotaremos la cámara si mantenemos presionado el clic derecho (botón 1)
        if (Input.GetMouseButton(1))
        {
            rotacionY += Input.GetAxis("Mouse X") * sensibilidadRaton;
            // Restamos en Y para que no se invierta el control vertical
            rotacionX -= Input.GetAxis("Mouse Y") * sensibilidadRaton;

            // Limitamos la rotación vertical para que la cámara no dé volteretas por debajo del suelo o por encima del cenit
            rotacionX = Mathf.Clamp(rotacionX, 5f, 80f);
        }
    }

    // LateUpdate se ejecuta después de que todos los demás objetos se han movido,
    // ideal para cámaras, evitando tirones visuales.
    void LateUpdate()
    {
        if (target != null)
        {
            // Calculamos la nueva rotación basada en los movimientos del ratón
            Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);

            // Calculamos la posición manteniendo la distancia respecto al target
            Vector3 posicion = target.position - (rotacion * Vector3.forward * distancia);

            // Añadimos un pequeño desfase hacia arriba para que la cámara mire el centro de la grúa y no el suelo
            Vector3 targetCentro = target.position + (Vector3.up * 5f);

            transform.position = posicion;
            transform.LookAt(targetCentro);
        }
    }
}