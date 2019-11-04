using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    [SerializeField]
    Point a;
    [SerializeField]
    Point b;
    [SerializeField]
    Point c;
    [SerializeField]
    Vector2 centroid;

    public Triangle(Point a, Point b, Point c) {
        this.a = a;
        this.b = b;
        this.c = c;
        centroid = (a.Position() + b.Position() + c.Position())/3.0f;
    }

    public Vector2 Centroid() {
        return centroid;
    }

    public static Triangle Create(Point a, Point b, Point c)
    {
        Triangle t = new Triangle(a, b, c);
        return t;
    }


    private void Generate2Clip(Vector3 v, Vector3 clipa, Vector3 clipb, TerrainMesh.MeshHelper helper)
    {
        Vector2 va = helper.Locate(clipa, v);
        Vector2 vb = helper.Locate(clipb, v);

        helper.AddIso(va, vb);

        helper.Add(va);
        helper.Add(vb);
        helper.Add(v);
    }

    private void Generate1Clip(Vector3 va, Vector3 vb, Vector3 clip, TerrainMesh.MeshHelper helper)
    {
        Vector3 ac = helper.Locate(clip, va);
        Vector3 bc = helper.Locate(clip, vb);

        helper.AddIso(bc, ac);

        helper.Add(ac);
        helper.Add(va);
        helper.Add(bc);

        helper.Add(bc);
        helper.Add(va);
        helper.Add(vb);
    }

    private void Generate(Vector3 va, Vector3 vb, Vector3 vc, TerrainMesh.MeshHelper helper)
    {
        helper.Add(va);
        helper.Add(vb);
        helper.Add(vc);
    }

    private void Background2Clip(Vector3 v, Vector3 clipa, Vector3 clipb, TerrainMesh.MeshHelper helper)
    {
        Vector2 va = helper.LocateBackground(clipa, v);
        Vector2 vb = helper.LocateBackground(clipb, v);

        helper.AddBack(va);
        helper.AddBack(vb);
        helper.AddBack(v);
    }

    private void Background1Clip(Vector3 va, Vector3 vb, Vector3 clip, TerrainMesh.MeshHelper helper)
    {
        Vector3 ac = helper.LocateBackground(clip, va);
        Vector3 bc = helper.LocateBackground(clip, vb);

        helper.AddBack(ac);
        helper.AddBack(va);
        helper.AddBack(bc);

        helper.AddBack(bc);
        helper.AddBack(va);
        helper.AddBack(vb);
    }

    private void Background(Vector3 va, Vector3 vb, Vector3 vc, TerrainMesh.MeshHelper helper)
    {
        helper.AddBack(va);
        helper.AddBack(vb);
        helper.AddBack(vc);
    }

    public void GenerateMesh(TerrainMesh.MeshHelper helper)
    {
        bool ac = a.IsCollided();
        bool bc = b.IsCollided();
        bool cc = c.IsCollided();
        Vector3 ap = a.Position();
        Vector3 bp = b.Position();
        Vector3 cp = c.Position();

        if (helper.background && (ac || bc || cc))
        {
            bool ao = a.IsBackground();
            bool bo = b.IsBackground();
            bool co = c.IsBackground();
            if (!ao || !bo || !co)
            {
                if (ao)
                {
                    if (bo)
                    {
                        Background2Clip(cp, ap, bp, helper);
                    }
                    else if (co)
                    {
                        Background2Clip(bp, cp, ap, helper);
                    }
                    else
                    {
                        Background1Clip(bp, cp, ap, helper);
                    }
                }
                else if (bo)
                {
                    if (ao)
                    {
                        Background2Clip(cp, ap, bp, helper);
                    }
                    else if (co)
                    {
                        Background2Clip(ap, bp, cp, helper);
                    }
                    else
                    {
                        Background1Clip(cp, ap, bp, helper);
                    }
                }
                else if (co)
                {
                    if (bo)
                    {
                        Background2Clip(ap, bp, cp, helper);
                    }
                    else if (ao)
                    {
                        Background2Clip(bp, cp, ap, helper);
                    }
                    else
                    {
                        Background1Clip(ap, bp, cp, helper);
                    }
                }
                else
                {
                    Background(ap, bp, cp, helper);
                }
            }
        }

        if (!(ac && bc && cc))
        {

            if (ac)
            {
                if (bc)
                {
                    Generate2Clip(cp, ap, bp, helper);
                }
                else if (cc)
                {
                    Generate2Clip(bp, cp, ap, helper);
                }
                else
                {
                    Generate1Clip(bp, cp, ap, helper);
                }
            }
            else if (bc)
            {
                if (ac)
                {
                    Generate2Clip(cp, ap, bp, helper);
                }
                else if (cc)
                {
                    Generate2Clip(ap, bp, cp, helper);
                }
                else
                {
                    Generate1Clip(cp, ap, bp, helper);
                }
            }
            else if (cc)
            {
                if (bc)
                {
                    Generate2Clip(ap, bp, cp, helper);
                }
                else if (ac)
                {
                    Generate2Clip(bp, cp, ap, helper);
                }
                else
                {
                    Generate1Clip(ap, bp, cp, helper);
                }
            }
            else
            {
                Generate(ap, bp, cp, helper);
            }
        }
    }

    public void DrawGizmo()
    {
        //a -> b
        if (a.IsCollided() ^ b.IsCollided())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            if (a.IsCollided())
            {
                Gizmos.color = Color.black;
            }
            else
            {
                Gizmos.color = Color.white;
            }
        }
        Gizmos.DrawLine(a.Position(), b.Position());

        //c -> b
        if (c.IsCollided() ^ b.IsCollided())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            if (c.IsCollided())
            {
                Gizmos.color = Color.black;
            }
            else
            {
                Gizmos.color = Color.white;
            }
        }
        Gizmos.DrawLine(c.Position(), b.Position());

        //c -> a
        if (c.IsCollided() ^ a.IsCollided())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            if (a.IsCollided())
            {
                Gizmos.color = Color.black;
            }
            else
            {
                Gizmos.color = Color.white;
            }
        }
        Gizmos.DrawLine(c.Position(), a.Position());
    }
}
