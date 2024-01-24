using UnityEngine;

public interface IPlanetNoise {
    float Evaluate(Vector3 point);

    /**
     *
     */
    public static IPlanetNoise New(NoiseLayer layer) {
        return layer.noiseType switch {
            NoiseType.Simple => new SimplePlanetNoise(layer),
            NoiseType.Ridge => new RidgePlanetNoise(layer),
            _ => null
        };
    }
}