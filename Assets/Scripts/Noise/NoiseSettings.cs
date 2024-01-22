using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class NoiseSettings {
    [Range(1, 8)]
    public int numberOfLayers = 1;
    public NoiseType noiseType;
    public float strength = 1;
    public float persistence = 2;
    public float baseRoughness = 1;
    public float roughness = 2;
    public float minValue = 1;
    public Vector3 center;
}

[System.Serializable]
public enum NoiseType {
    Simple,
    Ridge
}