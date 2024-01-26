using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public const int PlanetTextureResolution = 50;
    public const int PlanetFaces = 6;
    public static readonly int TextureID = Shader.PropertyToID("_texture");
    public static readonly int MinMaxElevationID = Shader.PropertyToID("_elevationMinMax");

    /**
    *
    */
    public static void CreatePlanet(PlanetSettings planetSettings) {
        GameObject planetGameObject = new(planetSettings.planetName);
        Planet planet = planetGameObject.AddComponent<Planet>();
        planet.InitializePlanet(planetSettings);
        planet.GeneratePlanet();
    }

}