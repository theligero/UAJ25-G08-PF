using UnityEngine;

public class ArrowIndicator : MonoBehaviour {
    public GameObject objetivo; // Objetivo a apuntar
    public Vector3 offset = new Vector3(0, 2, 0); // Altura sobre el jugador

    [SerializeField]
    private GameObject modeloFlecha;      

    private GameObject flecha;

    void Start() {
        if (objetivo != null && modeloFlecha != null)
            CrearFlecha();
    }

    void Update() {
        if (objetivo != null) {
            if (flecha == null && modeloFlecha != null) {
                CrearFlecha();
            }

            // Posicionar sobre el jugador
            flecha.transform.position = transform.position + offset;

            // Apuntar hacia el objetivo
            Vector3 direccion = objetivo.transform.position - flecha.transform.position;
            if (direccion != Vector3.zero) {
                flecha.transform.rotation = Quaternion.LookRotation(direccion);
            }
        }
        else {
            if (flecha != null) {
                Destroy(flecha);
                flecha = null;
            }
        }
    }

    void CrearFlecha() {
        flecha = Instantiate(modeloFlecha, transform.position + offset, Quaternion.identity, transform);
    }
}
