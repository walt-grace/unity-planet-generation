using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetSettings : ScriptableObject {
    public string planetName = "Planet";
    [Range(1, 256)]
    public int resolution = 30;
    public int radius = 10;
    public List<NoiseLayer> noiseLayers = new();
    public List<BiomeSettings> biomes = new();
}


