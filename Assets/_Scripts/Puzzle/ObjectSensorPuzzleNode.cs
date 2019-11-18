using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Input/Object Sensor PuzzleNode")]
public class ObjectSensorPuzzleNode : PuzzleNode {
    [Tooltip("The object to sense.")]
    public Transform target;
    [Tooltip("The area where the object must be to complete this node.")]
    public Rect area;
    [Tooltip("If true, the node is complete when the target is outside the area. Otherwise, the node is complete when the target is in the area.")]
    public bool invert = false;

    private void Update() {
        if (target != null) {
            Vector2 test = transform.InverseTransformPoint(target.transform.position);
            SetCompletion(area.Contains(test) != invert);
        }
    }

    private void OnDrawGizmos() {
        DrawBaseGizmos();
        Gizmos.color = GizmoColor();
        Gizmos.DrawWireCube(area.center + (Vector2)transform.position, area.size);
    }
}
