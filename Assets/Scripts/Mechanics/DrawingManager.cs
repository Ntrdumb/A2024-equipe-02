using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public GameObject linePrefab;
    public LayerMask drawableLayerMask;  // Only allow drawing on this layer
    private LineRenderer currentLine;
    private List<Vector2> points = new List<Vector2>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UnityEngine.Debug.Log("Mouse button down detected at position: " + mousePos);

            if (IsDrawableArea(mousePos))
            {
                UnityEngine.Debug.Log("Drawable area detected, starting drawing.");
                StartDrawing(mousePos);
            }
            else
            {
                UnityEngine.Debug.Log("Non-drawable area detected.");
            }
        }

        if (Input.GetMouseButton(0) && currentLine != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UnityEngine.Debug.Log("Mouse is being held and moved to position: " + mousePos);

            if (IsDrawableArea(mousePos))
            {
                UpdateDrawing(mousePos);
            }
        }

        if (Input.GetMouseButtonUp(0) && currentLine != null)
        {
            UnityEngine.Debug.Log("Mouse button released, ending drawing.");
            EndDrawing();
        }
    }

    bool IsDrawableArea(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, drawableLayerMask);
        bool isDrawable = hit.collider != null;
        UnityEngine.Debug.Log("Raycast hit detected: " + (isDrawable ? "Yes" : "No"));
        return isDrawable;
    }

    void StartDrawing(Vector2 startPos)
    {
        GameObject lineObject = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = lineObject.GetComponent<LineRenderer>();
        points.Clear();
        points.Add(startPos);
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, startPos);
        UnityEngine.Debug.Log("Started drawing at position: " + startPos);
    }

    void UpdateDrawing(Vector2 newPoint)
    {
        if (Vector2.Distance(points[points.Count - 1], newPoint) > 0.1f)  // Minimum distance to avoid clutter
        {
            points.Add(newPoint);
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, newPoint);
            UnityEngine.Debug.Log("Drawing updated with new point: " + newPoint);
        }
    }

    void EndDrawing()
    {
        UnityEngine.Debug.Log("Drawing ended, creating polygon collider.");
        // Convert line into a polygonal shape with collider
        PolygonCollider2D polygonCollider = currentLine.gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.points = points.ToArray();

        currentLine = null;
    }
}
