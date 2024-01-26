using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    public PlanetSettings planetSettings;
    public Material planetMaterial;

    readonly List<MeshFilter> _meshFilters = new(PlanetGenerator.PlanetFaces);
    public Texture2D texture;

    float _minElevation = float.MaxValue;
    float _maxElevation = float.MinValue;

    readonly List<Vector3> _directions = new() {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };


    /**
     *
     */
    public void InitializePlanet(PlanetSettings newPlanetSettings) {
        planetSettings = newPlanetSettings;
        planetMaterial = Resources.Load<Material>("PlanetMaterial");
        InitializePlanetSides();
    }

    /**
     *
     */
    public void GeneratePlanet() {
        const int textureResolution = PlanetGenerator.PlanetTextureResolution;
        texture = new Texture2D(textureResolution, 1);
        List<IPlanetNoise> noiseFilters = new();
        foreach (NoiseLayer noiseLayerSettings in planetSettings.noiseLayers) {
            noiseFilters.Add(IPlanetNoise.New(noiseLayerSettings));
        }
        _minElevation = float.MaxValue;
        _maxElevation = float.MinValue;
        for (int i = 0; i < _meshFilters.Count; i++) {
            MeshFilter meshFilter = _meshFilters[i];
            ConstructMesh(meshFilter.sharedMesh, _directions[i], noiseFilters);
        }
        planetMaterial.SetVector(PlanetGenerator.MinMaxElevationID, new Vector4(_minElevation, _maxElevation));
        Color[] colors = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++) {
            colors[i] = planetSettings.gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colors);
        texture.Apply();
        planetMaterial.SetTexture(PlanetGenerator.TextureID, texture);
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
        if (planetSettings.noiseLayers is { Count: > 0 } && planetSettings.noiseLayers[0].enabled) {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            elevation = firstLayerValue;
        }
        // The rest of the layers
        if (planetSettings.noiseLayers is { Count: > 0 }) {
            for (int i = 1; i < planetSettings.noiseLayers.Count; i++) {
                if (!planetSettings.noiseLayers[i].enabled) continue;
                float mask = planetSettings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
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
        for (int i = 0; i < PlanetGenerator.PlanetFaces; i++) {
            GameObject planetSide = new("PlanetSide" + i);
            planetSide.transform.parent = transform;
            // Add mesh filter
            MeshFilter meshFilter = planetSide.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            _meshFilters.Add(meshFilter);
            // Add mesh renderer
            MeshRenderer meshRenderer = planetSide.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = planetMaterial;
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
}