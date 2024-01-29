using System;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour {
    public float radius;
    public float mass;
    public Vector3 initialVelocity;
    Vector3 _currentVelocity;
    Rigidbody _rigidBody;

    void Start() {
        _rigidBody = GetComponent<Rigidbody>();
        _currentVelocity = initialVelocity;
    }

    /**
     *
     */
    public void UpdateVelocity(List<CelestialBody> allBodies, float gravity) {
        foreach (CelestialBody celestialBody in allBodies) {
            if (celestialBody == this) continue;
            Vector3 distance = celestialBody._rigidBody.position - _rigidBody.position;
            float force = gravity * _rigidBody.mass * celestialBody._rigidBody.mass / distance.sqrMagnitude;
            _currentVelocity += distance.normalized * force;
        }
        _rigidBody.MovePosition(_rigidBody.position + _currentVelocity * Time.deltaTime);
    }
}