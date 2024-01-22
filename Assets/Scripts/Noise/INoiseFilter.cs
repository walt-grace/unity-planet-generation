using System;
using UnityEngine;

public interface INoiseFilter {
    float Evaluate(Vector3 point);

    /**
     *
     */
    public static INoiseFilter Instance(NoiseSettings settings) {
        return settings.noiseType switch {
            NoiseType.Simple => new SimpleNoiseFilter(settings),
            NoiseType.Ridge => new RidgeNoiseFilter(settings),
            _ => null
        };
    }
}