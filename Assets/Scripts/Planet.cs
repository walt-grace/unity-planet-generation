using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    public string planetName = "Planet";
    [Range(1, 256)]
    public int resolution = 30;
    public int radius = 10;
    public List<NoiseLayer> noiseLayers = new();
    public List<BiomeSettings> biomes = new();
    public Material planetMaterial;

    readonly List<MeshFilter> _meshFilters = new(PlanetGenerator.PlanetFaces);
    public Texture2D texture;

    float _minElevation = float.MaxValue;
    float _maxElevation = float.MinValue;


    /**
     *
     */
    public void UpdatePlanet() {
        SetPlanetSides();
        SetBiomeTextures();
        planetMaterial.SetVector(PlanetGenerator.MinMaxElevationID, new Vector4(_minElevation, _maxElevation));
    }

    /**
     *
     */
    void SetPlanetSides() {
        _minElevation = float.MaxValue;
        _maxElevation = float.MinValue;
        // Noise
        List<IPlanetNoiseFilter> noiseFilters = new();
        foreach (NoiseLayer noiseLayer in noiseLayers) {
            noiseFilters.Add(IPlanetNoiseFilter.New(noiseLayer));
        }
        // Mesh
        for (int i = 0; i < PlanetGenerator.PlanetFaces; i++) {
            ConstructPlanetSide(_meshFilters[i].sharedMesh, PlanetGenerator.Directions[i], noiseFilters);
        }
    }

    /**
    *
    */
    void ConstructPlanetSide(Mesh mesh, Vector3 localUp, IReadOnlyList<IPlanetNoiseFilter> noiseFilters) {
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
    Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere, IReadOnlyList<IPlanetNoiseFilter> noiseFilters) {
        float elevation = 0;
        float firstLayerValue = 0;
        // First layer
        if (noiseLayers is { Count: > 0 } && noiseLayers[0].enabled) {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            elevation = firstLayerValue;
        }
        // The rest of the layers
        if (noiseLayers is { Count: > 0 }) {
            for (int i = 1; i < noiseLayers.Count; i++) {
                if (!noiseLayers[i].enabled) continue;
                float mask = noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevation = radius * (elevation + 1);
        SetMinMaxElevation(elevation);
        return pointOnUnitSphere * elevation;
    }

    /**
     *
     */
    void SetBiomeTextures() {
        for (int i = 0; i < PlanetGenerator.PlanetFaces; i++) {
            UpdateBiomeUV(_meshFilters[i].sharedMesh, PlanetGenerator.Directions[i]);
        }
        int biomesCount = biomes.Count;
        texture = new Texture2D(PlanetGenerator.PlanetTextureResolution, biomesCount, TextureFormat.RGBA32, false);
        List<Color> colors = new(texture.width * texture.height);
        foreach (BiomeSettings biomeSettings in biomes) {
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
    void UpdateBiomeUV(Mesh mesh, Vector3 localUp) {
        Vector3 axisA = new(localUp.y, localUp.z, localUp.x);
        Vector3 axisB = Vector3.Cross(localUp, axisA);
        Vector2[] uv = new Vector2[resolution * resolution];
        for (int x = 0; x < resolution; x++) {
            for (int y = 0; y < resolution; y++) {
                if (x == resolution - 1 || y == resolution - 1) continue;
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnSphere = pointOnCube.normalized;
                uv[i] = new Vector2(CalculateBiomePercentage(pointOnSphere), 0);
            }
        }
        mesh.uv = uv;
    }

    /**
     *
     */
    float CalculateBiomePercentage(Vector3 point) {
        float heightPercent = (point.y + 1) / 2f;
        int biomesCount = biomes.Count;
        for (int i = 0; i < biomesCount; i++) {
            BiomeSettings biomeSettings = biomes[i];
            if (biomeSettings.startHeight < heightPercent) {
                return (float) i / Mathf.Max(1, biomesCount - 1);
            }
        }
        return 0;
    }

    /**
     *
     */
    public void InitializePlanet(Shader shader) {
        planetMaterial = new Material(shader);
        SetupPlanetSides();
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
    void SetMinMaxElevation(float elevation) {
        if (elevation < _minElevation) {
            _minElevation = elevation;
        }
        if (elevation > _maxElevation) {
            _maxElevation = elevation;
        }
    }
}