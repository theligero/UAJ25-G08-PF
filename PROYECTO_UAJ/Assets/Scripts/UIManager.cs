using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/UI Manager Runtime")]
public class UIManager : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Tiempo total en segundos")]
    public float totalTime = 180f;  // 3 minutos

    [Header("Pause Menu Settings")]
    [Tooltip("Tecla para abrir/cerrar el menú")]
    public KeyCode toggleMenuKey = KeyCode.Escape;
    [Tooltip("Botón de mando para abrir/cerrar (Start)")]
    public string toggleMenuButton = "Cancel"; // mapea al botón Start en Input Settings

    private float timeRemaining;
    private int totalItems;
    private int itemsRemaining;

    private Text timeText;
    private Text itemsText;

    private GameObject pausePanel;
    private bool isPaused = false;
    private Canvas mainCanvas;

    void Awake()
    {
        CreateHUD();
        CreatePauseMenu();
    }

    void OnEnable()
    {
        InteractableItem.OnItemPicked += HandleItemPicked;
    }

    void OnDisable()
    {
        InteractableItem.OnItemPicked -= HandleItemPicked;
    }

    void Start()
    {
        // Inicializa valores y oculta menú
        timeRemaining = totalTime;
        totalItems = FindObjectsOfType<InteractableItem>().Length;
        itemsRemaining = totalItems;
        UpdateTimeText();
        UpdateItemsText();

        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Toggle pause menu con teclado o mando
        if (Input.GetKeyDown(toggleMenuKey) || Input.GetButtonDown(toggleMenuButton))
        {
            TogglePauseMenu();
        }

        // Si estamos en pausa, no actualizar cronómetro
        if (isPaused) return;

        // Cuenta atrás
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;
            UpdateTimeText();
        }
    }

    private void HandleItemPicked()
    {
        // Decrementa y actualiza conteo
        itemsRemaining = Mathf.Max(0, itemsRemaining - 1);
        UpdateItemsText();
    }

    private void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        if (timeText != null)
            timeText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateItemsText()
    {
        if (itemsText != null)
            itemsText.text = string.Format("Objetos restantes: {0}/{1}", itemsRemaining, totalItems);
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
            pausePanel.transform.SetAsLastSibling();
        }
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void CreateHUD()
    {
        // --- HUD Canvas ---
        GameObject canvasGO = new GameObject("UICanvas");
        mainCanvas = canvasGO.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.overrideSorting = true;
        mainCanvas.sortingOrder = 1000;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel superior con tiempo y objetos
        GameObject panelGO = new GameObject("HUDPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.5f);
        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 1f);
        panelRT.anchorMax = new Vector2(1f, 1f);
        panelRT.pivot = new Vector2(0.5f, 1f);
        panelRT.anchoredPosition = Vector2.zero;
        panelRT.sizeDelta = new Vector2(0f, 100f);

        // Fuente
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Time Text
        GameObject timeGO = new GameObject("TimeText");
        timeGO.transform.SetParent(panelGO.transform, false);
        timeText = timeGO.AddComponent<Text>();
        timeText.font = font;
        timeText.fontSize = 24;
        timeText.alignment = TextAnchor.MiddleLeft;
        timeText.color = Color.white;
        RectTransform timeRT = timeGO.GetComponent<RectTransform>();
        timeRT.anchorMin = new Vector2(0f, 0f);
        timeRT.anchorMax = new Vector2(0f, 1f);
        timeRT.pivot = new Vector2(0f, 0.5f);
        timeRT.anchoredPosition = new Vector2(10f, 0f);
        timeRT.sizeDelta = new Vector2(200f, 0f);

        // Items Text
        GameObject itemsGO = new GameObject("ItemsText");
        itemsGO.transform.SetParent(panelGO.transform, false);
        itemsText = itemsGO.AddComponent<Text>();
        itemsText.font = font;
        itemsText.fontSize = 24;
        itemsText.alignment = TextAnchor.MiddleRight;
        itemsText.color = Color.white;
        RectTransform itemsRT = itemsGO.GetComponent<RectTransform>();
        itemsRT.anchorMin = new Vector2(1f, 0f);
        itemsRT.anchorMax = new Vector2(1f, 1f);
        itemsRT.pivot = new Vector2(1f, 0.5f);
        itemsRT.anchoredPosition = new Vector2(-10f, 0f);
        itemsRT.sizeDelta = new Vector2(200f, 0f);
    }

    private void CreatePauseMenu()
    {
        if (mainCanvas == null) {
            Debug.LogError("UIManager: mainCanvas is null. Make sure CreateHUD has been called.");
            return;
        }

        // Panel central (fondo)
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(mainCanvas.transform, false);
        RectTransform bgRT = pausePanel.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0f, 0f);
        bgRT.anchorMax = new Vector2(1f, 1f);
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bg = pausePanel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.75f);

        // Fuente
        Font fontBtn = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane + 10f); // +10f o la distancia que necesites desde la cámara
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);


        // Botones
        CreateButton(pausePanel.transform, "Resume", fontBtn, () => TogglePauseMenu(), 0, 300);

        CreateButton(pausePanel.transform, "Restart Level", fontBtn, () => UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex), 0, 200);

        CreateButton(pausePanel.transform, "Toggle Flashlight", fontBtn, () => {
            var gf = FindObjectOfType<GreenFlashlight>();
            if (gf != null) gf.flashlight.enabled = !gf.flashlight.enabled;
        }, 0, 100);

        CreateButton(pausePanel.transform, "Quit Game", fontBtn, () => Application.Quit(), 0, 0);

        // Ocultar inicialmente
        pausePanel.SetActive(false);
    }

    private void CreateButton(Transform parent, string label, Font font, UnityEngine.Events.UnityAction onClick, float x, float y)
    {
        GameObject btnGO = new GameObject(label + "Button");
        btnGO.transform.SetParent(parent, false);

        RectTransform btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(200, 50);

        Image img = btnGO.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.9f);
        Button btn = btnGO.AddComponent<Button>();

        btn.onClick.AddListener(onClick);
        // Pivote y anclas centradas para que (0,0) sea el centro del panel
        btnRT.anchorMin = new Vector2(0.5f, 0.5f);
        btnRT.anchorMax = new Vector2(0.5f, 0.5f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);

        // Posición relativa al centro
        btnRT.anchoredPosition = new Vector2(x, y);

        // Texto
        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform, false);
        Text txt = txtGO.AddComponent<Text>();
        txt.font = font;
        txt.text = label;
        txt.fontSize = 24;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.black;

        RectTransform txtRT = txtGO.GetComponent<RectTransform>();
        txtRT.anchorMin = new Vector2(0f, 0f);
        txtRT.anchorMax = new Vector2(1f, 1f);
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;
    }
}
