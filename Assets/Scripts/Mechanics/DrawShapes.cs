using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Platformer.Mechanics;
using UnityEngine;

public class DrawShapes : MonoBehaviour
{
    private bool isDrawingMode = false;
    private LineRenderer currentLineRenderer;
    private List<Vector2> points = new List<Vector2>();
    public Material lineMaterial;
    public UnityEngine.UI.Image drawingModeIndicator;
    public Sprite drawingModeOnImage;
    public Sprite drawingModeOffImage;

    private GameObject shapeObject;

    public PlayerController playerController;
    [SerializeField] Texture2D mouseDrawIcon;
    [SerializeField] Texture2D mouseEraseIcon;
    
    private float maxDrawingRange = 4.0f;
    [SerializeField] private LineRenderer rangeIndicator; 
    private Vector2 drawingStartPosition;

    
    void Start()
    {
        isDrawingMode = false;
        if (rangeIndicator != null)
        {
            // Configure the circle
            rangeIndicator.startWidth = 0.05f;
            rangeIndicator.endWidth = 0.05f;
            DrawCircle(rangeIndicator, maxDrawingRange);
            rangeIndicator.gameObject.SetActive(false); // Hide the circle initially
        }
    }
    
    void Update()
    {

        // Toggle drawing mode with Shift key
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isDrawingMode = !isDrawingMode;
            UpdateDrawingModeUI();
            
            // Enable or disable player movement
            playerController.controlEnabled = !isDrawingMode;
            
            // Show or hide the circle range
            if (rangeIndicator != null)
            {
                rangeIndicator.gameObject.SetActive(isDrawingMode);
            }
        }

        if (isDrawingMode)
        {
            // Mouse down to start drawing
            if (Input.GetMouseButtonDown(0) && playerController.getCurrentGauge() > 0)
            {
                //UnityEngine.Debug.Log($"Mouse Button Down - Drawing Mode: {isDrawingMode}, Gauge: {playerController.getCurrentGauge()}");
                CreateNewLine();
                //UnityEngine.Debug.Log("How");
                Cursor.SetCursor(mouseDrawIcon, Vector2.zero, CursorMode.Auto);
            }

            // Mouse hold to draw
            if (Input.GetMouseButton(0) && playerController.getCurrentGauge() > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                AddPointToLine(mousePos);
            }

            // Right-click to erase part of the shape
            if (Input.GetMouseButton(1))
            {
                ErasePartOfShape();
                Cursor.SetCursor(mouseEraseIcon, Vector2.zero, CursorMode.Auto);
            }

        }
        
        // Mouse up to finish drawing
        if (Input.GetMouseButtonUp(0))
        {
            if (points.Count > 2)
            {
                CreateEdgeCollider();

                GameObject[] drawnShapes2 = GameObject.FindGameObjectsWithTag("DrawnShape");
                foreach (GameObject shape in drawnShapes2)
                {
                    LineRenderer lineRenderer = shape.GetComponent<LineRenderer>();
                    EdgeCollider2D edgeCollider = shape.GetComponent<EdgeCollider2D>();


                    if (lineRenderer != null && lineRenderer.sharedMaterial == null)
                    {
                        Destroy(shape);
                    }
                }
            }
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
    
    void LateUpdate()
    {
        if (isDrawingMode && rangeIndicator != null)
        {
            // Keep the circle centered on the player
            rangeIndicator.transform.position = playerController.transform.position;
        }
    }
    
    void UpdateDrawingModeUI()
    {
        if (drawingModeIndicator != null)
        {
            // Update the UI sprite based on the drawing mode state
            drawingModeIndicator.sprite = isDrawingMode ? drawingModeOnImage : drawingModeOffImage;
        }
    }

    void CreateNewLine()
    {
        //UnityEngine.Debug.Log("Creating new shape object");
        shapeObject = new GameObject("DrawnShape");
        shapeObject.tag = "DrawnShape";
        //UnityEngine.Debug.Log($"Created object: {shapeObject.name}, Instance ID: {shapeObject.GetInstanceID()}");

        currentLineRenderer = shapeObject.AddComponent<LineRenderer>();
        currentLineRenderer.positionCount = 0;
        currentLineRenderer.startWidth = 0.1f;
        currentLineRenderer.endWidth = 0.1f;
        //currentLineRenderer.material = lineMaterial;
        currentLineRenderer.sharedMaterial = lineMaterial;
        //UnityEngine.Debug.Log($"Material: {currentLineRenderer.sharedMaterial}");

        points.Clear();

        drawingStartPosition = (Vector2)playerController.transform.position;
    }
    
    void DrawCircle(LineRenderer lineRenderer, float radius)
    {
        if (lineRenderer == null)
        {
            return;
        }

        int segments = 50; // Number of segments in the circle
        lineRenderer.positionCount = segments + 1; // Extra point to close the circle
        lineRenderer.loop = true; // Ensures the circle loops
        lineRenderer.useWorldSpace = false; // Relative to the player

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += 360f / segments;
        }
    }

    void AddPointToLine(Vector2 newPoint)
    {
        // Check if the new point is within the maximum drawing range
        float distanceFromStart = Vector2.Distance(drawingStartPosition, newPoint);
        if (distanceFromStart > maxDrawingRange)
        {
            return; // Do not allow drawing outside the range
        }
        
        // Check if the new point overlaps with any colliders tagged as "level"
        Collider2D hit = Physics2D.OverlapPoint(newPoint);
        if (hit != null && hit.CompareTag("level"))
        {
            UnityEngine.Debug.Log("Cannot draw on level objects.");
            return; // Stop adding points if overlapping with level objects
        }

        // Ensure we only add points if they are spaced apart to prevent clutter
        if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], newPoint) > 0.1f)
        {
            // Deduct gauge based on the distance between points
            if (points.Count > 1)
            {
                float distanceBetweenPoints = Vector2.Distance(points[points.Count - 1] * 1.01f, newPoint);
                playerController.DeltaGauge(-distanceBetweenPoints); // Reduce gauge
            }

            // Add the point and update the LineRenderer
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
            //newLineRenderer.material = lineMaterial;
            newLineRenderer.sharedMaterial = lineMaterial;


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

            if (lineRenderer != null && lineRenderer.sharedMaterial == null)
            {
                Destroy(shape);
            }
        }
    }

}