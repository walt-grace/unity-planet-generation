using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _settingsEditor;

    void OnEnable() {
        _planet = (Planet)target;
        _planet.InitializePlanet();
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        using EditorGUI.ChangeCheckScope check = new();
        CreateCachedEditor(_planet.planetSettings, null, ref _settingsEditor);
        _settingsEditor.OnInspectorGUI();
        if (check.changed || GUILayout.Button("Generate Planet")) {
            _planet.GeneratePlanet();
        }
    }
}