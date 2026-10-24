using UnityEngine;

public class HookController : MonoBehaviour
{
    private GameObject cargaDetectada = null;
    public GameObject cargaEnganchada = null;

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

        ConfigurableJoint joint = cargaEnganchada.AddComponent<ConfigurableJoint>();
        joint.connectedBody = this.GetComponent<Rigidbody>();

        // CORRECCIÓN 1: Desactivamos la colisión interna para eliminar el temblor explosivo.
        joint.enableCollision = false;

        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector3(0f, 0.5f, 0f);
        joint.connectedAnchor = new Vector3(0f, -0.25f, 0f);

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // CORRECCIÓN 2: Cambiamos Free a Limited para que el viento no balancee la caja 
        // más allá de cierto ángulo, evitando que atraviese físicamente el gancho.
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        // Configuramos el límite máximo de oscilación a 20 grados
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 2f;
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;
        joint.angularZLimit = limit;

        Debug.Log("Carga enganchada. Temblor corregido y oscilación limitada.");
    }

    private void SoltarCarga()
    {
        ConfigurableJoint joint = cargaEnganchada.GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            Destroy(joint);
        }

        cargaEnganchada = null;
        cargaDetectada = null;

        Debug.Log("Carga liberada.");
    }
}