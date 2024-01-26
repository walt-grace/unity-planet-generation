using UnityEngine;

[System.Serializable]
public enum NoiseType {
    Simple,
    Ridge
}

[System.Serializable]
public class NoiseLayer {
    public bool enabled;
    [Range(1, 8)]
    public int numberOfLayers;
    public bool useFirstLayerAsMask;
    public NoiseType noiseType;
    public float strength;
    public float persistence;
    public float baseRoughness;
    public float roughness;
    public float minValue;
    public Vector3 center;


}