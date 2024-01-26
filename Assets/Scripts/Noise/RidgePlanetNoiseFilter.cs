using UnityEngine;

public class RidgePlanetNoiseFilter : IPlanetNoiseFilter {
    readonly Noise _noise = new();
    readonly NoiseLayer _noiseLayer;

    public RidgePlanetNoiseFilter(NoiseLayer noiseLayer) {
        _noiseLayer = noiseLayer;
    }

    public float Evaluate(Vector3 point) {
        float frequency = _noiseLayer.baseRoughness;
        float weight = 1;
        float amplitude = 1;
        float noiseValue = 0;
        for (int i = 0; i < _noiseLayer.numberOfLayers; i++) {
            float v = 1 - Mathf.Abs(_noise.Evaluate(point * frequency + _noiseLayer.center));
            v *= v;
            v *= weight;
            weight = v;
            noiseValue += v * amplitude;
            frequency *= _noiseLayer.roughness;
            amplitude *= _noiseLayer.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - _noiseLayer.minValue);
        return noiseValue * _noiseLayer.strength;
    }
}