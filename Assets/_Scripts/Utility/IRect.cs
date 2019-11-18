using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IRect : System.Object
{
    public int left;
    public int right;
    public int top;
    public int bottom;

    public int area {
        get
        {
            return width * height;
        }
    }

    public int width
    {
        get
        {
            return ((right - left) + 1);
        }
    }

    public int height
    {
        get
        {
            return ((top - bottom) + 1);
        }
    }

    public IRect(int l, int r, int t, int b)
    {
        left = l;
        right = r;
        top = t;
        bottom = b;
    }

    public bool Intersects(IRect other, int margin = 1)
    {
        return 
            other.left < (right + margin) &&
            (left - margin) < other.right &&
            other.bottom < (top + margin) &&
            (bottom - margin) < other.top;
    }

    internal bool Contains(int x, int y)
    {
        return
            x >= left && x <= right &&
            y >= bottom && y <= top;
    }
}
