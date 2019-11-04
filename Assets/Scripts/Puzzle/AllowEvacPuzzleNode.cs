using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Output/AllowEvacPuzzleNode")]
public class AllowEvacPuzzleNode : PuzzleNode {
    public override void SetCompletion(bool state) {
        base.SetCompletion(state);
        PlaySessionControl.AllowEvac(state);
    }
}
