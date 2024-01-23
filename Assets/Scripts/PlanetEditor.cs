using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _settingsEditor;
    bool _settingsFoldout;

    void OnEnable() {
        _planet = (Planet)target;
    }

    public override void OnInspectorGUI() {
        using EditorGUI.ChangeCheckScope check = new();
        base.OnInspectorGUI();
        bool button = GUILayout.Button("Generate Planet");
        if (check.changed || button) {
            _planet.GeneratePlanet();
            return;
        }
        _settingsFoldout = EditorGUILayout.InspectorTitlebar(_settingsFoldout, _planet.planetSettings);
        if (!_settingsFoldout) return;
        CreateCachedEditor(_planet.planetSettings, null, ref _settingsEditor);
        _settingsEditor.OnInspectorGUI();
        _planet.GeneratePlanet();
    }
}