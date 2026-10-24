using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ContainerPhysics : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private PhysicsMaterial originalMaterial;
    private PhysicsMaterial dynamicMaterial;

    [Header("Propiedades Físicas (Fase 6)")]
    public float masaKg = 2200f;
    public float dragCoefficient = 2.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = masaKg;
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        // Extraemos el material físico de la caja (ej. Fisica_Pesada)
        col = GetComponent<Collider>();
        if (col.material != null)
        {
            originalMaterial = col.material;
            // Clonamos el material para poder modificarlo sin alterar el archivo original del proyecto
            dynamicMaterial = Instantiate(originalMaterial);
            col.material = dynamicMaterial;
        }
    }

    void FixedUpdate()
    {
        if (WeatherManager.Instance != null && !rb.isKinematic)
        {
            // 1. Aplicar Fuerzas de Viento
            Vector3 windVelocity = WeatherManager.Instance.GetWindVelocity();
            if (windVelocity.magnitude > 0.1f)
            {
                ApplyWindForce(windVelocity);
            }

            // 2. Aplicar Efecto de Lluvia (Fricción Dinámica)
            if (dynamicMaterial != null)
            {
                float rain = WeatherManager.Instance.rainIntensity;

                // Si llueve al máximo (1), la fricción baja al 25% de su valor original. Si no llueve (0), se mantiene al 100%.
                float frictionMultiplier = Mathf.Lerp(1f, 0.25f, rain);

                dynamicMaterial.staticFriction = originalMaterial.staticFriction * frictionMultiplier;
                dynamicMaterial.dynamicFriction = originalMaterial.dynamicFriction * frictionMultiplier;
            }
        }
    }

    private void ApplyWindForce(Vector3 windVelocity)
    {
        Vector3 relativeVelocity = windVelocity - rb.linearVelocity;
        float speed = relativeVelocity.magnitude;

        if (speed > 0.1f)
        {
            Vector3 scale = transform.localScale;
            Vector3 normWind = relativeVelocity.normalized;

            float areaX = Mathf.Abs(Vector3.Dot(transform.right, normWind)) * (scale.y * scale.z);
            float areaY = Mathf.Abs(Vector3.Dot(transform.up, normWind)) * (scale.x * scale.z);
            float areaZ = Mathf.Abs(Vector3.Dot(transform.forward, normWind)) * (scale.x * scale.y);

            float effectiveArea = areaX + areaY + areaZ;
            float forceMagnitude = 0.5f * WeatherManager.Instance.airDensity * dragCoefficient * effectiveArea * (speed * speed);
            Vector3 forceVector = normWind * forceMagnitude;

            rb.AddForce(forceVector, ForceMode.Force);
            Debug.DrawRay(transform.position, forceVector / 5000f, Color.red);
        }
    }
}