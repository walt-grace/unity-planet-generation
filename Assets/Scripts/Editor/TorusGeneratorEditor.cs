using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TorusGenerator))]
public class TorusGeneratorEditor : Editor {
    Editor _settingsEditor;
    string _torusName = "Torus";
    TorusGenerator _torusGenerator;

    void OnEnable() {
        _torusGenerator = (TorusGenerator)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        _torusName = EditorGUILayout.TextField("Torus Name", _torusName);
        if (GUILayout.Button("Generate Torus")) {
            Torus torus = _torusGenerator.CreateTorus(_torusName);
            _torusName = "Torus";
        }
    }
}