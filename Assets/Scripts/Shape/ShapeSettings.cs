using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject {
    public int radius;
    public List<NoiseLayerSettings> noiseLayerSettings = new();
}
