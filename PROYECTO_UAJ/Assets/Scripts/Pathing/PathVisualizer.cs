using UnityEngine;
using UnityEngine.AI;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    [SerializeField]
    private LineRenderer lineRenderer;

    void Update()
    {
        if (navMeshAgent.hasPath)
        {
            lineRenderer.positionCount = navMeshAgent.path.corners.Length;
            lineRenderer.SetPositions(navMeshAgent.path.corners);
        }
    }
}
