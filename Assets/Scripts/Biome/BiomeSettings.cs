using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class BiomeSettings : ScriptableObject {
    public Gradient gradient = new();
    [FormerlySerializedAs("tintColor")]
    public Color tint;
    [Range(0, 1)]
    public float tintPercent;
    [Range(0, 1)]
    public float startHeight;
}