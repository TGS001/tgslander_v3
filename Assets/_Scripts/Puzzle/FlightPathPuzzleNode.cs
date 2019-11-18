using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Puzzle/Output/FlightPathPuzzleNode")]
public class FlightPathPuzzleNode : PuzzleNode {
    public FlightAI flyer;
    public FlightPath path;

    private void OnDrawGizmosSelected() {
        path.DrawGizmo(0);
    }

    public override void SetCompletion(bool state) {
        if (state) {
            flyer.ExecutePath(path);
        }
        base.SetCompletion(state);
    }
}
