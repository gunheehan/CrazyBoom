using System.Collections.Generic;
using UnityEngine;

public class RayTrailManager : MonoBehaviour
{
    private List<LineRenderer> trails = new List<LineRenderer>();
    public Material lineMaterial;
    
    public void DrawRay(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("RayTrail");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        trails.Add(lineRenderer);
    }
}