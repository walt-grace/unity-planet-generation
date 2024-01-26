using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _settingsEditor;

    void OnEnable() {
        _planet = (Planet)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        using EditorGUI.ChangeCheckScope check = new();
        CreateCachedEditor(_planet.planetSettings, null, ref _settingsEditor);
        _settingsEditor.OnInspectorGUI();
        if (check.changed) {
            _planet.GeneratePlanet();
        }
    }
}

/**
 *
 */
[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor {
    PlanetGenerator _planetGenerator;
    Editor _settingsEditor;
    PlanetSettings _planetSettings;

    void OnEnable() {
        _planetGenerator = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (!_planetSettings) {
            _planetSettings = CreateInstance<PlanetSettings>();
        }
        CreateCachedEditor(_planetSettings, null, ref _settingsEditor);
        _settingsEditor.OnInspectorGUI();
        if (GUILayout.Button("Generate Planet")) {
            PlanetGenerator.CreatePlanet(_planetSettings);
            _planetSettings = null;
        }
    }
}