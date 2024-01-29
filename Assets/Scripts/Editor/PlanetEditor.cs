using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    Planet _planet;
    Editor _editor;

    void OnEnable() {
        if (target) {
            _planet = (Planet)target;
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUI.changed) {
            _planet.UpdatePlanet();
        }
    }
}