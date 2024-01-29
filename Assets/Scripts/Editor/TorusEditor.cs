using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Torus))]
public class TorusEditor : Editor {
    Torus _torus;
    Editor _editor;

    void OnEnable() {
        _torus = (Torus)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUI.changed) {
            _torus.UpdateTorus();
        }
    }
}