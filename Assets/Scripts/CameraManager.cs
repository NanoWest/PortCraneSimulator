using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Asignación de Cámaras")]
    public Camera camaraPrincipal;
    public Camera camaraSeguimiento; // Nueva cámara intermedia
    public Camera camaraGancho;

    private int indiceCamara = 0; // 0 = Principal, 1 = Seguimiento, 2 = Gancho

    void Start()
    {
        ActualizarCamaras();
    }

    void Update()
    {
        // Alternar cámaras cíclicamente al presionar la tecla C
        if (Input.GetKeyDown(KeyCode.C))
        {
            indiceCamara++;
            if (indiceCamara > 2)
            {
                indiceCamara = 0; // Reinicia el ciclo
            }
            ActualizarCamaras();
        }
    }

    private void ActualizarCamaras()
    {
        if (camaraPrincipal != null) camaraPrincipal.enabled = (indiceCamara == 0);
        if (camaraSeguimiento != null) camaraSeguimiento.enabled = (indiceCamara == 1);
        if (camaraGancho != null) camaraGancho.enabled = (indiceCamara == 2);
    }
}