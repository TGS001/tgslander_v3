using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Puzzle/Output/MoverPuzzleNode")]
public class MoverPuzzleNode : PuzzleNode {
    public Transform target;
    public float moveTime = 1;
    public Vector3 startOffset;
    public Vector3 endOffset;
    [Range(0, 1)]
    public float timer = 0;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public bool signalOn;


    // Update is called once per frame
    void Update () {
        if (target) {
            if (Application.isPlaying) {
                float distance = Time.deltaTime / moveTime;
                bool moving = (timer != 0 && timer != 1);
                if (signalOn) {
                    timer = Mathf.MoveTowards(timer, 1, distance);
                } else {
                    timer = Mathf.MoveTowards(timer, 0, distance);
                }
                if (moving && (timer == 0 || timer == 1)) {
                    base.SetCompletion(signalOn);
                }
            }
            target.localPosition = Vector3.Lerp(startOffset, endOffset, curve.Evaluate(timer));
        }
    }

    public override void SetCompletion(bool state) {
        signalOn = state;
    }
}
