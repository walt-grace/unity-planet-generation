using UnityEngine;

[System.Serializable]
public class BiomeSettings {
    public Gradient gradient = new();
    [Range(0, 1)]
    public float startHeight;
    [Range(0, 1)]
    public float endHeight;
}