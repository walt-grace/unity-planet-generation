using System.Collections.Generic;
using UnityEngine;

public class Torus : MonoBehaviour {
    public float majorRadius = 10;
    public float minorRadius = 3;
    [Range(4, 200)]
    public int segments = 20;
    public int circlesNumber = 20;
    public float widthMultiplier = 0.2f;

    /**
     *
     */
    public void UpdateTorus() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < circlesNumber; i++) {
            GameObject circle = new("circle" + i);
            LineRenderer lineRenderer = circle.AddComponent<LineRenderer>();
            circle.transform.parent = transform;
            lineRenderer.positionCount = segments + 1;
            lineRenderer.widthMultiplier = widthMultiplier;
            float u = (float) i / circlesNumber * 2 * Mathf.PI;
            for (int j = 0; j < segments + 1; j++) {
                float v = (float) j / segments * 2 * Mathf.PI;
                Vector3 position = TorusPoint(u, v);
                lineRenderer.SetPosition(j, position);
            }
        }
    }

    /**
     *
     */
    Vector3 TorusPoint(float u, float v) {
        float x = (majorRadius + minorRadius * Mathf.Cos(v)) * Mathf.Cos(u);
        float y = (majorRadius + minorRadius * Mathf.Cos(v)) * Mathf.Sin(u);
        float z = minorRadius * Mathf.Sin(v);
        return new Vector3(x, y, z);
    }
}