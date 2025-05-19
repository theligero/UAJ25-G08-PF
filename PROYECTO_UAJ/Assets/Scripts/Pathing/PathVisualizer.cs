using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class PathVisualizer : MonoBehaviour
{
    private LineRenderer line;
    private Transform objetivo;

    private bool active = true;

    private NavMeshPath path;
    void Start()
    {
        Debug.Log("Para activar/desactivar la línea de pathing pulsa L o el botón oeste del mando");
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();
        if(line != null)
        {
            line.material = new Material(Shader.Find("Sprites/Default"));
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.yellow, 0.0f),
                    new GradientColorKey(Color.red, 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 1.0f)
                }
            );

            line.colorGradient = gradient;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
        }
    }

    void Update()
    {
        if (objetivo != null)
        {
            // Calcula una ruta desde tu posición actual hasta el objetivo
            if (active && NavMesh.CalculatePath(transform.position, objetivo.position, NavMesh.AllAreas, path))
            {
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
            }
            else line.positionCount = 0;
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
        switch (evt.Type)
        {
            case EventType.InterestPoint:
                objetivo = evt.Source;
                break;
            case EventType.Enable:
                if (evt.Target == AccessibilityTarget.PathVisualizer)
                    active = true;
                break;
            case EventType.Disable:
                if (evt.Target == AccessibilityTarget.PathVisualizer)
                    active = false;
                break;
        }
    }
}
