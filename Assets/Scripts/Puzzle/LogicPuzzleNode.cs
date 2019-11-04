using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Puzzle/Filter/LogicPuzzleNode")]
public class LogicPuzzleNode : PuzzleNode {
    public enum LogicFunction {
        any,
        all
    }
    public LogicFunction function;
    public bool invert = false;
    public PuzzleNode[] indicators;

    private void OnDrawGizmosSelected() {
        Vector2 origin = transform.position + Vector3.up;
        Gizmos.color = Color.black;
        foreach (PuzzleNode node in downstream) {
            Gizmos.DrawLine(origin, node.transform.position);
        }

        Gizmos.color = Color.red;
        foreach (PuzzleNode node in upstream) {
            Gizmos.DrawLine(origin, node.transform.position);
        }
    }


    // Use this for initialization
    void Start() {
        BaseStart();
        updateIndicators(int.MaxValue);
    }

    void updateIndicators(int offCount) {
        for (int i = 0; i < indicators.Length; i++) {
            indicators[i].SetCompletion(i < indicators.Length - offCount);
        }
    }

    public override void SetCompletion(bool state) {
        if (state != complete) {
            switch (function) {
                case LogicFunction.any:
                    foreach (PuzzleNode node in upstream) {
                        if (node.complete) {
                            base.SetCompletion(!invert);
                            updateIndicators(0);
                            return;
                        }
                    }
                    base.SetCompletion(invert);
                    updateIndicators(int.MaxValue);
                    break;
                case LogicFunction.all:
                    int incompletes = 0;
                    foreach (PuzzleNode node in upstream) {
                        if (!node.complete) {
                            incompletes++;
                        }
                    }
                    updateIndicators(incompletes);
                    if (invert) {
                        base.SetCompletion(incompletes != 0);
                        break;
                    }
                    base.SetCompletion(incompletes == 0);
                    break;
                default:
                    break;
            }
        }
    }
}
