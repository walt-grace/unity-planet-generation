using UnityEngine;

public class SimplePlanetNoise : IPlanetNoise {
    readonly Noise _noise = new();
    readonly NoiseLayerSettings _noiseLayerSettings;

    public SimplePlanetNoise(NoiseLayerSettings noiseLayerSettings) {
        _noiseLayerSettings = noiseLayerSettings;
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = _noiseLayerSettings.baseRoughness;
        float amplitude = 1;
        for (int i = 0; i < _noiseLayerSettings.numberOfLayers; i++) {
            frequency *= _noiseLayerSettings.roughness;
            amplitude *= _noiseLayerSettings.persistence;
            float v = _noise.Evaluate(point * frequency + _noiseLayerSettings.center);
            noiseValue += (v + 1) * .5f * amplitude;
        }

        noiseValue = Mathf.Max(0, noiseValue - _noiseLayerSettings.minValue);
        return noiseValue * _noiseLayerSettings.strength;
    }
}