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

        // --- ALINEACIÓN MILIMÉTRICA PRE-ANCLAJE ---
        // 1. Purgamos la energía cinética y rotacional residual de la caja
        Rigidbody rbCaja = cargaEnganchada.GetComponent<Rigidbody>();
        if (rbCaja != null)
        {
            rbCaja.linearVelocity = Vector3.zero;
            rbCaja.angularVelocity = Vector3.zero;
        }

        // 2. Forzamos a la caja a calcar la rotación del gancho (paralela al suelo)
        cargaEnganchada.transform.rotation = transform.rotation;

        // 3. Embonamos la caja centrada, basándonos en los vectores del anchor (0.5) y connectedAnchor (0.25)
        cargaEnganchada.transform.position = transform.position + new Vector3(0f, -0.75f, 0f);
        // ------------------------------------------

        ConfigurableJoint joint = cargaEnganchada.AddComponent<ConfigurableJoint>();
        joint.connectedBody = this.GetComponent<Rigidbody>();

        // Desactivamos la colisión interna para eliminar el temblor explosivo.
        joint.enableCollision = false;

        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector3(0f, 0.5f, 0f);
        joint.connectedAnchor = new Vector3(0f, -0.25f, 0f);

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Mantenemos el límite anti-viento para evitar atravesamientos
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 2f;
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;
        joint.angularZLimit = limit;

        Debug.Log("Carga enganchada. Alineación y embonado perfectos aplicados.");
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