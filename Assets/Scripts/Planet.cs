using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Planet : MonoBehaviour {
    [Range(1, 256)]
    public int resolution = 10;
    [SerializeField, HideInInspector]
    List<MeshFilter> meshFilters = new(6);

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    float _maxElevation;
    float _minElevation;

    readonly List<Vector3> _directions = new() {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };

    /**
     *
     */
    public void GeneratePlanet() {
        GenerateMeshes();
        GenerateColors();
    }

    /**
     *
     */
    public void GenerateMeshes() {
        List<IPlanetNoise> noiseFilters = new();
        foreach (NoiseLayerSettings noiseLayerSettings in shapeSettings.noiseLayerSettings) {
            noiseFilters.Add(IPlanetNoise.New(noiseLayerSettings));
        }
        if (meshFilters.Count == 0) {
            InitializeMeshFilters();
        }
        for (int i = 0; i < meshFilters.Count; i++) {
            ConstructMesh(meshFilters[i].sharedMesh, _directions[i], noiseFilters);
        }
    }

    /**
     *
     */
    void InitializeMeshFilters() {
        for (int i = 0; i < _directions.Count; i++) {
            GameObject meshObj = new("PlanetSide");
            meshObj.transform.parent = transform;

            MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = colorSettings.planetMaterial;

            MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            meshFilters.Add(meshFilter);
        }
    }

    /**
     *
     */
    public void GenerateColors() {
        foreach (MeshFilter meshFilter in meshFilters) {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.color;
        }
    }

    /**
    *
    */
    void ConstructMesh(Mesh mesh, Vector3 localUp, List<IPlanetNoise> noiseFilters) {
        int triangleIndex = 0;
        Vector3 axisA = new(localUp.y, localUp.z, localUp.x);
        Vector3 axisB = Vector3.Cross(localUp, axisA);

        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangle = new int[(resolution - 1) * (resolution - 1) * 6];
        for (int x = 0; x < resolution; x++) {
            for (int y = 0; y < resolution; y++) {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnSphere = pointOnCube.normalized;
                vertices[i] = CalculatePointOnPlanet(pointOnSphere, noiseFilters);

                if (x == resolution - 1 || y == resolution - 1) continue;
                // Triangle 1
                triangle[triangleIndex] = i;
                triangle[triangleIndex + 1] = i + resolution + 1;
                triangle[triangleIndex + 2] = i + resolution;
                // Triangle 2
                triangle[triangleIndex + 3] = i;
                triangle[triangleIndex + 4] = i + 1;
                triangle[triangleIndex + 5] = i + resolution + 1;

                triangleIndex += 6;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();
    }

    /**
    *
    */
    Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere, List<IPlanetNoise> noiseFilters) {
        float elevation = 0;
        float firstLayerValue = 0;
        // First layer
        if (shapeSettings.noiseLayerSettings is { Count: > 0 } && shapeSettings.noiseLayerSettings[0].enabled) {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            elevation = firstLayerValue;
        }
        // The rest of the layers
        if (shapeSettings.noiseLayerSettings is { Count: > 0 }) {
            for (int i = 1; i < shapeSettings.noiseLayerSettings.Count; i++) {
                if (!shapeSettings.noiseLayerSettings[i].enabled) continue;
                float mask = shapeSettings.noiseLayerSettings[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevation = shapeSettings.radius * (elevation + 1);
        SetMinMaxElevation(elevation);
        return pointOnUnitSphere * elevation;
    }

    /**
    *
    */
    void SetMinMaxElevation(float elevation) {
        if (elevation < _minElevation) {
            _minElevation = elevation;
        } else if (elevation > _maxElevation) {
            _maxElevation = elevation;
        }
    }
}