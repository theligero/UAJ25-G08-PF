using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    public float rangoReaparicion = 5f;
    public float distanciaAlcance = 1.5f;

    [SerializeField]
    private Transform playerTransform;

    [HideInInspector]
    public bool activo = true;

    private void Start() {
        // Si no se asignó el jugador, buscar uno automáticamente
        if (playerTransform == null && GameObject.FindWithTag("Player") != null) {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        // Asegurar que el dummy tenga forma visible
        SetupVisuals();
    }

    private void Update()
    {
        if (!activo || playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= distanciaAlcance){
            activo = false;


            //Vector3 randomOffset = new Vector3(
            //    Random.Range(-rangoReaparicion, rangoReaparicion),
            //    0,
            //    Random.Range(-rangoReaparicion, rangoReaparicion)
            //);

            //Vector3 nuevoPos = transform.position + randomOffset;

            //GameObject nuevoDummy = Instantiate(gameObject, nuevoPos, Quaternion.identity);
            //nuevoDummy.name = gameObject.name;
            //DummyBehaviour nuevoScript = nuevoDummy.GetComponent<DummyBehaviour>();

            //AccessibilityManager.Instance.AddObjective(nuevoDummy.transform);

            //if (nuevoScript != null) {
            //    nuevoScript.playerTransform = playerTransform;
            //    nuevoScript.activo = true;
            //}
       
            AccessibilityManager.Instance.NextObjective();
            Destroy(gameObject);
        }
    }

    private void SetupVisuals() {
        if (GetComponent<MeshFilter>() == null) {
            gameObject.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        }

        if (GetComponent<MeshRenderer>() == null) {
            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.color = new Color(0f, 0.8f, 0f, 1f);
            renderer.material.SetColor("_EmissionColor", new Color(0f, 0.5f, 0f));
            renderer.material.EnableKeyword("_EMISSION");
        }
    }
}
