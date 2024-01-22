using UnityEngine;

public class Planet : MonoBehaviour {
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    ShapeGenerator[] _terrainFaces;

    [Range(1, 256)]
    public int resolution = 10;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    const int MeshFiltersNumber = 6;

    /**
     *
     */
    void Initialize() {
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[MeshFiltersNumber];
        }

        _terrainFaces = new ShapeGenerator[MeshFiltersNumber];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int i = 0; i < MeshFiltersNumber; i++) {
            if (!meshFilters[i]) {
                GameObject meshObj = new("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            _terrainFaces[i] = new ShapeGenerator(meshFilters[i].sharedMesh, resolution, directions[i], shapeSettings);
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
        foreach (ShapeGenerator terrainFace in _terrainFaces) {
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
    public void OnShapeSettingsUpdated() {
        Initialize();
        GenerateMesh();
    }

    /**
     *
     */
    public void OnColorSettingsUpdated() {
        Initialize();
        GenerateColors();
    }
}