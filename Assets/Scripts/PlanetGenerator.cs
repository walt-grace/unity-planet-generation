using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public Shader shader;
    public const int PlanetTextureResolution = 50;
    public const int PlanetFaces = 6;
    public static readonly int TextureID = Shader.PropertyToID("_texture");
    public static readonly int MinMaxElevationID = Shader.PropertyToID("_elevationMinMax");
    public static readonly List<Vector3> Directions = new() {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };

    /**
    *
    */
    Planet CreatePlanet(string planetName) {
        GameObject planetGameObject = new(planetName);
        Planet planet = planetGameObject.AddComponent<Planet>();
        planet.InitializePlanet(shader);
        planet.UpdatePlanet();
        Selection.activeObject = planet;
        return planet;
    }

    /**
     *
     */
    [CustomEditor(typeof(PlanetGenerator))]
    public class PlanetGeneratorEditor : Editor {
        Editor _settingsEditor;
        string _planetName = "Planet";
        PlanetGenerator _planetGenerator;
        Camera _mainCamera;

        void OnEnable() {
            _planetGenerator = (PlanetGenerator)target;
            _mainCamera = Camera.main;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            _planetName = EditorGUILayout.TextField("Planet Name", _planetName);
            if (GUILayout.Button("Generate Planet")) {
                Planet planet = _planetGenerator.CreatePlanet(_planetName);
                // _mainCamera.transform.LookAt(planet.transform);
                _planetName = "Planet";
            }
        }
    }
}