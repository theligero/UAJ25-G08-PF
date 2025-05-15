using UnityEngine;

public class ArrowIndicator : MonoBehaviour {
    public GameObject objetivo; // Objetivo a apuntar
    public Vector3 offset = new Vector3(0, 2, 0); // Altura sobre la cabeza

    private GameObject flecha;

    void Start() {
        if (objetivo == null)
            return; // No crear la flecha si no hay objetivo

        CrearFlecha();
    }

    void Update() {
        if (objetivo != null) {
            if (flecha == null) { // Si objetivo aparece en tiempo de ejecucion, creamos la flecha
                CrearFlecha();
            }

            // Mantener la posicion sobre el jugador
            flecha.transform.position = transform.position + offset;

            // Apuntar hacia el objetivo
            Vector3 direccion = objetivo.transform.position - flecha.transform.position;
            if (direccion != Vector3.zero) {
                flecha.transform.rotation = Quaternion.LookRotation(direccion);
            }
        }
        else {
            if (flecha != null) { // Si no hay objetivo, destruir la flecha
                Destroy(flecha);
                flecha = null;
            }
        }
    }

    void CrearFlecha() {
        // Crear un GameObject vacio para la flecha
        flecha = new GameObject("Flecha");
        flecha.transform.SetParent(transform);
        flecha.transform.localPosition = offset;

        // Apuntar el eje Z hacia arriba visualmente (porque el modelo esta vertical)
        flecha.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Crear material rojo
        Material rojo = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        rojo.color = Color.red;

        // Crear cuerpo de la flecha
        GameObject cuerpo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cuerpo.transform.SetParent(flecha.transform);
        cuerpo.transform.localPosition = new Vector3(0, 0.25f, 0);
        cuerpo.transform.localScale = new Vector3(0.1f, 0.25f, 0.1f);
        cuerpo.GetComponent<Renderer>().material = rojo;

        // Punta con un cubo rotado
        GameObject punta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        punta.transform.SetParent(flecha.transform);
        punta.transform.localPosition = new Vector3(0, 0.25f, 0.25f);
        punta.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        punta.transform.rotation = Quaternion.Euler(0, 0, 45); // inclinacion diagonal
        punta.GetComponent<Renderer>().material = rojo;
    }
}
