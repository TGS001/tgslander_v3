using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Puzzle/Output/RotatorPuzzleNode")]
public class RotatorPuzzleNode : PuzzleNode {
    public Transform target;
    public float rotateTime = 1;
    public Vector3 startAngle;
    public Vector3 rotateAngle;
    [Range(0,1)]
    public float timer = 0;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public bool signalOn;

    // Update is called once per frame
    void Update () {
        if (target) {
            if (Application.isPlaying) {
                float distance = Time.deltaTime / rotateTime;
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
            target.localEulerAngles = Vector3.Lerp(startAngle, rotateAngle, curve.Evaluate(timer));
        }
	}

    public override void SetCompletion(bool state) {
        signalOn = state;
    }
}
