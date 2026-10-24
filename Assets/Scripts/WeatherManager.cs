using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configuración del Viento")]
    public bool windEnabled = true;
    public float windSpeedKmH = 40f;
    [Range(0f, 359f)]
    public float windDirectionDegrees = 90f;
    public float airDensity = 1.225f;

    [Header("Configuración de Lluvia y Tormenta")]
    [Range(0f, 1f)]
    [Tooltip("0 = Despejado, 1 = Tormenta intensa")]
    public float rainIntensity = 0f;

    public ParticleSystem rainParticleSystem;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (rainParticleSystem != null)
        {
            // 1. Control de intensidad (cantidad de gotas)
            var emission = rainParticleSystem.emission;
            emission.rateOverTime = rainIntensity * 5000f;

            // 2. Control de dirección (influencia del viento)
            var velocityModule = rainParticleSystem.velocityOverLifetime;
            velocityModule.enabled = true;
            // Aseguramos que el viento afecte a las partículas en el espacio global del mundo
            velocityModule.space = ParticleSystemSimulationSpace.World;

            Vector3 windVel = GetWindVelocity();

            // Aplicamos la velocidad del viento en los ejes X y Z. 
            // El eje Y se mantiene en 0 para que la gravedad normal del Particle System controle la caída.
            velocityModule.x = new ParticleSystem.MinMaxCurve(windVel.x);
            velocityModule.y = new ParticleSystem.MinMaxCurve(0f);
            velocityModule.z = new ParticleSystem.MinMaxCurve(windVel.z);
        }
    }

    public Vector3 GetWindVelocity()
    {
        if (!windEnabled) return Vector3.zero;
        float speedMS = windSpeedKmH / 3.6f;
        float rad = windDirectionDegrees * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
        return direction.normalized * speedMS;
    }
}