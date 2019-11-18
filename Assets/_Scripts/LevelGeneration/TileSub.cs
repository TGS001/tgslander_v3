using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileSub : ScriptableObject
{
    public TileGroup owner;
    public IRect bounds;
    public GameObject[] tiles;
    public Color debugColor;

    public void DoGizmos()
    {
        Vector2 bottomLeft, topRight;
        owner.TileToWorld(bounds.left, bounds.bottom, out bottomLeft);
        owner.TileToWorld(bounds.right + 1, bounds.top + 1, out topRight);
        Transform t = owner.transform;
        Gizmos.color = debugColor;

        Vector3 tr = t.TransformPoint(topRight);
        Vector3 bl = t.TransformPoint(bottomLeft);
        Vector3 br = t.TransformPoint(new Vector2(topRight.x, bottomLeft.y));
        Vector3 tl = t.TransformPoint(new Vector2(bottomLeft.x, topRight.y));

        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(tl, bl);
        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(bl, br);
    }

    public void RandomColor()
    {
        debugColor = UnityEngine.Random.ColorHSV();
    }

    public void DoHandles()
    {
#if UNITY_EDITOR
        Vector2 bottomLeft, topRight;
        float otw = owner.tileWidth;
        float oth = owner.tileHeight;
        Vector2 offset = new Vector2(otw * 0.25f, oth * 0.25f);
        Transform t = owner.transform;
        owner.TileToWorld(bounds.left, bounds.bottom, out bottomLeft);
        owner.TileToWorld(bounds.right + 1, bounds.top + 1, out topRight);

        Vector2 blHandleStart = bottomLeft + offset;
        Vector3 bottomLeftHandle = t.TransformPoint(blHandleStart);

        Vector2 trHandleStart = topRight - offset;
        Vector3 topRightHandle = t.TransformPoint(trHandleStart);

        float handleScale = Mathf.Min(otw, oth) * Mathf.Min(t.lossyScale.x, t.lossyScale.y) * 0.2f;


        Handles.color = Color.green;

        Vector2 blHandleRes = t.InverseTransformPoint(
            Handles.FreeMoveHandle(bottomLeftHandle, t.rotation, handleScale, Vector3.zero, Handles.CircleHandleCap)
            );

        Vector2 trHandleRes = t.InverseTransformPoint(
            Handles.FreeMoveHandle(topRightHandle, t.rotation, handleScale, Vector3.zero, Handles.CircleHandleCap)
            );

        int dl = Mathf.FloorToInt(((blHandleRes.x - blHandleStart.x) / otw) + 0.5f);
        int dr = Mathf.FloorToInt(((trHandleRes.x - trHandleStart.x) / otw) + 0.5f);
        int dt = Mathf.FloorToInt(((trHandleRes.y - trHandleStart.y) / oth) + 0.5f);
        int db = Mathf.FloorToInt(((blHandleRes.y - blHandleStart.y) / oth) + 0.5f);

        if (dl != 0 || dr != 0 || dt != 0 || db != 0)
        {
            Modify(new IRect(bounds.left + dl, bounds.right + dr, bounds.top + dt, bounds.bottom + db));
        }
#endif
    }

    public static TileSub create(int x, int y, int w, int h, TileGroup owner)
    {
        TileSub sub = ScriptableObject.CreateInstance<TileSub>();
        sub.owner = owner;
        sub.bounds = new IRect(x, x + w - 1, y + h - 1, y);
        sub.debugColor = Color.gray;
        return sub;
    }

    public void Clear()
    {
        if (tiles != null)
        {
            foreach (GameObject o in tiles)
            {
                if (o != null)
                    DestroyImmediate(o);
            }
        }
    }

    public void Regenerate()
    {
        if (tiles != null)
        {
            foreach (GameObject o in tiles)
            {
                if (o != null)
                    DestroyImmediate(o);
            }
        }

        tiles = new GameObject[bounds.area];
        int i = 0;
        for (int u = bounds.left; u <= bounds.right; u++)
        {
            int j = 0;
            for (int v = bounds.bottom; v <= bounds.top; v++)
            {
                bool up = Contains(u, v + 1) || owner.IsFilled(u, v + 1);
                bool down = Contains(u, v - 1) || owner.IsFilled(u, v - 1);
                bool left = Contains(u - 1, v) || owner.IsFilled(u - 1, v);
                bool right = Contains(u + 1, v) || owner.IsFilled(u + 1, v);
                GameObject tile = owner.GetTile(up, down, left, right);
                if (tile)
                {
                    tile = Instantiate(tile, owner.transform);
                    Vector2 pos;
                    owner.TileToWorld(u, v, out pos);
                    tile.transform.localPosition = pos;
                    tiles[i + j * bounds.width] = tile;
                }
                j++;
            }
            i++;
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(owner);
#endif
    }

    public void Modify(IRect nbounds)
    {
        if (nbounds.width < 1 || nbounds.height < 1 || !owner.CanModify(nbounds, this))// || !owner.CanModify(dx, dy, dw, dh, this))
        {
            return;
        }
        bounds = nbounds;
        owner.RegenerateNeighbors(this);
    }

    internal bool Contains(int x, int y)
    {
        return bounds.Contains(x, y);
    }

    internal bool IsNeighbor(TileSub s)
    {
        return s.bounds.Intersects(bounds, 3);
    }
}
