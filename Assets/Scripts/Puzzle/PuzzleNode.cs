using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Puzzle/PuzzleNode")]
public class PuzzleNode : MonoBehaviour {
    public bool complete = false;
    public bool disableOnComplete = false;
    [ReadOnly]
    public bool canReceive = true;
    public PuzzleNode[] downstream = new PuzzleNode[0];
    protected List<PuzzleNode> upstream = new List<PuzzleNode>();

    public bool IsComplete() {
        return complete;
    }

    public void BaseStart() {
        foreach (PuzzleNode node in downstream) {
            if (node)
                node.upstream.Add(this);
        }
        SetCompletion(complete);
    }

    private void Start() {
        BaseStart();
    }

    public virtual void SignalDownstream(bool state) {
        if (canReceive) {
            foreach (PuzzleNode node in downstream) {
                if (node)
                    node.SetCompletion(state);
            }
        }
    }

    public virtual void SetCompletion(bool state) {
        if (canReceive) {
            if (state != complete) {
                complete = state;
                SignalDownstream(state);
                if (disableOnComplete && state) {
                    canReceive = false;
                }
            }
        }
    }

    public Color GizmoColor() {
        if (complete) {
            return Color.green;
        } else {
            return Color.grey;
        }
    }

    public void DrawBaseGizmos() {
        Gizmos.color = GizmoColor();
        foreach (PuzzleNode node in downstream) {
            if (node)
                Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }

    public void DrawBaseGizmosSelected() {
        Gizmos.color = GizmoColor();
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmosSelected() {
        DrawBaseGizmosSelected();
    }

    private void OnDrawGizmos() {
        DrawBaseGizmos();
    }
}
