using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class AutoNavigator : MonoBehaviour
{
    public bool isActive = false;
    public Transform targetDestination;

    private NavMeshAgent agent;
    private StarterAssetsInputs input;
    private ThirdPersonController controller;

    private float stoppingDistance = 0.5f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();

        if (agent != null)
        {
            agent.updatePosition = false; // no mueve automáticamente el transform
            agent.updateRotation = false;
        }

        agent.speed = controller.MoveSpeed;
    }

    void Update() {
        if (!isActive || agent == null)
            return;

        // Buscar destino si ha desaparecido
        if (targetDestination == null)
        {
            GameObject newTarget = GameObject.FindWithTag("Objetivo");
            if (newTarget != null)
            {
                targetDestination = newTarget.transform;
                agent.SetDestination(targetDestination.position);
            }
            else
            {
                // Si no hay objetivo, parar y esperar
                controller.SetOverrideMoveDirection(null);
                return;
            }
        }

        agent.SetDestination(targetDestination.position);

        if (agent.pathPending || !agent.hasPath)
            return;

        Vector3 direction = (agent.steeringTarget - transform.position);
        direction.y = 0;

        bool wantsToMoveForward = input.move.y > 0.1f;

        if (wantsToMoveForward && direction.magnitude > 0.1f)
        {
            controller.SetOverrideMoveDirection(direction.normalized);
        }
        else
        {
            controller.SetOverrideMoveDirection(null);
        }

        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            controller.SetOverrideMoveDirection(null);
            input.move = Vector2.zero;
            agent.ResetPath();
            isActive = false;
        }
    }

    void LateUpdate()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.nextPosition = transform.position;
        }
    }

    public void SetAutoNavigation(bool enabled, Transform destination = null)
    {
        isActive = enabled;
        targetDestination = destination;

        if (enabled && targetDestination != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetDestination.position);
        }
        else
        {
            controller.SetOverrideMoveDirection(null);
            input.move = Vector2.zero;
            agent.ResetPath();
        }
    }

    private void OnEnable()
    {
        AccessibilityManager.Instance.NotifyContextEvent += HandleEvent;
    }

    private void OnDisable()
    {
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.NotifyContextEvent -= HandleEvent;
    }

    public void HandleEvent(AccessibilityEvent evt)
    {
        if (evt.Target != AccessibilityTarget.AutoNavigator && evt.Target != AccessibilityTarget.ALL)
            return;

        switch (evt.Type)
        {
            case EventType.InterestPoint:
                targetDestination = evt.Source;
                break;
            case EventType.Enable:
                SetAutoNavigation(true, targetDestination);
                break;
            case EventType.Disable:
                SetAutoNavigation(false, targetDestination);
                break;
        }
    }
}
