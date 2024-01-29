using System;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour {
    public float gravity;
    public List<CelestialBody> planets = new();

    void Update() {
        foreach (CelestialBody celestialBody in planets) {
            celestialBody.UpdateVelocity(planets, gravity);
        }
    }
}