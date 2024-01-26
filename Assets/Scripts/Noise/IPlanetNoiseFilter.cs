using UnityEngine;

public interface IPlanetNoiseFilter {
    float Evaluate(Vector3 point);

    /**
     *
     */
    public static IPlanetNoiseFilter New(NoiseLayer layer) {
        return layer.noiseType switch {
            NoiseType.Simple => new SimplePlanetNoiseFilter(layer),
            NoiseType.Ridge => new RidgePlanetNoiseFilter(layer),
            _ => null
        };
    }
}