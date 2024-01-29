using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor {
    Editor _settingsEditor;
    string _planetName = "Planet";
    bool _asCelestialBody;
    PlanetGenerator _planetGenerator;

    void OnEnable() {
        _planetGenerator = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        _planetName = EditorGUILayout.TextField("Planet Name", _planetName);
        _asCelestialBody = EditorGUILayout.Toggle("With Celestial Body", _asCelestialBody);
        if (GUILayout.Button("Generate Planet")) {
            Planet planet = _planetGenerator.CreatePlanet(_planetName, _asCelestialBody);
            _planetName = "Planet";
        }
    }
}