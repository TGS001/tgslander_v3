using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Output/FailPuzzleNode")]
public class FailPuzzleNode : PuzzleNode {
    public Sprite icon;
    public string reason;
    public string description;
    public override void SetCompletion(bool state) {
        base.SetCompletion(state);
        if (state) {
            PlaySessionControl.Lose(icon, reason, description);
        }
    }
}
