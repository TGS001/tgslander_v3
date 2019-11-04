using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Input/ObjectivePuzzleNode")]
public class ObjectivePuzzleNode : PuzzleNode {
    public ObjectiveMarker marker;

    // Update is called once per frame
    void Update() {
        if (!complete) {
            if (marker) {
                if (marker.complete)
                    SetCompletion(true);
            } else {
                SetCompletion(true);
            }
        }
    }

    private void OnDrawGizmos() {
        foreach (PuzzleNode node in downstream) {
            if (complete) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.grey;
            }
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
        if (marker) {
            if (marker.failed) {
                Gizmos.color = Color.red;
            } else if (complete) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.grey;
            }
            Gizmos.DrawLine(transform.position, marker.transform.position);
        }
    }
}
