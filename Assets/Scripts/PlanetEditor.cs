using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _shapeEditor;
    Editor _colorEditor;
    bool _shapeSettingsFoldout;
    bool _colorSettingsFoldout;

    void OnEnable() {
        _planet = (Planet)target;
    }

    public override void OnInspectorGUI() {
        using EditorGUI.ChangeCheckScope check = new();
        base.OnInspectorGUI();
        if (check.changed) _planet.GeneratePlanet();
        if (GUILayout.Button("Generate Planet")) _planet.GeneratePlanet();
        DrawSettingsEditor(_planet.shapeSettings, _planet.GenerateMeshes, ref _shapeSettingsFoldout, ref _shapeEditor);
        DrawSettingsEditor(_planet.colorSettings, _planet.GenerateColors, ref _colorSettingsFoldout, ref _colorEditor);
    }

    /**
     *
     */
    static void DrawSettingsEditor(Object settings, Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
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