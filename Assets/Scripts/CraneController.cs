using UnityEngine;

public class CraneController : MonoBehaviour
{
    [Header("Partes Móviles")]
    public Transform carro;
    public Transform gancho;
    public Transform cableVisual;

    [Header("Ajustes de Velocidad")]
    public float velocidadCarro = 5f;
    public float velocidadGancho = 4f;

    [Header("Límites de Movimiento")]
    public float limiteCarroMin = -7.5f;
    public float limiteCarroMax = 7.5f;
    public float limiteGanchoMin = -8.5f; // Lo más bajo (cerca del suelo)
    public float limiteGanchoMax = -1f;   // Lo más alto (pegado al carro)

    void Update()
    {
        MoverCarro();
        MoverGancho();
    }

    private void MoverCarro()
    {
        float inputX = 0f;
        if (Input.GetKey(KeyCode.D)) inputX = 1f;
        if (Input.GetKey(KeyCode.A)) inputX = -1f;

        if (inputX != 0)
        {
            // Time.deltaTime hace que el movimiento sea fluido sin importar los FPS
            Vector3 nuevaPos = carro.localPosition + Vector3.right * inputX * velocidadCarro * Time.deltaTime;

            // Mathf.Clamp impide que el valor se pase de los límites mínimo y máximo
            nuevaPos.x = Mathf.Clamp(nuevaPos.x, limiteCarroMin, limiteCarroMax);
            carro.localPosition = nuevaPos;
        }
    }

    private void MoverGancho()
    {
        float inputY = 0f;
        if (Input.GetKey(KeyCode.W)) inputY = 1f;
        if (Input.GetKey(KeyCode.S)) inputY = -1f;

        if (inputY != 0)
        {
            Vector3 nuevaPos = gancho.localPosition + Vector3.up * inputY * velocidadGancho * Time.deltaTime;

            nuevaPos.y = Mathf.Clamp(nuevaPos.y, limiteGanchoMin, limiteGanchoMax);
            gancho.localPosition = nuevaPos;

            ActualizarCable();
        }
    }

    private void ActualizarCable()
    {
        // Ajustamos dinámicamente el tamaño y posición del cilindro visual
        if (cableVisual != null)
        {
            float distancia = Mathf.Abs(gancho.localPosition.y);
            // La escala Y primitiva de Unity es 2, por lo que dividimos la distancia entre 2
            cableVisual.localScale = new Vector3(0.1f, distancia / 2f, 0.1f);
            // El centro del cable debe estar a la mitad del recorrido del gancho
            cableVisual.localPosition = new Vector3(0f, gancho.localPosition.y / 2f, 0f);
        }
    }
}