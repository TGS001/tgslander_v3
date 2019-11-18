using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Input/LandingPuzzleNode")]
public class LandingPuzzleNode : PuzzleNode {
    public void OnLandingEnter() {
        SetCompletion(true);
    }

    public void OnLandingExit() {
        SetCompletion(false);
    }
}
