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
        if (GUI.changed) {
            _planet.GeneratePlanet();
        }
    }
}

/**
 *
 */
[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor {
    Editor _settingsEditor;
    PlanetSettings _planetSettings;

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
