using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/Filter/TimerPuzzleNode")]
public class TimerPuzzleNode : PuzzleNode {
    public float delay;
    public bool instantComplete = false;
    public bool instantIncomplete = true;
    public PuzzleNode[] indicators;
    IEnumerator timeoutFunc;

    void SetAllIndicators(bool state) {
        foreach (PuzzleNode node in indicators) {
            node.SetCompletion(state);
        }
    }

    public override void SetCompletion(bool state) {
        if (timeoutFunc != null) {
            StopCoroutine(timeoutFunc);
        }
        if (state) {
            if (instantComplete) {
                base.SetCompletion(state);
                SetAllIndicators(state);
            } else {
                SetAllIndicators(!state);
                timeoutFunc = Timeout(state);
                StartCoroutine(timeoutFunc);
            }
        } else {
            if (instantIncomplete) {
                base.SetCompletion(state);
                SetAllIndicators(state);
            } else {
                SetAllIndicators(!state);
                timeoutFunc = Timeout(state);
                StartCoroutine(timeoutFunc);
            }
        }
    }
    private void OnDrawGizmos() {
        foreach (PuzzleNode node in downstream) {
            if (complete) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.grey;
            }
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
        foreach (PuzzleNode node in indicators) {
            if (node) {
                if (node.complete) {
                    Gizmos.color = Color.green;
                } else {
                    Gizmos.color = Color.grey;
                }
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }

    IEnumerator Timeout(bool state) {
        if (indicators.Length == 0) {
            yield return new WaitForSeconds(delay);
            SetAllIndicators(state);
        } else {
            float wait = delay / (indicators.Length + 1);
            for (int i = 0; i < indicators.Length; i++) {
                yield return new WaitForSeconds(wait);
                indicators[i].SetCompletion(state);
            }
            yield return new WaitForSeconds(wait);
        }
        
        base.SetCompletion(state);
    }
}
