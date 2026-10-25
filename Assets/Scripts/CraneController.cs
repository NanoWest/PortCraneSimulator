using UnityEngine;

public class CraneController : MonoBehaviour
{
    [Header("Partes Móviles")]
    public Transform carro;
    public Transform gancho;
    public Transform cableVisual;

    [Header("Ajustes de Velocidad y Detección")]
    public float velocidadCarro = 5f;
    public float velocidadGancho = 4f;

    [Header("Físicas del Cable (Clima)")]
    [Tooltip("Vector de fuerza del viento. Puede leerlo desde su WeatherManager.")]
    public Vector3 fuerzaVientoGlobal = new Vector3(2f, 0, 0);
    [Tooltip("Qué tanto afecta el viento al peso del gancho.")]
    public float sensibilidadViento = 0.05f;
    [Tooltip("Tiempo que tarda en estabilizarse (simula el peso de la carga).")]
    public float amortiguacionPendulo = 2f;

    private Vector3 offsetBalanceo;
    private Vector3 velocidadOscilacion;

    [Tooltip("Distancia del sensor hacia abajo.")]
    public float distanciaDeteccion = 0.3f;
    [Tooltip("Distancia del sensor hacia los lados.")]
    public float distanciaDeteccionHorizontal = 1.5f;
    [Tooltip("Distancia del sensor volumétrico hacia arriba.")]
    public float distanciaDeteccionArriba = 0.1f;

    [Header("Límites de Movimiento")]
    public float limiteCarroMin = -7.5f;
    public float limiteCarroMax = 7.5f;
    public float limiteGanchoMin = -8.5f;
    public float limiteGanchoMax = -1f;

    [Header("Configuración de Grúa (Base sobre Rieles)")]
    public float velocidadGrua = 3f;
    public float limiteGruaMin = -20f;
    public float limiteGruaMax = 20f;

    void Update()
    {
        MoverCarro();
        MoverGancho();
        MoverGruaBase();
    }

    private void MoverCarro()
    {
        float inputX = 0f;
        if (Input.GetKey(KeyCode.D)) inputX = 1f;
        if (Input.GetKey(KeyCode.A)) inputX = -1f;

        if (inputX != 0)
        {
            Vector3 direccion = inputX > 0 ? Vector3.right : Vector3.left;

            // CORRECCIÓN: El sensor lateral nace del centro exacto del gancho
            Vector3 origenSensor = gancho.position;
            Vector3 tamañoSensorLateral = new Vector3(0.1f, 0.4f, 0.9f);

            if (Physics.BoxCast(origenSensor, tamañoSensorLateral, direccion, out RaycastHit hit, gancho.rotation, distanciaDeteccionHorizontal, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.transform.root != gancho.root)
                {
                    Rigidbody rbObstaculo = hit.collider.attachedRigidbody;
                    HookController hook = gancho.GetComponent<HookController>();
                    bool esCargaEnganchada = (hook != null && hook.cargaEnganchada == hit.collider.gameObject);

                    if (rbObstaculo != null && !rbObstaculo.isKinematic && !esCargaEnganchada)
                    {
                        Vector3 nuevaVelocidad = rbObstaculo.linearVelocity;
                        nuevaVelocidad.x = direccion.x * velocidadCarro;
                        rbObstaculo.linearVelocity = nuevaVelocidad;
                    }
                    inputX = 0f;
                }
            }
        }

        if (inputX != 0)
        {
            Vector3 nuevaPos = carro.localPosition + Vector3.right * inputX * velocidadCarro * Time.deltaTime;
            nuevaPos.x = Mathf.Clamp(nuevaPos.x, limiteCarroMin, limiteCarroMax);
            carro.localPosition = nuevaPos;
        }
    }

    private void MoverGancho()
    {
        float inputY = 0f;
        if (Input.GetKey(KeyCode.W)) inputY = 1f;
        if (Input.GetKey(KeyCode.S)) inputY = -1f;

        if (inputY < 0)
        {
            Vector3 origenSensor = gancho.position + (Vector3.up * 0.5f);
            HookController hook = gancho.GetComponent<HookController>();
            bool llevaCarga = (hook != null && hook.cargaEnganchada != null);
            float distanciaTotal = llevaCarga ? (distanciaDeteccion + 0.5f + 2.5f) : (distanciaDeteccion + 0.5f);
            Vector3 tamañoSensorVertical = new Vector3(0.9f, 0.1f, 0.9f);

            RaycastHit[] hits = Physics.BoxCastAll(origenSensor, tamañoSensorVertical, Vector3.down, gancho.rotation, distanciaTotal, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            bool obstaculoDetectado = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform == gancho) continue;
                if (hit.collider.transform.root == gancho.root) continue;
                if (llevaCarga && hit.collider.gameObject == hook.cargaEnganchada) continue;

                obstaculoDetectado = true;
                break;
            }

            if (obstaculoDetectado) inputY = 0f;
        }
        else if (inputY > 0)
        {
            Vector3 origenSensor = gancho.position;
            Vector3 tamañoSensorVertical = new Vector3(0.9f, 0.1f, 0.9f);

            RaycastHit[] hits = Physics.BoxCastAll(origenSensor, tamañoSensorVertical, Vector3.up, gancho.rotation, distanciaDeteccionArriba, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            bool obstaculoDetectado = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform == gancho) continue;
                if (hit.collider.transform.root == gancho.root) continue;

                HookController hook = gancho.GetComponent<HookController>();
                bool esCargaEnganchada = (hook != null && hook.cargaEnganchada != null && hit.collider.gameObject == hook.cargaEnganchada);
                if (esCargaEnganchada) continue;

                // NUEVO: Transferencia de energía cinética hacia arriba
                Rigidbody rbObstaculo = hit.collider.attachedRigidbody;
                if (rbObstaculo != null && !rbObstaculo.isKinematic)
                {
                    Vector3 nuevaVelocidad = rbObstaculo.linearVelocity;
                    // Inyectamos la velocidad de ascenso del gancho directamente al contenedor
                    nuevaVelocidad.y = velocidadGancho;
                    rbObstaculo.linearVelocity = nuevaVelocidad;
                }

                obstaculoDetectado = true;
                break;
            }

            // Pausamos el modelo del gancho para no atravesar la caja mientras la empujamos
            if (obstaculoDetectado) inputY = 0f;
        }

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
        if (cableVisual != null)
        {
            float distancia = Mathf.Abs(gancho.localPosition.y);
            cableVisual.localScale = new Vector3(0.1f, distancia / 2f, 0.1f);
            cableVisual.localPosition = new Vector3(0f, gancho.localPosition.y / 2f, 0f);
        }
    }

    private void MoverGruaBase()
    {
        float inputZ = 0f;
        // Usamos las flechas direccionales para mover toda la estructura
        if (Input.GetKey(KeyCode.UpArrow)) inputZ = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) inputZ = -1f;

        if (inputZ != 0)
        {
            // Movemos el objeto raíz (la grúa completa) en el eje Z (forward/back)
            Vector3 nuevaPos = transform.position + Vector3.forward * inputZ * velocidadGrua * Time.deltaTime;
            // Limitamos el recorrido de los rieles
            nuevaPos.z = Mathf.Clamp(nuevaPos.z, limiteGruaMin, limiteGruaMax);
            transform.position = nuevaPos;
        }
    }
}