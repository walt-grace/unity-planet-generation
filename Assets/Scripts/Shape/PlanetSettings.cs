using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetSettings : ScriptableObject {
    public int radius;
    public Color color;
    public List<NoiseLayerSettings> noiseLayerSettings = new();
}
