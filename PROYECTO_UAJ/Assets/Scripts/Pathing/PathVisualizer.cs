using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform objetivo;

    private bool active = false;

    private NavMeshPath path;
    void Start()
    {
        Debug.Log("Para activar/desactivar la línea de pathing pulsa L o el botón oeste del mando");
        path = new NavMeshPath();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Fire3")) active = !active;
        if (objetivo != null)
        {
            // Calcula una ruta desde tu posición actual hasta el objetivo
            if (active && NavMesh.CalculatePath(transform.position, objetivo.position, NavMesh.AllAreas, path))
            {
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }
            else lineRenderer.positionCount = 0;
        }
        else objetivo = GameObject.FindWithTag("Objetivo").transform;
    }
}
