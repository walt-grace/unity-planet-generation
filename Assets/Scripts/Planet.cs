using System;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    [Range(1, 256)]
    public int resolution = 10;
    [SerializeField]
    List<MeshFilter> meshFilters = new(6);
    [SerializeField]
    List<MeshRenderer> meshRenderers = new(6);

    public PlanetSettings planetSettings;
    public Material planetMaterial;

    float _maxElevation;
    float _minElevation;
    static readonly int MinMaxElevation = Shader.PropertyToID("_minMaxElevation");

    readonly List<Vector3> _directions = new() {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };

    /**
     *
     */
    public void GeneratePlanet() {
        List<IPlanetNoise> noiseFilters = new();
        foreach (NoiseLayerSettings noiseLayerSettings in planetSettings.noiseLayerSettings) {
            noiseFilters.Add(IPlanetNoise.New(noiseLayerSettings));
        }
        _minElevation = 0;
        _maxElevation = 0;
        for (int i = 0; i < meshFilters.Count; i++) {
            MeshFilter meshFilter = meshFilters[i];
            ConstructMesh(meshFilter.sharedMesh, _directions[i], noiseFilters);
            MeshRenderer meshRenderer = meshRenderers[i];
            meshRenderer.sharedMaterial.color = planetSettings.color;
        }
        planetMaterial.SetVector(MinMaxElevation, new Vector4(_minElevation, _maxElevation));
    }

    /**
    *
    */
    void ConstructMesh(Mesh mesh, Vector3 localUp, IReadOnlyList<IPlanetNoise> noiseFilters) {
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
    Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere, IReadOnlyList<IPlanetNoise> noiseFilters) {
        float elevation = 0;
        float firstLayerValue = 0;
        // First layer
        if (planetSettings.noiseLayerSettings is { Count: > 0 } && planetSettings.noiseLayerSettings[0].enabled) {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            elevation = firstLayerValue;
        }
        // The rest of the layers
        if (planetSettings.noiseLayerSettings is { Count: > 0 }) {
            for (int i = 1; i < planetSettings.noiseLayerSettings.Count; i++) {
                if (!planetSettings.noiseLayerSettings[i].enabled) continue;
                float mask = planetSettings.noiseLayerSettings[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevation = planetSettings.radius * (elevation + 1);
        SetMinMaxElevation(elevation);
        return pointOnUnitSphere * elevation;
    }

    /**
     *
     */
    void InitializePlanetSides() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        transform.DetachChildren();
        meshRenderers.Clear();
        meshFilters.Clear();
        for (int i = 0; i < 6; i++) {
            GameObject meshObj = new("PlanetSide");
            meshObj.transform.parent = transform;
            // Add mesh filter
            MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            meshFilters.Add(meshFilter);
            // Add mesh renderer
            MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = planetMaterial;
            meshRenderers.Add(meshRenderer);
        }
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

    /**
     *
     */
    public void InitializePlanet() {
        if (!planetSettings) {
            planetSettings = ScriptableObject.CreateInstance<PlanetSettings>();
        }
        if (!planetMaterial) {
            planetMaterial = Resources.Load<Material>("PlanetMaterial");
        }
        if (transform.childCount != 6) {
            InitializePlanetSides();
        }
    }
}