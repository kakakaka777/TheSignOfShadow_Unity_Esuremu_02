using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public int segments = 60;            // ‰~‚Ì•ªŠ„”
    public float radius = 3f;            // ”¼Œa
    public float lineWidth = 0.05f;      // ü‚Ì‘¾‚³

    void Start()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        Vector3[] points = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (360f / segments) * i;
            points[i] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }

        lr.SetPositions(points);
    }

}
