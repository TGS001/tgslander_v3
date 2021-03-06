﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Output/SpinnerPuzzleNode")]
public class SpinnerPuzzleNode : PuzzleNode {
    public Transform target;
    public float moveTime = 1;
    public Vector3 incompleteSpeed;
    public Vector3 completeSpeed;
    [Range(0, 1)]
    public float timer = 0;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public bool signalOn;
	
	// Update is called once per frame
	void Update () {
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
        target.Rotate(Vector3.Lerp(incompleteSpeed, completeSpeed, curve.Evaluate(timer)) * Time.deltaTime);
    }

    public override void SetCompletion(bool state) {
        signalOn = state;
    }
}
