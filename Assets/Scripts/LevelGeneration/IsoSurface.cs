using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IsoSurface : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    public List<IsoIsland> islands;
    [SerializeField]
    [HideInInspector]
    public List<Vector2> points;

    public class Segment
    {
        public int a;
        public int b;
        public bool used = false;
    }

    public void SetPoints(Vector2[] newPoints)
    {
        if ((newPoints.Length % 2) == 0)
        {
            {
                IsoSurfaceRegion[] reg = GetComponentsInChildren<IsoSurfaceRegion>();
                foreach (IsoSurfaceRegion region in reg)
                {
                    DestroyImmediate(region);
                }
            }
            if (islands == null)
            {
                islands = new List<IsoIsland>();
            }
            else
            {
                islands.Clear();
            }

            if (points == null)
            {
                points = new List<Vector2>();
            }
            else
            {
                points.Clear();
            }

            Segment[] segments = new Segment[newPoints.Length / 2];
            Map<Vector2, int> indexMap = new Map<Vector2, int>();
            for (int i = 0; i < segments.Length; i++)
            {
                Vector2 pointa = newPoints[i * 2];
                Vector2 pointb = newPoints[i * 2 + 1];
                Segment seg = new Segment();
                if (indexMap.ContainsKey(pointa))
                {
                    seg.a = indexMap[pointa];
                }
                else
                {
                    indexMap.Add(pointa, points.Count);
                    seg.a = points.Count;
                    points.Add(pointa);
                }

                if (indexMap.ContainsKey(pointb))
                {
                    seg.b = indexMap[pointb];
                }
                else
                {
                    indexMap.Add(pointb, points.Count);
                    seg.b = points.Count;
                    points.Add(pointb);
                }
                segments[i] = seg;
            }

            for (int i = 0; i < segments.Length; i++)
            {
                if (!segments[i].used)
                {
                    segments[i].used = true;
                    List<SurfaceSegment> surf = new List<SurfaceSegment>();
                    SurfaceSegment cur = SurfaceSegment.Create(segments[i]);
                    surf.Add(cur);
                    int head = cur.b;
                    int tail = cur.a;
                    //expand along head
                    while (true)
                    {
                        Segment seg = null;
                        for (int j = i; j < segments.Length; j++)
                        {
                            if (segments[j].used == false && segments[j].a == head)
                            {
                                seg = segments[j];
                            }
                        }

                        if (seg == null)
                        {
                            break;
                        }

                        head = seg.b;
                        seg.used = true;
                        surf.Add(SurfaceSegment.Create(seg));
                    }

                    //expand along tail
                    if (head != tail)
                    {
                        while (true)
                        {
                            Segment seg = null;
                            for (int j = i; j < segments.Length; j++)
                            {
                                if (segments[j].used == false && segments[j].b == tail)
                                {
                                    seg = segments[j];
                                }
                            }

                            if (seg == null)
                            {
                                break;
                            }

                            tail = seg.a;
                            seg.used = true;
                            surf.Add(SurfaceSegment.Create(seg));
                        }
                    }

                    islands.Add(IsoIsland.Create(surf.ToArray(), points));
                }
            }
        }
    }

    public IsoIsland GetClosestIsland(Vector2 point)
    {
        if (islands.Count == 0)
        {
            return null;
        }
        float bestDist = float.MaxValue;
        IsoIsland bestIsland = null;
        foreach (IsoIsland island in islands)
        {
            SurfaceSegment seg = island.GetClosestSegment(point);
            float dist = island.SegmentDistance(seg, point);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestIsland = island;
            }
        }
        return bestIsland;
    }

    public void OnDrawGizmosSelected()
    {
        Color[] colors = {
         Color.blue,
         Color.cyan,
         Color.green,
         Color.magenta,
         Color.red,
         Color.yellow
      };

        int seg = 0;
        foreach (IsoIsland island in islands)
        {
            island.Gizmo(colors[seg++ % colors.Length]);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IsoSurface))]
public class IsoSurfaceEditor : Editor
{
    bool create = false;
    SurfaceSegment segment = null;
    IsoIsland island = null;
    Vector2 point;

    void StartCreate()
    {
        create = true;
        Repaint();
    }

    void StopCreate()
    {
        create = false;
        segment = null;
        island = null;
        point = Vector2.zero;
        Repaint();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        IsoSurface targ = (IsoSurface)target;
        switch (Event.current.type)
        {
            case EventType.MouseUp:
                if (create == true)
                {
                    Plane xplane = new Plane(Vector3.forward, Vector3.zero);
                    Camera c = SceneView.lastActiveSceneView.camera;
                    Vector3 mp = new Vector3(Event.current.mousePosition.x, c.pixelHeight - Event.current.mousePosition.y);
                    Ray r = c.ScreenPointToRay(mp);
                    float enter;
                    Vector2 pickPoint = Vector2.zero;
                    if (xplane.Raycast(r, out enter))
                    {
                        pickPoint = r.GetPoint(enter);
                    }
                    if (Event.current.button == 0)
                    {
                        if (island == null)
                        {
                            island = targ.GetClosestIsland(pickPoint);
                            segment = island.GetClosestSegment(pickPoint);
                            point = island.SegmentPoint(segment, pickPoint);
                            SceneView.RepaintAll();
                            Repaint();
                        }
                        else
                        {
                            SurfaceSegment seg = island.GetClosestSegment(pickPoint);
                            Vector2 pointb = island.SegmentPoint(seg, pickPoint);
                            GameObject region = new GameObject("Region");
                            IsoSurfaceRegion sr = region.AddComponent<IsoSurfaceRegion>();
                            sr.island = island;
                            sr.startSegment = segment;
                            sr.endSegment = seg;
                            sr.startOffset = island.SegmentProgress(segment, point);
                            sr.endOffset = island.SegmentProgress(seg, pointb);
                            sr.FixRegion();
                            sr.transform.SetParent(targ.transform);
                            StopCreate();
                            Selection.activeGameObject = region;
                        }
                    }
                    else
                    {
                        StopCreate();
                    }
                }
                break;
        }

        if (island != null)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(point, Vector3.forward, 0.5f);
        }
    }

    override public void OnInspectorGUI()
    {
        if (create == true)
        {
            if (island == null)
            {
                EditorGUILayout.LabelField("Pick a start point");
            }
            else
            {
                EditorGUILayout.LabelField("Pick an end point");
            }
            if (GUILayout.Button("Cancel"))
            {
                StopCreate();
            }
        }
        else
        {
            if (GUILayout.Button("Create Region"))
            {
                StartCreate();
            }
        }
    }
}
#endif
