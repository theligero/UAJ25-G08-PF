using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    [SerializeField]
    private Transform objetivo;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        CalcularRutaOptima();
    }

    void CalcularRutaOptima()
    {
        if (objetivo != null)
        {
            navMeshAgent.SetDestination(objetivo.position);
        }
    }
}
