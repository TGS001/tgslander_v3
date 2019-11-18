using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileGroup : MonoBehaviour
{
    public float tileWidth = 1;
    public float tileHeight = 1;
    

    public bool CanModify(IRect newBounds, TileSub tileSub)
    {
        foreach (TileSub sub in subs)
        {
            if (sub.Equals(tileSub))
                continue;
            if ( sub.bounds.Intersects(newBounds))
            {
                sub.RandomColor();
                return false;
            }
        }
        return true;
    }

    public GameObject top, mid, bottom, topRight, midRight, bottomRight, topLeft, midLeft, bottomLeft, topCenter, midCenter, bottomCenter;

    private GameObject GetTileFor(bool left, bool right, GameObject l, GameObject m, GameObject r, GameObject def)
    {
        if (left && right)
        {
            return m;
        }
        else if (left)
        {
            return r;
        }
        else if (right)
        {
            return l;
        }
        return def;
    }

    public void RegenerateNeighbors(TileSub sub)
    {
        foreach (TileSub s in subs)
        {
            if (sub.IsNeighbor(s))
            {
                s.Regenerate();
                s.RandomColor();
            }
        }
    }

    public GameObject GetTile(bool up, bool down, bool left, bool right)
    {
        if (up && down)
        {
            //mid
            return GetTileFor(left, right, midLeft, midCenter, midRight, mid);
        }
        else if (up)
        {
            //bottom
            return GetTileFor(left, right, bottomLeft, bottomCenter, bottomRight, bottom);
        }
        //top
        return GetTileFor(left, right, topLeft, topCenter, topRight, top);
    }

    [SerializeField]
    [HideInInspector]
    List<TileSub> subs = new List<TileSub>();

    public bool IsFilled(int x, int y)
    {
        foreach (TileSub sub in subs)
        {
            if (sub != null && sub.Contains(x, y))
            {
                return true;
            }
        }
        return false;
    }

    public void TileToWorld(int x, int y, out Vector2 pos)
    {
        pos = new Vector2(x * tileWidth, y * tileHeight);
    }

    public void WorldToTile(Vector2 pos, out int x, out int y)
    {
        x = Mathf.FloorToInt((pos.x / tileWidth));
        y = Mathf.FloorToInt((pos.y / tileHeight));
    }

    public void AddSub(Vector3 pos)
    {
        int x, y;
        Vector2 rp = transform.InverseTransformPoint(pos);
        WorldToTile(rp, out x, out y);
        if (!IsFilled(x, y))
        {
            TileSub sub = TileSub.create(x, y, 1, 1, this);
            subs.Add(sub);
            RegenerateNeighbors(sub);
        }
    }

    public void Clear()
    {
        foreach (TileSub sub in subs)
        {
            sub.Clear();
        }
        subs.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        foreach (TileSub sub in subs)
        {
            if (sub != null)
            sub.DoGizmos();
        }
    }

    public void DrawHandles()
    {
        foreach (TileSub sub in subs)
        {
            if (sub != null)
            sub.DoHandles();
        }
    }

    internal void RemoveSub(Vector3 pos)
    {
        int x, y;
        Vector2 rp = transform.InverseTransformPoint(pos);
        WorldToTile(rp, out x, out y);
        TileSub sub = GetSubAt(x, y);
        if (sub != null)
        {
            sub.Clear();
            subs.Remove(sub);
            RegenerateNeighbors(sub);
            DestroyImmediate(sub);
        }
    }

    private TileSub GetSubAt(int x, int y)
    {
        foreach (TileSub sub in subs)
        {
            if (sub.Contains(x, y))
            {
                return sub;
            }
        }
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TileGroup))]
public class TileGroupEditor : Editor
{
    bool addingSub = false;
    private bool removingSub = false;

    private void OnSceneGUI()
    {
        TileGroup t = (TileGroup)target;
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (addingSub)
                {
                    Plane cp = new Plane(t.transform.forward, t.transform.position);
                    Camera c = SceneView.lastActiveSceneView.camera;
                    Ray r = c.ScreenPointToRay(new Vector3(e.mousePosition.x, c.pixelHeight - e.mousePosition.y));
                    float enter = 0;

                    if (cp.Raycast(r, out enter))
                    {
                        t.AddSub(r.GetPoint(enter));
                    }
                    addingSub = false;
                    Repaint();
                    e.Use();
                }
                if (removingSub)
                {
                    Plane cp = new Plane(t.transform.forward, t.transform.position);
                    Camera c = SceneView.lastActiveSceneView.camera;
                    Ray r = c.ScreenPointToRay(new Vector3(e.mousePosition.x, c.pixelHeight - e.mousePosition.y));
                    float enter = 0;

                    if (cp.Raycast(r, out enter))
                    {
                        t.RemoveSub(r.GetPoint(enter));
                    }
                    removingSub = false;
                    Repaint();
                    e.Use();
                }
                break;
            default:
                break;
        }
        t.DrawHandles();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TileGroup t = (TileGroup)target;
        if (!addingSub && !removingSub)
        {
            if (GUILayout.Button("place new sub rect"))
            {
                addingSub = true;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("delete sub rect"))
            {
                removingSub = true;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("reset subs"))
            {
                t.Clear();
                EditorUtility.SetDirty(t);
            }
        }
    }
}
#endif
