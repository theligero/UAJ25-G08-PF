using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Gameplay/Clue Manager")]
public class ClueManager : MonoBehaviour
{
    [Header("Clue Settings")]
    [Tooltip("Prefab de pista con Collider (IsTrigger) y el script Clue")]
    public GameObject cluePrefab;
    [Tooltip("Espaciado entre pistas (unidades)")]
    public float clueSpacing = 2f;
    [Tooltip("Altura sobre el suelo donde colocar la pista")]
    public float heightOffset = 0.1f;
    [Tooltip("Layers del terreno para alinear las pistas")]
    public LayerMask groundLayer;
    [Tooltip("Tag del jugador que recoge pistas")]
    public string playerTag = "Player";

    private Transform player;
    private InteractableItem targetItem;
    private List<Vector3> cluePositions;
    private int nextClueIndex;
    private bool routingActive;

    void Awake()
    {
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go) player = go.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !routingActive)
        {
            StartRouting();
        }
    }

    void StartRouting()
    {
        if (player == null || cluePrefab == null) return;

        // Encuentra el InteractableItem más cercano
        targetItem = null;
        float bestDist = float.MaxValue;
        foreach (var item in FindObjectsOfType<InteractableItem>())
        {
            float d = Vector3.Distance(player.position, item.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                targetItem = item;
            }
        }
        if (targetItem == null) return;

        // Determinar posiciones en suelo
        Vector3 startGround = GetGroundPosition(player.position);
        Vector3 endGround = GetGroundPosition(targetItem.transform.position);
        Vector3 direction = (endGround - startGround).normalized;
        float totalDist = Vector3.Distance(startGround, endGround);

        int count = Mathf.FloorToInt(totalDist / clueSpacing);
        cluePositions = new List<Vector3>(count);
        for (int i = 1; i <= count; i++)
        {
            Vector3 point = startGround + direction * (i * clueSpacing);
            cluePositions.Add(point);
        }

        nextClueIndex = 0;
        routingActive = true;
        SpawnNextClue();
    }

    public void SpawnNextClue()
    {
        if (!routingActive || cluePositions == null) return;
        if (nextClueIndex < cluePositions.Count)
        {
            Vector3 rawPos = cluePositions[nextClueIndex++];
            Vector3 spawnPos = GetGroundPosition(rawPos) + Vector3.up * heightOffset;
            GameObject clueGO = Instantiate(cluePrefab, spawnPos, Quaternion.identity);
            // Asegura Collider y Rigidbody para detección
            var col = clueGO.GetComponent<Collider>();
            if (col) col.isTrigger = true;
            else
            {
                col = clueGO.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }
            var rb = clueGO.GetComponent<Rigidbody>();
            if (rb == null) rb = clueGO.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            Clue clueComp = clueGO.GetComponent<Clue>();
            if (clueComp != null)
                clueComp.manager = this;
        }
        else
        {
            // Llegamos al final de pistas
            routingActive = false;
        }
    }

    private Vector3 GetGroundPosition(Vector3 origin)
    {
        RaycastHit hit;
        Vector3 rayOrigin = origin + Vector3.up * 5f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f, groundLayer))
        {
            return hit.point;
        }
        // Si falla raycast, baja manualmente
        return origin;
    }
}

[AddComponentMenu("Gameplay/Clue")]
public class Clue : MonoBehaviour
{
    [HideInInspector] public ClueManager manager;
    public string playerTag = "Player";
    [Tooltip("Distancia a la que se considera que el jugador ha pisado la pista")]
    public float triggerDistance = 1f;

    private Transform player;
    private bool activated = false;

    void Start()
    {
        // Asignar player
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go) player = go.transform;

        // Asegurar collider y rigidbody si existen
        var col = GetComponent<Collider>(); if (col) col.isTrigger = true;
        var rb = GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;
    }

    void Update()
    {
        if (activated || player == null) return;
        // Detección por proximidad en lugar de trigger
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist <= triggerDistance)
        {
            OnPicked();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.CompareTag(playerTag))
        {
            OnPicked();
        }
    }

    private void OnPicked()
    {
        activated = true;
        Debug.Log($"Clue recogida: {name}");
        manager.SpawnNextClue();
        Destroy(gameObject);
    }
}
