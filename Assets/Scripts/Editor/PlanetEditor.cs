using System;
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

    void OnEnable() {
        _planetGenerator = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        _planetName = EditorGUILayout.TextField("Planet Name", _planetName);
        if (GUILayout.Button("Generate Planet")) {
            _planetGenerator.CreatePlanet(_planetName);
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
        _planet = (Planet)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUI.changed) {
            _planet.UpdatePlanet();
        }
    }
}