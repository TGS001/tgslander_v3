using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FlightPathNode {
    public FlightAI.FlightMode mode;
    public Vector2 targetPosition;
    public float cruiseSpeed;
    public float radius;

    public FlightPathNode(FlightAI.FlightMode mode, Vector2 targetPosition, float cruiseSpeed, float radius) {
        this.mode = mode;
        this.targetPosition = targetPosition;
        this.cruiseSpeed = cruiseSpeed;
        this.radius = radius;
    }
}
