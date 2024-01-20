using UnityEngine;

public class Planet : MonoBehaviour {
    [Range(0, 256)]
    public int resolution = 10;
    const int MeshFiltersNumber = 6;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] _terrainFaces;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    /**
     *
     */
    void Initialize() {
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[MeshFiltersNumber];
        }
        _terrainFaces = new TerrainFace[MeshFiltersNumber];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int i = 0; i < MeshFiltersNumber; i++) {
            if (meshFilters[i] == null) {
                GameObject meshObj = new("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            _terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i], shapeSettings.radius);
        }
    }

    /**
     *
     */
    public void GeneratePlanet() {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    /**
     *
     */
    void GenerateMesh() {
        foreach (TerrainFace terrainFace in _terrainFaces) {
            terrainFace.ConstructMesh();
        }
    }
    /**
     *
     */
    void GenerateColors() {
        foreach (MeshFilter meshFilter in meshFilters) {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.color;
        }
    }

    /**
     *
     */
    public void OnColorSettingsUpdated() {
        Initialize();
        GenerateMesh();
    }

    /**
     *
     */
    public void OnShapeSettingsUpdated() {
        Initialize();
        GenerateColors();
    }
}