using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class DrawShapes : MonoBehaviour
{
    private LineRenderer currentLineRenderer;
    private List<Vector2> points = new List<Vector2>();
    public Material lineMaterial;

    private GameObject shapeObject;

    public PlayerController playerController;

    void Update()
    {
        // Mouse down
        if (Input.GetMouseButtonDown(0) && playerController.getCurrentGauge() > 0)
        {
            CreateNewLine();
        }

        // Mouse hold
        if (Input.GetMouseButton(0) && playerController.getCurrentGauge() > 0)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AddPointToLine(mousePos);
        }

        // Mouse up
        if (Input.GetMouseButtonUp(0))
        {
            if (points.Count > 2)
            {
                CreateEdgeCollider();
            }
        }

        // Update LineRenderer to follow the movement of the shape due to physics
        if (shapeObject != null && currentLineRenderer != null)
        {
            UpdateLineRendererPosition();
        }
    }

    void CreateNewLine()
    {
        shapeObject = new GameObject("DrawnShape");
        shapeObject.tag = "DrawnShape";

        // Add LineRenderer
        currentLineRenderer = shapeObject.AddComponent<LineRenderer>();
        currentLineRenderer.positionCount = 0;
        currentLineRenderer.startWidth = 0.1f;
        currentLineRenderer.endWidth = 0.1f;
        currentLineRenderer.material = lineMaterial;

        // Clear the points
        points.Clear();
    }

    void AddPointToLine(Vector2 newPoint)
    {
        if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], newPoint) > 0.1f)
        {
            if (points.Count > 1)
            {
                playerController.DeltaGauge(-Vector2.Distance(points[points.Count - 1] * 1.2f, newPoint));
            }

            points.Add(newPoint);
            currentLineRenderer.positionCount = points.Count;
            currentLineRenderer.SetPosition(points.Count - 1, newPoint);
        }
    }

    // Function to create an EdgeCollider2D when the drawing is finished
    void CreateEdgeCollider()
    {
        EdgeCollider2D edgeCollider = shapeObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = points.ToArray(); 

        // Add Rigidbody2D 
        Rigidbody2D rb = shapeObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;  
        rb.gravityScale = 1;  

        Vector2 center = CalculateCenterOfMass(points);
        rb.centerOfMass = center; 
    }

    // Calculate the center of mass of the shape for stability
    Vector2 CalculateCenterOfMass(List<Vector2> points)
    {
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points)
        {
            center += point;
        }
        return center / points.Count;
    }

    // Update LineRenderer's positions to match the object's movement
    void UpdateLineRendererPosition()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 worldPoint = shapeObject.transform.TransformPoint(points[i]);
            currentLineRenderer.SetPosition(i, worldPoint);
        }
    }

    public void ClearDrawnShapes()
    {
        // Find all objects tagged as "DrawnShape"
        GameObject[] drawnShapes = GameObject.FindGameObjectsWithTag("DrawnShape");

        // Loop through and destroy each one
        foreach (GameObject shape in drawnShapes)
        {
            Destroy(shape);
        }
    }
}