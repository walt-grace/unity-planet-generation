using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SolarSystem))]
public class SolarSystemEditor : Editor {
    Editor _settingsEditor;
    SolarSystem _solarSystem;

    void OnEnable() {
        _solarSystem = (SolarSystem)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Planet")) {
        }
    }
}