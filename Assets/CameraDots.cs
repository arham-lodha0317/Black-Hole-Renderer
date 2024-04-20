using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDots : MonoBehaviour
{
    public int columns = 5;  // Number of points horizontally
    public int rows = 5;     // Number of points vertically
    public float sphereRadius = 0.1f;  // Radius of each sphere drawn

    private Vector3[,] points;  // Array to hold point positions

    void Start()
    {
        points = new Vector3[rows, columns]; // Initialize the points array
        UpdatePoints(); // Initial update to setup points
    }

    void Update()
    {
        UpdatePoints(); // Update the points every frame to reflect camera changes
    }

    void UpdatePoints()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * cam.aspect;
        Vector3 bottomLeft = new Vector3(-width / 2, -height / 2, cam.nearClipPlane);

        float xSpacing = width / (columns - 1);
        float ySpacing = height / (rows - 1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 pointPosition = bottomLeft + new Vector3(j * xSpacing, i * ySpacing, 0);
                points[i, j] = cam.transform.TransformPoint(pointPosition); // Transform to world space
            }
        }
    }

    void OnDrawGizmos()
    {
        if (points == null)
            return;

        Gizmos.color = Color.red; // Set the color of the Gizmos

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Gizmos.DrawSphere(points[i, j], sphereRadius);
            }
        }
    }
}
