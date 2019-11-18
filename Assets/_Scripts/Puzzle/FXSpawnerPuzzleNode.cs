using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Puzzle/Output/FXSpawnerPuzzleNode")]
public class FXSpawnerPuzzleNode : PuzzleNode {
    public SFX onFX;
    public SFX offFX;
    public float magnitude;
    public float size;

    public override void SetCompletion(bool state) {
        if (state != complete) {
            SFX fx;
            if (state) {
                fx = SFX.Spawn(onFX, transform);
            } else {
                fx = SFX.Spawn(offFX, transform);
            }
            fx.size = size;
            fx.magnitude = magnitude;
            fx.normal = transform.forward;
        }
        base.SetCompletion(state);
    }
}
