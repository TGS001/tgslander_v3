using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Output/TriggerPuzzleNode")]
public class TriggerPuzzleNode : PuzzleNode {
    public Trigger trigger;

    public override void SetCompletion(bool state) {
        if (state)
            trigger.Activate(gameObject);
        base.SetCompletion(state);
    }
}
