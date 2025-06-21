using UnityEngine;

public class TestObjectiveDummy : MonoBehaviour {
    public float rangoReaparicion = 5f;
    public float distanciaAlcance = 1.5f;

    [SerializeField]
    private Transform playerTransform;  

    private bool activo = true;

    void Start() {
        if (playerTransform == null) {
            Debug.LogWarning("PlayerTransform no asignado en TestObjectiveDummy");
        }

        // Enviamos evento para que la flecha apunte a este dummy
        AccessibilityManager.Instance.SendEvent(new AccessibilityEvent(
            EventType.InterestPoint, transform, AccessibilityTarget.ALL, "Nuevo objetivo activado"
        ));

        if (GetComponent<MeshFilter>() == null) {
            gameObject.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        }

        if (GetComponent<MeshRenderer>() == null){
            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.color = new Color(0f, 0.8f, 0f, 1f);
            renderer.material.SetColor("_EmissionColor", new Color(0f, 0.5f, 0f));
            renderer.material.EnableKeyword("_EMISSION");
        }
    }

    void Update() {
        if (!activo || playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
       
        if (dist <= distanciaAlcance) {
            activo = false;

            Vector3 randomOffset = new Vector3(
                Random.Range(-rangoReaparicion, rangoReaparicion),
                0,
                Random.Range(-rangoReaparicion, rangoReaparicion)
            );

            Vector3 nuevoPos = transform.position + randomOffset;

            // Instanciar copia del mismo objeto usando el prefab que es el propio objeto
            GameObject nuevoDummy = Instantiate(gameObject, nuevoPos, Quaternion.identity);

            // Asignar playerTransform al nuevo dummy
            TestObjectiveDummy dummyScript = nuevoDummy.GetComponent<TestObjectiveDummy>();
            if (dummyScript != null) {
                dummyScript.playerTransform = playerTransform;
                dummyScript.activo = true; // Re-activar el dummy para que funcione
            }

            Destroy(gameObject);
        }
    }
}
