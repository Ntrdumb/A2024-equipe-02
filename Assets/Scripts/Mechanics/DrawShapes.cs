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
    [SerializeField] Texture2D mouseDrawIcon;
    [SerializeField] Texture2D mouseEraseIcon;

    void Update()
    {
        // Mouse down to start drawing
        if (Input.GetMouseButtonDown(0) && playerController.getCurrentGauge() > 0)
        {
            CreateNewLine();
            Cursor.SetCursor(mouseDrawIcon, Vector2.zero, CursorMode.Auto);
        }

        // Mouse hold to draw
        if (Input.GetMouseButton(0) && playerController.getCurrentGauge() > 0)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AddPointToLine(mousePos);
        }

        // Mouse up to finish drawing
        if (Input.GetMouseButtonUp(0))
        {
            if (points.Count > 2)
            {
                CreateEdgeCollider();
            }
        }

        // Right-click to erase part of the shape
        if (Input.GetMouseButton(1))
        {
            ErasePartOfShape();
            Cursor.SetCursor(mouseEraseIcon, Vector2.zero, CursorMode.Auto);
        }

        // Update LineRenderer positions for all DrawnShape objects
        GameObject[] drawnShapes = GameObject.FindGameObjectsWithTag("DrawnShape");
        foreach (GameObject shape in drawnShapes)
        {
            LineRenderer lineRenderer = shape.GetComponent<LineRenderer>();
            EdgeCollider2D edgeCollider = shape.GetComponent<EdgeCollider2D>();

            if (lineRenderer != null && edgeCollider != null)
            {
                UpdateLineRendererPosition(shape, lineRenderer, edgeCollider);
            }
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
                playerController.DeltaGauge(-Vector2.Distance(points[points.Count - 1] * 1.005f, newPoint));
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
    void UpdateLineRendererPosition(GameObject shape, LineRenderer lineRenderer, EdgeCollider2D edgeCollider)
    {
        // Get the EdgeCollider2D points
        Vector2[] colliderPoints = edgeCollider.points;

        // Update the LineRenderer positions to match the collider points in world space
        for (int i = 0; i < colliderPoints.Length; i++)
        {
            Vector3 worldPoint = shape.transform.TransformPoint(colliderPoints[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }
    }

    public void ClearDrawnShapes()
    {
        GameObject[] drawnShapes = GameObject.FindGameObjectsWithTag("DrawnShape");

        foreach (GameObject shape in drawnShapes)
        {
            Destroy(shape);
        }
    }

    bool IsMouseOverShape(Vector2 mousePos, GameObject shape, EdgeCollider2D edgeCollider)
    {
        // Check if the mouse is near any of the collider's points
        Vector2[] localPoints = edgeCollider.points;
        foreach (Vector2 localPoint in localPoints)
        {
            Vector2 worldPoint = shape.transform.TransformPoint(localPoint);
            if (Vector2.Distance(mousePos, worldPoint) <= 0.2f) // Threshold for interaction
            {
                return true;
            }
        }
        return false;
    }


    void HandleLineSplitting(GameObject originalShape, List<List<Vector2>> segments, Rigidbody2D originalRb)
    {
        // Store the original shape's transform data
        Vector3 originalPosition = originalShape.transform.position;
        Quaternion originalRotation = originalShape.transform.rotation;
        Vector3 originalScale = originalShape.transform.localScale;

        // Store original velocity data
        Vector2 originalVelocity = originalRb.velocity;
        float originalAngularVelocity = originalRb.angularVelocity;

        Destroy(originalShape);

        foreach (List<Vector2> segment in segments)
        {
            if (segment.Count < 2) continue;

            GameObject newShape = new GameObject("DrawnShape");
            newShape.tag = "DrawnShape";

            // Set the transform to match the original shape
            newShape.transform.position = originalPosition;
            newShape.transform.rotation = originalRotation;
            newShape.transform.localScale = originalScale;

            // Add LineRenderer
            LineRenderer newLineRenderer = newShape.AddComponent<LineRenderer>();
            newLineRenderer.positionCount = segment.Count;
            newLineRenderer.startWidth = 0.1f;
            newLineRenderer.endWidth = 0.1f;
            newLineRenderer.material = lineMaterial;

            // Add EdgeCollider2D with points in local space
            EdgeCollider2D newEdgeCollider = newShape.AddComponent<EdgeCollider2D>();
            newEdgeCollider.points = segment.ToArray();

            // Add Rigidbody2D and preserve physics state
            Rigidbody2D newRb = newShape.AddComponent<Rigidbody2D>();
            newRb.bodyType = RigidbodyType2D.Dynamic;
            newRb.gravityScale = originalRb.gravityScale;
            newRb.mass = originalRb.mass;
            newRb.drag = originalRb.drag;
            newRb.angularDrag = originalRb.angularDrag;
            newRb.collisionDetectionMode = originalRb.collisionDetectionMode;

            // Apply the original velocities
            newRb.velocity = originalVelocity;
            newRb.angularVelocity = originalAngularVelocity;

            // Calculate and set the center of mass in local space
            Vector2 centerOfMass = CalculateCenterOfMass(segment);
            newRb.centerOfMass = centerOfMass;

            // Update the visual representation
            SynchronizeShape(newShape, newLineRenderer, newEdgeCollider);
        }
    }

    void SynchronizeShape(GameObject shape, LineRenderer lineRenderer, EdgeCollider2D edgeCollider)
    {
        Vector2[] colliderPoints = edgeCollider.points;
        lineRenderer.positionCount = colliderPoints.Length;

        for (int i = 0; i < colliderPoints.Length; i++)
        {
            // Convert local collider points to world space for the LineRenderer
            Vector3 worldPoint = shape.transform.TransformPoint(colliderPoints[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }
    }

    void ErasePartOfShape()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject[] drawnShapes = GameObject.FindGameObjectsWithTag("DrawnShape");

        foreach (GameObject shape in drawnShapes)
        {
            LineRenderer lineRenderer = shape.GetComponent<LineRenderer>();
            EdgeCollider2D edgeCollider = shape.GetComponent<EdgeCollider2D>();
            Rigidbody2D rb = shape.GetComponent<Rigidbody2D>();

            if (lineRenderer != null && edgeCollider != null && IsMouseOverShape(mousePos, shape, edgeCollider))
            {
                // Get points in local space
                Vector2[] localPoints = edgeCollider.points;
                List<List<Vector2>> newSegments = new List<List<Vector2>>();
                List<Vector2> currentSegment = new List<Vector2>();

                foreach (Vector2 localPoint in localPoints)
                {
                    // Convert point to world space for distance check
                    Vector2 worldPoint = shape.transform.TransformPoint(localPoint);

                    if (Vector2.Distance(mousePos, worldPoint) > 0.2f)
                    {
                        // Keep points in local space for the new segments
                        currentSegment.Add(localPoint);
                    }
                    else if (currentSegment.Count > 0)
                    {
                        newSegments.Add(new List<Vector2>(currentSegment));
                        currentSegment.Clear();
                    }
                }

                if (currentSegment.Count > 0)
                {
                    newSegments.Add(currentSegment);
                }

                if (newSegments.Count == 0)
                {
                    Destroy(shape);
                }
                else
                {
                    HandleLineSplitting(shape, newSegments, rb);
                }
            }
        }
    }

}