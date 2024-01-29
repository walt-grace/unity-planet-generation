using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor {
    Editor _settingsEditor;
    string _planetName = "Planet";
    PlanetGenerator _planetGenerator;

    void OnEnable() {
        _planetGenerator = (PlanetGenerator)target;
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