using UnityEngine;

[System.Serializable]
public class BiomeSettings {
    public Gradient gradient = new();
    public Color tint;
    [Range(0, 1)]
    public float tintPercent;
    [Range(0, 1)]
    public float startHeight;
}