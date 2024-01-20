using System;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _shapeEditor;
    Editor _colorEditor;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        using EditorGUI.ChangeCheckScope check = new();
        if (check.changed) {
            _planet.GeneratePlanet();
        }
        if (GUILayout.Button("Generate Planet")) {
            _planet.GeneratePlanet();
        }
        DrawSettingsEditor(_shapeEditor, _planet.OnShapeSettingsUpdated, ref _planet.shapeSettingsFoldout, ref _shapeEditor);
        DrawSettingsEditor(_colorEditor, _planet.OnColorSettingsUpdated, ref _planet.colourSettingsFoldout, ref _colorEditor);
    }

    /**
     *
     */
    void DrawSettingsEditor(Object settings, Action onSettingsUpdate, ref bool foldout, ref Editor editor) {
        if (settings == null || onSettingsUpdate == null || !foldout) return;
        using EditorGUI.ChangeCheckScope check = new();
        Editor editor2 = CreateEditor(settings);
        editor2.OnInspectorGUI();
        if (!check.changed) return;
        onSettingsUpdate();
    }

    void OnEnable() {
        _planet = (Planet) target;
    }
}