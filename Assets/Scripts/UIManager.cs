using UnityEngine;
using TMPro;
using UnityEngine.UI; // Necesario para los Sliders

public class UIManager : MonoBehaviour
{
    [Header("Referencias a Sistemas")]
    public CraneController craneController;
    public HookController hookController;

    [Header("Elementos de Texto (UI)")]
    public TextMeshProUGUI textoClima;
    public TextMeshProUGUI textoGancho;

    [Header("Controles de Clima (Sliders)")]
    public Slider sliderLluvia;
    public Slider sliderVientoVelocidad;
    public Slider sliderVientoDireccion;

    void Start()
    {
        // Conectamos los sliders interactivos directamente al WeatherManager
        if (WeatherManager.Instance != null)
        {
            if (sliderLluvia != null)
            {
                sliderLluvia.minValue = 0f;
                sliderLluvia.maxValue = 1f; // La lluvia va de 0 a 1
                sliderLluvia.value = WeatherManager.Instance.rainIntensity;
                // AddListener detecta cuando usted mueve el slider y actualiza el clima
                sliderLluvia.onValueChanged.AddListener(val => WeatherManager.Instance.rainIntensity = val);
            }

            if (sliderVientoVelocidad != null)
            {
                sliderVientoVelocidad.minValue = 0f;
                sliderVientoVelocidad.maxValue = 150f; // Viento máximo de 150 km/h
                sliderVientoVelocidad.value = WeatherManager.Instance.windSpeedKmH;
                sliderVientoVelocidad.onValueChanged.AddListener(val => WeatherManager.Instance.windSpeedKmH = val);
            }

            if (sliderVientoDireccion != null)
            {
                sliderVientoDireccion.minValue = 0f;
                sliderVientoDireccion.maxValue = 359f; // Grados de rotación
                sliderVientoDireccion.value = WeatherManager.Instance.windDirectionDegrees;
                sliderVientoDireccion.onValueChanged.AddListener(val => WeatherManager.Instance.windDirectionDegrees = val);
            }
        }
    }

    void Update()
    {
        ActualizarClima();
        ActualizarGancho();
    }

    private void ActualizarClima()
    {
        if (WeatherManager.Instance != null && textoClima != null)
        {
            float viento = WeatherManager.Instance.windSpeedKmH;
            float direccion = WeatherManager.Instance.windDirectionDegrees;
            float lluvia = WeatherManager.Instance.rainIntensity * 100f;

            textoClima.text = $"CLIMA\nViento: {viento:F1} km/h (Dir: {direccion:F0}°)\nLluvia: {lluvia:F0}%";
        }
    }

    private void ActualizarGancho()
    {
        if (hookController != null && textoGancho != null)
        {
            if (hookController.cargaEnganchada != null)
            {
                Rigidbody rbCaja = hookController.cargaEnganchada.GetComponent<Rigidbody>();
                float peso = rbCaja != null ? rbCaja.mass : 0f;

                textoGancho.text = $"ESTADO DEL GANCHO\nCarga: ENGANCHADA\nPeso: {peso} kg";
                textoGancho.color = Color.green;
            }
            else
            {
                textoGancho.text = "ESTADO DEL GANCHO\nCarga: VACÍO";
                textoGancho.color = Color.white;
            }
        }
    }
}