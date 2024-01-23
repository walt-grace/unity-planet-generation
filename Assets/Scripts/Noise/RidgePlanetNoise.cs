using UnityEngine;

public class RidgePlanetNoise : IPlanetNoise {
    readonly Noise _noise = new();
    readonly NoiseLayerSettings _noiseLayerSettings;

    public RidgePlanetNoise(NoiseLayerSettings noiseLayerSettings) {
        _noiseLayerSettings = noiseLayerSettings;
    }

    public float Evaluate(Vector3 point) {
        float frequency = _noiseLayerSettings.baseRoughness;
        float weight = 1;
        float amplitude = 1;
        float noiseValue = 0;
        for (int i = 0; i < _noiseLayerSettings.numberOfLayers; i++) {
            float v = 1 - Mathf.Abs(_noise.Evaluate(point * frequency + _noiseLayerSettings.center));
            v *= v;
            v *= weight;
            weight = v;
            noiseValue += v * amplitude;
            frequency *= _noiseLayerSettings.roughness;
            amplitude *= _noiseLayerSettings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - _noiseLayerSettings.minValue);
        return noiseValue * _noiseLayerSettings.strength;
    }
}