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
    public void GeneratePlanet() {
        SetPlanetSides();
        SetBiomeTextures();
    }

    /**
     *
     */
    public void InitializePlanet(PlanetSettings newPlanetSettings) {
        planetSettings = newPlanetSettings;
        planetMaterial = Resources.Load<Material>("PlanetMaterial");
        SetupPlanetSides();
    }

    /**
     *
     */
    void SetPlanetSides() {
        // Noise
        List<IPlanetNoise> noiseFilters = new();
        foreach (NoiseLayer noiseLayer in planetSettings.noiseLayers) {
            noiseFilters.Add(IPlanetNoise.New(noiseLayer));
        }
        // Mesh
        _minElevation = float.MaxValue;
        _maxElevation = float.MinValue;
        for (int i = 0; i < _meshFilters.Count; i++) {
            MeshFilter meshFilter = _meshFilters[i];
            ConstructPlanetSide(meshFilter.sharedMesh, _directions[i], noiseFilters);
        }
        planetMaterial.SetVector(PlanetGenerator.MinMaxElevationID, new Vector4(_minElevation, _maxElevation));
    }

    /**
    *
    */
    void ConstructPlanetSide(Mesh mesh, Vector3 localUp, IReadOnlyList<IPlanetNoise> noiseFilters) {
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
    void SetupPlanetSides() {
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
    void SetBiomeTextures() {
        int biomesCount = planetSettings.biomes.Count;
        texture = new Texture2D(PlanetGenerator.PlanetTextureResolution, biomesCount);
        List<Color> colors = new(texture.width * texture.height);
        foreach (BiomeSettings biomeSettings in planetSettings.biomes) {
            for (int i = 0; i < PlanetGenerator.PlanetTextureResolution; i++) {
                Color gradientColor = biomeSettings.gradient.Evaluate(i / (PlanetGenerator.PlanetTextureResolution - 1f));
                Color color = gradientColor * (1 - biomeSettings.tintPercent) + biomeSettings.tint * biomeSettings.tintPercent;
                colors.Add(color);
            }
        }
        texture.SetPixels(colors.ToArray());
        texture.Apply();
        planetMaterial.SetTexture(PlanetGenerator.TextureID, texture);
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