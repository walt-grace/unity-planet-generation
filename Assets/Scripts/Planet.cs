using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    [HideInInspector]
    public PlanetSettings planetSettings;
    [HideInInspector]
    public Material planetMaterial;

    const int PlanetFaces = 6;
    readonly List<MeshFilter> _meshFilters = new(PlanetFaces);
    readonly List<MeshRenderer> _meshRenderers = new(PlanetFaces);

    float _minElevation;
    float _maxElevation;

    static readonly int MinMaxElevation = Shader.PropertyToID("_elevationMinMax");
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
        for (int i = 0; i < _meshFilters.Count; i++) {
            MeshFilter meshFilter = _meshFilters[i];
            ConstructMesh(meshFilter.sharedMesh, _directions[i], noiseFilters);
            MeshRenderer meshRenderer = _meshRenderers[i];
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
        int resolution = planetSettings.resolution;
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
        _meshFilters.Clear();
        _meshRenderers.Clear();
        for (int i = 0; i < PlanetFaces; i++) {
            GameObject planetSide = new("PlanetSide" + i);
            planetSide.transform.parent = transform;
            // Add mesh filter
            MeshFilter meshFilter = planetSide.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            _meshFilters.Add(meshFilter);
            // Add mesh renderer
            MeshRenderer meshRenderer = planetSide.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = planetMaterial;
            _meshRenderers.Add(meshRenderer);
        }
    }

    /**
    *
    */
    void SetMinMaxElevation(float elevation) {
        if (elevation < _minElevation) {
            _minElevation = elevation;
        }
        if (elevation > _maxElevation) {
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
        if (transform.childCount == PlanetFaces) return;
        InitializePlanetSides();
        GeneratePlanet();
    }
}