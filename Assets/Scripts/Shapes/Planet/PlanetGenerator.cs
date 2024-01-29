using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public Shader shader;
    public const int PlanetTextureResolution = 50;
    public const int PlanetFaces = 6;
    public static readonly int TextureID = Shader.PropertyToID("_texture");
    public static readonly int MinMaxElevationID = Shader.PropertyToID("_elevationMinMax");
    public static readonly List<Vector3> Directions = new() {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };

    /**
    *
    */
    public Planet CreatePlanet(string planetName) {
        GameObject planetGameObject = new(planetName);
        Planet planet = planetGameObject.AddComponent<Planet>();
        planet.InitializePlanet(shader);
        planet.UpdatePlanet();
        Selection.activeObject = planet;
        return planet;
    }
}