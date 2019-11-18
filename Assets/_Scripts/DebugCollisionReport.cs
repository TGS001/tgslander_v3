using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollisionReport : MonoBehaviour {

    public float indicatorSize = 1;
    public float indicatorTime = 2;

    private void DrawCollision(Vector2 position, Vector2 normal, Color c) {
        Debug.DrawRay(position, normal * indicatorSize, c, indicatorTime);
    }

    void ReportCollision(Collision2D collision, Color col, string report) {
        Debug.Log(report);
        foreach (ContactPoint2D c in collision.contacts) {
            DrawCollision(c.point, c.normal, col);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        ReportCollision(collision, Color.red, "Collision Started");
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ReportCollision(collision, Color.yellow, "Collision");
    }

    private void OnCollisionExit2D(Collision2D collision) {
        ReportCollision(collision, Color.blue, "Collision Ended");
    }
}
