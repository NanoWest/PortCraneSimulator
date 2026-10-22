using UnityEngine;

public class HookController : MonoBehaviour
{
    private GameObject cargaDetectada = null;
    private GameObject cargaEnganchada = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && cargaDetectada != null && cargaEnganchada == null)
        {
            EngancharCarga();
        }

        if (Input.GetKeyDown(KeyCode.Q) && cargaEnganchada != null)
        {
            SoltarCarga();
        }

        // ELIMINADO: Ya no forzamos la posición de la caja en el Update.
        // El FixedJoint se encargará de mantenerla unida físicamente.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carga") && cargaEnganchada == null)
        {
            cargaDetectada = other.gameObject;
            Debug.Log("Carga detectada: presione E para enganchar.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == cargaDetectada && cargaEnganchada == null)
        {
            cargaDetectada = null;
            Debug.Log("Carga fuera de alcance.");
        }
    }

    private void EngancharCarga()
    {
        cargaEnganchada = cargaDetectada;

        // 1. Acomodamos la caja justo debajo del gancho una sola vez
        cargaEnganchada.transform.position = this.transform.position + new Vector3(0f, -0.75f, 0f);
        cargaEnganchada.transform.rotation = Quaternion.identity;

        // 2. NUEVO: Creamos una articulación física para pegar la caja al gancho
        FixedJoint joint = cargaEnganchada.AddComponent<FixedJoint>();
        joint.connectedBody = this.GetComponent<Rigidbody>();

        // NOTA: Eliminamos la línea que apagaba la física (isKinematic = true).
        // Al mantener sus físicas encendidas, la caja reaccionará de forma natural a los choques.

        Debug.Log("Carga enganchada con éxito.");
    }

    private void SoltarCarga()
    {
        // 1. Para soltarla, simplemente destruimos la articulación
        FixedJoint joint = cargaEnganchada.GetComponent<FixedJoint>();
        if (joint != null)
        {
            Destroy(joint);
        }

        cargaEnganchada = null;
        cargaDetectada = null;

        Debug.Log("Carga liberada.");
    }
}