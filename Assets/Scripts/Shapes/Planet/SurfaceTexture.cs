using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurfaceTexture {
    public List<BiomeSettings> biomes = new();
    public float biomeNoiseOffset;
    public float biomeNoiseStrength;
    [Range(0, 1)]
    public float biomeBlendAmount;
}