using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo de la Cámara")]
    public Transform target; // La grúa base

    [Header("Ajustes")]
    public float sensibilidadRaton = 3f;
    public float alturaObjetivo = 10f; // Ajuste este valor si quiere que el pivote de rotación esté más arriba

    private float distancia;
    private float rotacionX;
    private float rotacionY;

    void Start()
    {
        if (target != null)
        {
            // 1. Lee exactamente la rotación que usted configuró en el Inspector
            Vector3 angulos = transform.eulerAngles;
            rotacionX = angulos.x;
            rotacionY = angulos.y;

            // 2. Calcula la distancia real de la cámara respecto al centro del objetivo
            Vector3 pivote = target.position + (Vector3.up * alturaObjetivo);
            distancia = Vector3.Distance(transform.position, pivote);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotacionY += Input.GetAxis("Mouse X") * sensibilidadRaton;
            rotacionX -= Input.GetAxis("Mouse Y") * sensibilidadRaton;
            rotacionX = Mathf.Clamp(rotacionX, 5f, 85f);
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);
            Vector3 pivote = target.position + (Vector3.up * alturaObjetivo);

            // Mantiene la posición basada estrictamente en su rotación inicial
            Vector3 posicion = pivote - (rotacion * Vector3.forward * distancia);

            transform.position = posicion;
            transform.rotation = rotacion; // Reemplazamos LookAt por rotación directa para evitar saltos
        }
    }
}