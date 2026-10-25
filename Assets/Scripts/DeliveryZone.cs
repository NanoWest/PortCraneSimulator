using UnityEngine;
using System.Collections;

public class DeliveryZone : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Color colorMisionCumplida = new Color(0f, 1f, 0f, 0.5f);
    private Color colorOriginal;
    private Material materialZona;
    private bool zonaCompletada = false;

    [Header("Reubicación Aleatoria")]
    [Tooltip("Arrastre aquí su objeto Plataforma_Entrega desde la Jerarquía")]
    public Collider plataformaSuelo;

    [Header("Precisión de Entrega")]
    [Tooltip("Porcentaje mínimo de la caja que debe estar dentro (0.6 = 60%)")]
    [Range(0.1f, 1f)]
    public float porcentajeRequerido = 0.6f;

    private Collider miCollider;

    void Start()
    {
        miCollider = GetComponent<Collider>();
        Renderer render = GetComponent<Renderer>();
        if (render != null)
        {
            materialZona = render.material;
            colorOriginal = materialZona.color;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (zonaCompletada) return;

        if (other.CompareTag("Carga"))
        {
            Rigidbody rbCaja = other.attachedRigidbody;

            // 1. Validamos que la caja esté detenida
            if (rbCaja != null && rbCaja.linearVelocity.magnitude < 0.1f)
            {
                // 2. Validamos que el porcentaje dentro de la zona sea aceptable
                if (CalcularPorcentajeDentro(other) >= porcentajeRequerido)
                {
                    StartCoroutine(ProcesarEntregaYReubicar());
                }
            }
        }
    }

    private float CalcularPorcentajeDentro(Collider caja)
    {
        Bounds boundsZona = miCollider.bounds;
        Bounds boundsCaja = caja.bounds;

        // Encontramos las coordenadas de la intersección entre ambas cajas
        Vector3 minOverlap = Vector3.Max(boundsZona.min, boundsCaja.min);
        Vector3 maxOverlap = Vector3.Min(boundsZona.max, boundsCaja.max);

        // Validamos si hay una intersección física real
        if (minOverlap.x < maxOverlap.x && minOverlap.y < maxOverlap.y && minOverlap.z < maxOverlap.z)
        {
            // Calculamos el volumen del pedazo de la caja que está adentro
            float volumenInterseccion = (maxOverlap.x - minOverlap.x) * (maxOverlap.y - minOverlap.y) * (maxOverlap.z - minOverlap.z);

            // Calculamos el volumen total de la caja
            float volumenTotalCaja = boundsCaja.size.x * boundsCaja.size.y * boundsCaja.size.z;

            // Retornamos la proporción (ej. 0.65 = 65%)
            return volumenInterseccion / volumenTotalCaja;
        }

        return 0f;
    }

    private IEnumerator ProcesarEntregaYReubicar()
    {
        zonaCompletada = true;

        if (materialZona != null) materialZona.color = colorMisionCumplida;
        Debug.Log("¡ENTREGA EXITOSA! Volumen de carga aceptado.");

        yield return new WaitForSeconds(2f);

        if (plataformaSuelo != null)
        {
            Bounds limites = plataformaSuelo.bounds;
            // Restamos un margen mayor (3 unidades) para evitar que la nueva zona se genere en el puro borde
            float randomX = Random.Range(limites.min.x + 3f, limites.max.x - 3f);
            float randomZ = Random.Range(limites.min.z + 3f, limites.max.z - 3f);

            transform.position = new Vector3(randomX, transform.position.y, randomZ);
        }
        else
        {
            Debug.LogWarning("Falta asignar la Plataforma de Entrega en el Inspector.");
        }

        if (materialZona != null) materialZona.color = colorOriginal;
        zonaCompletada = false;
    }
}