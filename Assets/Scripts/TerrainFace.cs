using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {
    readonly Mesh _mesh;
    readonly int _resolution;
    readonly Vector3 _localUp;
    readonly Vector3 _axisA;
    readonly Vector3 _axisB;
    readonly int _radius;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp, int radius) {
        _mesh = mesh;
        _resolution = resolution;
        _localUp = localUp;
        _radius = radius;

        _axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
        _axisB = Vector3.Cross(_localUp, _axisA);
    }

    /**
     *
     */
    public void ConstructMesh() {
        int triangleIndex = 0;
        int resolutionMinus1 = _resolution - 1;

        Vector3[] vertices = new Vector3[_resolution * _resolution];
        int[] triangle = new int[resolutionMinus1 * resolutionMinus1 * 6];

        for (int x = 0; x < _resolution; x++) {
            for (int y = 0; y < _resolution; y++) {
                int i = x + y * _resolution;
                Vector2 percent = new Vector2(x, y) / resolutionMinus1;
                Vector3 pointOnCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                Vector3 pointOnSphere = pointOnCube.normalized;
                vertices[i] = CalculatePointOnPlanet(pointOnSphere);

                if (x == resolutionMinus1 || y == resolutionMinus1) continue;
                // Triangle 1
                triangle[triangleIndex] = i;
                triangle[triangleIndex + 1] = i + _resolution + 1;
                triangle[triangleIndex + 2] = i + _resolution;
                // Triangle 2
                triangle[triangleIndex + 3] = i;
                triangle[triangleIndex + 4] = i + 1;
                triangle[triangleIndex + 5] = i + _resolution + 1;

                triangleIndex += 6;
            }
        }
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangle;
        _mesh.RecalculateNormals();
    }

    /**
     *
     */
    Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere) {
        return pointOnUnitSphere * _radius;
    }
}