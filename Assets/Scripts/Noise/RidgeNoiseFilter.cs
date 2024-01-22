using UnityEngine;

public class RidgeNoiseFilter : INoiseFilter {
    readonly Noise _noise = new();
    readonly NoiseSettings _noiseSettings;

    public RidgeNoiseFilter(NoiseSettings noiseSettings) {
        _noiseSettings = noiseSettings;
    }

    public float Evaluate(Vector3 point) {
        float frequency = _noiseSettings.baseRoughness;
        float weight = 1;
        float amplitude = 1;
        float noiseValue = 0;
        for (int i = 0; i < _noiseSettings.numberOfLayers; i++) {
            float v = 1 - Mathf.Abs(_noise.Evaluate(point * frequency + _noiseSettings.center));
            v *= v;
            v *= weight;
            weight = v;
            noiseValue += v * amplitude;
            frequency *= _noiseSettings.roughness;
            amplitude *= _noiseSettings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - _noiseSettings.minValue);
        return noiseValue * _noiseSettings.strength;
    }
}