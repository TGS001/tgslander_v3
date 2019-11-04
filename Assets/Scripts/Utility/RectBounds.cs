using UnityEngine;

public struct RectBounds {
    public Vector2 min;
    public Vector2 max;

    public Vector2 position {
        get {
            return min;
        }

        set {
            max = value + max - min;
            min = value;
        }
    }

    public RectBounds(Vector2 min, Vector2 max) {
        this.min = min;
        this.max = max;
    }

    public RectBounds(Vector2 point) {
        min = point;
        max = point;
    }

    public static RectBounds empty {
        get {
            return new RectBounds(Vector2.one * float.MaxValue, Vector2.one * float.MinValue);
        }
    }

    public void Encompass(Vector2 point) {
        min = Vector2.Min(min, point);
        max = Vector2.Max(max, point);
    }

    public bool Contains(Vector2 point) {
        return (point.x >= min.x && point.y >= min.y && point.x <= max.x && point.y <= max.y);
    }
}