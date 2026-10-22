using UnityEngine;

public class CraneBuilder : MonoBehaviour
{
    // El atributo ContextMenu añade un botón al menú del script en el Inspector
    [ContextMenu("Construir Grúa")]
    public void ConstruirGrua()
    {
        // 1. Limpiar construcciones anteriores para evitar duplicados si lo ejecutamos varias veces
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        // 2. Estructura Estática (Padre de los pilares y la viga)
        GameObject estructura = new GameObject("Estructura_Estatica");
        estructura.transform.SetParent(this.transform);
        estructura.transform.localPosition = Vector3.zero;

        // Pilares (Alto: 10 unidades, elevados 5 unidades para que la base toque el suelo)
        CrearCubo("Pilar_1", estructura.transform, new Vector3(-6f, 5f, -3f), new Vector3(1f, 10f, 1f));
        CrearCubo("Pilar_2", estructura.transform, new Vector3(-6f, 5f, 3f), new Vector3(1f, 10f, 1f));
        CrearCubo("Pilar_3", estructura.transform, new Vector3(6f, 5f, -3f), new Vector3(1f, 10f, 1f));
        CrearCubo("Pilar_4", estructura.transform, new Vector3(6f, 5f, 3f), new Vector3(1f, 10f, 1f));

        // Viga Principal (Largo: 16 unidades, uniendo los pilares por arriba)
        CrearCubo("Viga_Principal", estructura.transform, new Vector3(0f, 10.5f, 0f), new Vector3(16f, 1f, 3f));

        // 3. Carro Móvil (Pieza que se deslizará por la viga)
        GameObject carro = CrearCubo("Carro_Movil", this.transform, new Vector3(0f, 9.5f, 0f), new Vector3(2f, 1f, 3.2f));

        // 4. Cable (Visual) - Es hijo del carro para que se mueva junto a él
        GameObject cable = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cable.name = "Cable_Visual";
        cable.transform.SetParent(carro.transform);
        cable.transform.localPosition = new Vector3(0f, -2f, 0f);
        cable.transform.localScale = new Vector3(0.1f, 2f, 0.1f); // Lo hacemos delgado y alargado
        DestroyImmediate(cable.GetComponent<Collider>()); // El cable visual no debe chocar con nada

        // 5. Gancho / Imán - Hijo del carro (No del cable, para evitar que su escala se deforme)
        CrearCubo("Gancho", carro.transform, new Vector3(0f, -4f, 0f), new Vector3(1.5f, 0.5f, 1.5f));

        Debug.Log("Modelo de grúa generado con éxito.");
    }

    // Método auxiliar para crear cubos rápidamente y asignarles un padre y medidas
    private GameObject CrearCubo(string nombre, Transform padre, Vector3 posLocal, Vector3 escala)
    {
        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubo.name = nombre;
        cubo.transform.SetParent(padre);
        cubo.transform.localPosition = posLocal;
        cubo.transform.localScale = escala;
        return cubo;
    }
}