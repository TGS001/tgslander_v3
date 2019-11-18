using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    [SerializeField]
    Vector3 p;
    [SerializeField]
    bool collided;
    [SerializeField]
    bool background;

    public Point(Vector3 p, bool collided, bool background) {
        this.p = p;
        this.collided = collided;
        this.background = background;
    }

    public static Point Create(Vector3 p, bool collided, bool background)
    {
        return new Point(p, collided, background);
    }

    public Vector3 Position()
    {
        return p;
    }

    public bool IsCollided()
    {
        return collided;
    }

    public bool IsBackground()
    {
        return background & collided;
    }

    public void DrawGizmo()
    {
        if (collided)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawCube(p, Vector3.one * 0.1f);
    }
}
