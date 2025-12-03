using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDrawer : MonoBehaviour
{
    public float maxBloodAmount = 100f;    
    public float bloodUsagePerPoint = 1f; //1“_‚²‚Æ‚ÌÁ”ï—Ê
    public float drawDistance = 5f; //Å‘å•`‰æ‹——£
    public LayerMask drawableLayer; // •`‚¯‚é‘ÎÛ‚ÌƒŒƒCƒ„[

    private float currentBlood;
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private Camera mainCam;
    private bool isDrawing = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCam = Camera.main;
        currentBlood = maxBloodAmount;

        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.red;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (!isDrawing || currentBlood <= 0f) return;

        if (Input.GetMouseButton(0)) // ¶ƒNƒŠƒbƒN‚Å•`‚­
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, drawDistance, drawableLayer))
            {
                Vector3 drawPoint = hit.point + hit.normal * 0.01f; // ­‚µ•‚‚©‚¹‚Ä•`‰æ
                if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], drawPoint) > 0.01f)
                {
                    points.Add(drawPoint);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPosition(points.Count - 1, drawPoint);

                    currentBlood -= bloodUsagePerPoint;
                    if (currentBlood <= 0f)
                    {
                        Debug.Log("ŒŒ‚ªs‚«‚½I");
                    }
                }
            }
        }
    }

    public void StartDrawing()
    {
        isDrawing = true;
        currentBlood = maxBloodAmount;
        lineRenderer.positionCount = 0;
        points.Clear();
    }

    public void StopDrawing()
    {
        isDrawing = false;
    }

    public bool IsDrawing() => isDrawing;
}
