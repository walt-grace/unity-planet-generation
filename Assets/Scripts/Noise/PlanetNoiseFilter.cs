using System;
using UnityEngine;

[Serializable]
public enum NoiseType {
    Simple,
    Ridge
}

[Serializable]
public class PlanetNoiseFilter {
    public bool enabled;
    public NoiseType noiseType;
    public float strength;
    public float roughness;
    public float minValue;
    public Vector3 center;
    public bool useFirstLayerAsMask;

    public float Evaluate(Vector3 point, Noise noise) {
        return noiseType switch {
            NoiseType.Simple => EvaluateSimple(point, noise),
            NoiseType.Ridge => EvaluateRidge(point, noise),
            _ => 0
        };
    }

    /**
     *
     */
    public float EvaluateSimple(Vector3 point, Noise noise) {
        float noiseValue = 0;
        float v = noise.Evaluate(point * roughness + center);
        noiseValue += (v + 1) * .5f;
        noiseValue = Mathf.Max(0, noiseValue - minValue);
        return noiseValue * strength;
    }

    /**
     *
     */
    public float EvaluateRidge(Vector3 point, Noise noise) {
        float noiseValue = 1 - Mathf.Abs(noise.Evaluate(point * roughness + center));
        noiseValue *= noiseValue;
        noiseValue = Mathf.Max(0, noiseValue - minValue);
        return noiseValue * strength;
    }
}