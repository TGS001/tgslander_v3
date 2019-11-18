using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Input/PlayerSensorPuzzleNode")]
public class PlayerSensorPuzzleNode : PuzzleNode {
    ModularLander lander;
    public Rect area;
	// Use this for initialization
	void Start () {
        lander = FindObjectOfType<ModularLander>();
        if (!lander) {
            enabled = false;
        }
        BaseStart();
	}

    private void Update() {
        if (!lander) {
            this.enabled = false;
            return;
        }
        Vector2 test = transform.InverseTransformPoint(lander.transform.position);
        SetCompletion(area.Contains(test));
    }

    private void OnDrawGizmos() {
        DrawBaseGizmos();
        Gizmos.color = GizmoColor();
        Gizmos.DrawWireCube(area.center + (Vector2)transform.position, area.size);
    }
}
