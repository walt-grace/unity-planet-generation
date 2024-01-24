using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetSettings : ScriptableObject {
    [Range(1, 256)]
    public int resolution = 30;
    public int radius = 10;
    public Color color;
    public List<NoiseLayerSettings> noiseLayerSettings = new();
}
