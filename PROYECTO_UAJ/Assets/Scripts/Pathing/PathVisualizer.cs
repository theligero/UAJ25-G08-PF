using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform objetivo;


    private NavMeshPath path;
    void Start()
    {
        Debug.Log("Para activar la línea de pathing pulsa L");
        path = new NavMeshPath();
    }

    void Update()
    {
        if (objetivo != null)
        {
            // Calcula una ruta desde tu posición actual hasta el objetivo
            if (Input.GetKey(KeyCode.L) && NavMesh.CalculatePath(transform.position, objetivo.position, NavMesh.AllAreas, path))
            {
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }
            else lineRenderer.positionCount = 0;
        }
        else objetivo = GameObject.FindWithTag("Objetivo").transform;
    }
}
