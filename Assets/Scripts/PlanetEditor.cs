using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _shapeEditor;
    Editor _colorEditor;

    void OnEnable() {
        _planet = (Planet)target;
    }

    public override void OnInspectorGUI() {
        using EditorGUI.ChangeCheckScope check = new();
        base.OnInspectorGUI();
        if (check.changed) _planet.GeneratePlanet();
        if (GUILayout.Button("Generate Planet")) _planet.GeneratePlanet();
        DrawSettingsEditor(_planet.shapeSettings, _planet.OnShapeSettingsUpdated, ref _planet.shapeSettingsFoldout, ref _shapeEditor);
        DrawSettingsEditor(_planet.colorSettings, _planet.OnColorSettingsUpdated, ref _planet.colourSettingsFoldout, ref _colorEditor);
    }

    /**
     *
     */
    void DrawSettingsEditor(Object settings, Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
        if (settings == null || onSettingsUpdated == null) return;
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        using EditorGUI.ChangeCheckScope check = new();
        if (!foldout) return;
        CreateCachedEditor(settings, null, ref editor);
        editor.OnInspectorGUI();
        if (!check.changed) return;
        onSettingsUpdated.Invoke();
    }
}