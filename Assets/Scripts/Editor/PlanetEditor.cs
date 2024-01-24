using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _settingsEditor;
    bool _settingsFoldout;

    void OnEnable() {
        _planet = (Planet)target;
        _planet.InitializePlanet();
    }

    public override void OnInspectorGUI() {
        using EditorGUI.ChangeCheckScope check = new();
        base.OnInspectorGUI();
        if (check.changed) {
            _planet.GeneratePlanet();
            return;
        }
        // _settingsFoldout = EditorGUILayout.InspectorTitlebar(_settingsFoldout, _planet.planetSettings);
        // if (!_settingsFoldout) return;
        CreateCachedEditor(_planet.planetSettings, null, ref _settingsEditor);
        _settingsEditor.OnInspectorGUI();
        // _planet.GeneratePlanet();
        if (GUILayout.Button("Generate Planet")) {
            _planet.GeneratePlanet();
        }
    }
}