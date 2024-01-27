using UnityEditor;
using UnityEngine;

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
            _mainCamera.transform.LookAt(planet.transform);
            _planetName = "Planet";
        }
    }
}

/**
 *
 */
[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _settingsEditor;

    void OnEnable() {
        if (target) {
            _planet = (Planet)target;
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUI.changed) {
            _planet.UpdatePlanet();
        }
    }
}