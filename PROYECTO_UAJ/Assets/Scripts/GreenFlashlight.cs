using UnityEngine;

[AddComponentMenu("Lighting/Green Flashlight Toggle")]
public class GreenFlashlight : MonoBehaviour
{
    [Header("Configuración del Beam")]
    [Tooltip("Si ya tienes un Light tipo Spot hijo, asígnalo aquí; si no, se creará uno automáticamente.")]
    public Light flashlight;

    [Tooltip("Posición local relativa desde donde sale el haz")]
    public Vector3 localOffset = new Vector3(0f, 0f, 0.5f);

    [Header("Parámetros del Haz")]
    [Tooltip("Color del haz de luz")]
    public Color beamColor = Color.green;
    [Tooltip("Intensidad de la luz")]
    public float intensity = 5f;
    [Tooltip("Alcance de la luz")]
    public float range = 20f;
    [Tooltip("Ángulo del cono de la spotlight (en grados)")]
    [Range(1f, 90f)]
    public float spotAngle = 30f;

    [Header("Opcional: Flicker")]
    [Tooltip("Activar ligera variación en la intensidad para simular imperfecciones")]
    public bool enableFlicker = false;
    [Tooltip("Máxima variación relativa (0 = sin variación, 0.5 = ±50%)")]
    [Range(0f, 0.5f)]
    public float flickerAmount = 0.1f;
    [Tooltip("Velocidad del flicker")]
    public float flickerSpeed = 0.2f;

    [Header("Tecla Toggle")]
    [Tooltip("Tecla para activar/desactivar la linterna")]
    public KeyCode toggleKey = KeyCode.Q;

    // Internos
    private float baseIntensity;
    private float flickerTimer;
    private bool isOn = true;

    void Awake()
    {
        // Ajustamos la luz ambiental para que el efecto destaque
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black * 0.1f;

        // Si no hay Light asignada, creamos una Spotlight como hijo
        if (flashlight == null)
        {
            GameObject go = new GameObject("GreenFlashlight");
            go.transform.SetParent(transform);
            go.transform.localPosition = localOffset;
            go.transform.localRotation = Quaternion.identity;
            flashlight = go.AddComponent<Light>();
            flashlight.type = LightType.Spot;
        }

        // Inicializamos propiedades
        flashlight.color = beamColor;
        flashlight.intensity = intensity;
        flashlight.range = range;
        flashlight.spotAngle = spotAngle;
        flashlight.shadows = LightShadows.Soft;

        baseIntensity = intensity;
        flashlight.enabled = isOn;
    }

    void Update()
    {
        // Toggle con la tecla Q (o la que definas)
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }

    void LateUpdate()
    {
        if (!isOn || flashlight == null) return;

        // 1. Mantener posición y rotación alineada con el jugador
        flashlight.transform.position = transform.TransformPoint(localOffset);
        flashlight.transform.rotation = transform.rotation;

        // 2. Flicker opcional
        if (enableFlicker)
        {
            flickerTimer += Time.deltaTime * flickerSpeed;
            float variation = Mathf.PerlinNoise(flickerTimer, 0f) * flickerAmount;
            flashlight.intensity = baseIntensity * (1f - flickerAmount + variation);
        }
        else
        {
            flashlight.intensity = baseIntensity;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Para que los cambios en el Inspector se reflejen en edición
        if (flashlight != null)
        {
            flashlight.color = beamColor;
            flashlight.range = range;
            flashlight.spotAngle = spotAngle;
            baseIntensity = intensity;
            flashlight.enabled = isOn;
        }
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black * 0.1f;
    }
#endif
}
