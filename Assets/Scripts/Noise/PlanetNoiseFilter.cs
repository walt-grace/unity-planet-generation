using System;
using UnityEngine;

[System.Serializable]
public enum NoiseType {
    Simple,
    Ridge
}

[System.Serializable]
public class PlanetNoiseFilter {
    protected readonly Noise Noise = new();
    public bool enabled;
    public NoiseType noiseType;
    public float strength;
    public float roughness;
    public float minValue;
    public Vector3 center;
    public bool useFirstLayerAsMask;

    public float Evaluate(Vector3 point) {
        return noiseType switch {
            NoiseType.Simple => EvaluateSimple(point),
            NoiseType.Ridge => EvaluateRidge(point),
            _ => 0
        };
    }

    /**
     *
     */
    public float EvaluateSimple(Vector3 point) {
        float noiseValue = 0;
        float v = Noise.Evaluate(point * roughness + center);
        noiseValue += (v + 1) * .5f;
        noiseValue = Mathf.Max(0, noiseValue - minValue);
        return noiseValue * strength;
    }

    /**
     *
     */
    public float EvaluateRidge(Vector3 point) {
        float noiseValue = 1 - Mathf.Abs(Noise.Evaluate(point * roughness + center));
        noiseValue *= noiseValue;
        noiseValue = Mathf.Max(0, noiseValue - minValue);
        return noiseValue * strength;
    }
}