using UnityEngine;

public class SimplePlanetNoiseFilter : IPlanetNoiseFilter {
    readonly Noise _noise = new();
    readonly NoiseLayer _noiseLayer;

    public SimplePlanetNoiseFilter(NoiseLayer noiseLayer) {
        _noiseLayer = noiseLayer;
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = _noiseLayer.baseRoughness;
        float amplitude = 1;
        for (int i = 0; i < _noiseLayer.numberOfLayers; i++) {
            frequency *= _noiseLayer.roughness;
            amplitude *= _noiseLayer.persistence;
            float v = _noise.Evaluate(point * frequency + _noiseLayer.center);
            noiseValue += (v + 1) * .5f * amplitude;
        }

        noiseValue = Mathf.Max(0, noiseValue - _noiseLayer.minValue);
        return noiseValue * _noiseLayer.strength;
    }
}