using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSegment : ScriptableObject {
    [SerializeField]
    Vector2 a;
    [SerializeField]
    Vector2 b;
    [SerializeField]
    Vector2 n;
    [SerializeField]
    float ra;
    [SerializeField]
    float rb;
    [SerializeField]
    float dist;
    [SerializeField]
    private Vector2 min;
    [SerializeField]
    private Vector2 max;

    void UpdateBounds() {
        min.x = Mathf.Min(a.x - ra, b.x - rb);
        min.y = Mathf.Min(a.y - ra, b.y - rb);

        max.x = Mathf.Max(a.x + ra, b.x + rb);
        max.y = Mathf.Max(a.y + ra, b.y + rb);
    }

    public Vector2 positionA {
        get {
            return a;
        }

        set {
            a = value;
            n = (b - a);
            dist = n.magnitude;
            n.Normalize();
            UpdateBounds();
        }
    }

    public Vector2 positionB {
        get {
            return b;
        }

        set {
            b = value;
            n = (b - a);
            dist = n.magnitude;
            n.Normalize();
            UpdateBounds();
        }
    }

    public float radiusA {
        get {
            return ra;
        }

        set {
            ra = value;
            UpdateBounds();
        }
    }

    public float radiusB {
        get {
            return rb;
        }

        set {
            rb = value;
            UpdateBounds();
        }
    }

    public float GetT(Vector2 point) {
        Vector2 bridge = point - a;
        return Mathf.Clamp01(Vector2.Dot(bridge, n) / dist);
    }

    public Vector2 ClosestPoint(Vector2 point) {
        float t = GetT(point);
        return Vector2.Lerp(a, b, t);
    }

    public float Distance(Vector2 point) {
        float t = GetT(point);
        Vector2 segPoint = Vector2.Lerp(a, b, t);
        float segRadius = Mathf.Lerp(ra, rb, t);
        return (segPoint - point).magnitude - segRadius;
    }

    internal bool InBounds(Vector2 position, float radius) {
        return
            position.x + radius > min.x &&
            position.x - radius < max.x &&
            position.y + radius > min.y &&
            position.y - radius < max.y;
    }

    internal Vector2 ModifyPosition(Vector2 position, float radius) {
        float t = GetT(position);
        Vector2 p = Vector2.LerpUnclamped(a, b, t);
        float segRadius = Mathf.LerpUnclamped(ra, rb, t);

        float distance = Vector2.Distance(position, p);
        float interference = 1 - ((segRadius - distance) / (radius));
        Vector2 mp;

        if (interference > 0) {
            mp = Vector2.MoveTowards(position, p, interference * radius * 0.5f);
        } else {
            mp = position;
        }
        //Debug.DrawLine(p, mp, Color.yellow);
        //Debug.DrawLine(mp, position, Color.blue);
        //Debug.DrawRay(position + Vector2.right, Vector2.up * interference, Color.red);
        return mp;
    }

    public Vector2 GetCameraTarget(Vector2 point) {
        float t = GetT(point);
        Vector2 segPoint = Vector2.Lerp(a, b, t);
        float segRadius = Mathf.Lerp(ra, rb, t);
        float distance = (point - segPoint).magnitude;

        return Vector2.Lerp(segPoint, point, 0.25f);
    }

    public void Gizmo(Color c) {
        Gizmos.color = c;
        Vector3 pa = Maths.tov3(a);
        Vector3 pb = Maths.tov3(b);
        Gizmos.DrawLine(pa, pb);
        Gizmos.DrawWireSphere(pa, ra);
        Gizmos.DrawWireSphere(pb, rb);
    }
}
