using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FlightPath {
    public bool loop;
    public List<FlightPathNode> nodes;
    internal bool noApproach;

    public void SetLastNodePosition(Vector2 position) {
        if (nodes.Count > 0) {
            FlightPathNode lastNode = nodes[nodes.Count - 1];
            lastNode.targetPosition = position;
            nodes[nodes.Count - 1] = lastNode;
        }
    }

    public bool GetNode(int index, out FlightPathNode node) {
        if (loop || index < nodes.Count) {
            node = nodes[index % nodes.Count];
            return true;
        }
        node = new FlightPathNode();
        return false;
    }

    Color ModeColor(FlightAI.FlightMode mode) {
        switch (mode) {
            case FlightAI.FlightMode.verticalStay:
                return Color.cyan * 0.8f;
            case FlightAI.FlightMode.followPath:
                return Color.blue * 0.8f;
            case FlightAI.FlightMode.freeStay:
                return Color.magenta * 0.8f;
            default:
                break;
        }
        return Color.black;
    }

    internal void DrawGizmo(int pathingStage) {
        if (nodes.Count == 0) {
            return;
        }

        Vector2 lastPosition;
        int i;
        if (pathingStage > 0 && !loop) {
            lastPosition = nodes[pathingStage - 1].targetPosition;
            i = pathingStage;
        } else {
            lastPosition = nodes[nodes.Count - 1].targetPosition;
            i = 0;
        }
        for (; i < nodes.Count; i++) {
            FlightPathNode node = nodes[i];

            Gizmos.color = ModeColor(node.mode);
            if (node.radius == 0) {
                Gizmos.DrawWireSphere(node.targetPosition, 1);
            } else {
                Gizmos.DrawWireSphere(node.targetPosition, node.radius);
            }
            
            if (i > 0 || loop) {
                Gizmos.DrawLine(lastPosition, node.targetPosition);
            }
            lastPosition = node.targetPosition;
        }
    }
}
