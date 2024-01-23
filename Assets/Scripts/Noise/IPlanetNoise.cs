using System;
using UnityEngine;

public interface IPlanetNoise {
    float Evaluate(Vector3 point);

    /**
     *
     */
    public static IPlanetNoise New(NoiseLayerSettings layerSettings) {
        return layerSettings.noiseType switch {
            NoiseType.Simple => new SimplePlanetNoise(layerSettings),
            NoiseType.Ridge => new RidgePlanetNoise(layerSettings),
            _ => null
        };
    }
}