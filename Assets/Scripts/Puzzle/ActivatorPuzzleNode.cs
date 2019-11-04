using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Puzzle/Output/ActivatorPuzzleNode")]
public class ActivatorPuzzleNode : PuzzleNode {
    public GameObject activeOnComplete;
    public GameObject inactiveOnComplete;

    private void Start() {
        if (activeOnComplete)
            activeOnComplete.SetActive(complete);
        if (inactiveOnComplete)
            inactiveOnComplete.SetActive(!complete);
    }

    public override void SetCompletion(bool state) {
        if (activeOnComplete)
            activeOnComplete.SetActive(state);
        if (inactiveOnComplete)
            inactiveOnComplete.SetActive(!state);
        base.SetCompletion(state);
    }
}
