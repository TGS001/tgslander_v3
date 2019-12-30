

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainMesh : MonoBehaviour { 
    public IsoSurface isoSurface;
    public Transform colliderContainer;
    public CameraConstraints constraints;

    public bool multiMaterial;
    public GameObject meshContainer;
    public Material foregroundMaterial;
    public Material backgroundMaterial;
    public MeshFilter foreground;
    public MeshFilter background;
    public MeshFilter shadow;

    public FlightPathMap pathingMap;
    Collider2D[] backgroundColliders;

    public float meshSegmentSize = 10;

    public float ceiling = 3;
    public float margin = 1;
    public float chunkSize = 1;
    public float distort = 0.5f;

    public float gradientSize = 2;
    public float gradientDepth = 8;
    public AnimationCurve gradientOffset = AnimationCurve.Linear(0, 0, 1, 1);
    public Gradient gradient;

    public Transform backgroundCutout;
    public float backgroundInset = 2;
    public float backgroundDistance = 6;
    public float backgroundDepth = 3;
    public AnimationCurve backgroundCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public float textureSize = 1;

    // Creating the lists for where the zones are and what zones are connected
    public List<Zone> zones = new List<Zone>();
    public List<ZoneLink> links = new List<ZoneLink>();


    [HideInInspector]
    [SerializeField]
    private Perlin xdistort;
    [HideInInspector]
    [SerializeField]
    private Perlin ydistort;

    private List<LinkCollider> linkColliders;
    
    private List<Point> points;
    private List<Triangle> triangles;

    private Map<Vector2, List<LinkCollider>> lcHash;
    private float lcHashDistance;
    Vector2 ToLcHashKey(Vector2 v) {
        Vector2 res = v;
        res /= lcHashDistance;
        res.x = Mathf.Floor(res.x);
        res.y = Mathf.Floor(res.y);
        return res;
    }

    void SetuplcHash() {
        lcHashDistance = backgroundDistance;
        linkColliders = new List<LinkCollider>();
        bool[] zonesUsed = new bool[zones.Count];
        lcHash = new Map<Vector2, List<LinkCollider>>();
        for (int i = 0; i < links.Count; i++) {
            ZoneLink link = links[i];
            zonesUsed[link.a] = true;
            zonesUsed[link.b] = true;
            Zone a = GetZone(link.a);
            Zone b = GetZone(link.b);
            linkColliders.Add(LinkCollider.Create(a, b));
        }

        for (int i = 0; i < zonesUsed.Length; i++) {
            if (!zonesUsed[i]) {
                Zone z = zones[i];
                linkColliders.Add(LinkCollider.Create(z, z));
            }
        }

        foreach (LinkCollider lc in linkColliders) {
            Vector2 hs = ToLcHashKey(lc.min - Vector2.one * lcHashDistance);
            Vector2 he = ToLcHashKey(lc.max + Vector2.one * lcHashDistance);
            int xmin = Mathf.FloorToInt(hs.x);
            int ymin = Mathf.FloorToInt(hs.y);
            int xmax = Mathf.FloorToInt(he.x);
            int ymax = Mathf.FloorToInt(he.y);

            for (int y = ymin; y <= ymax; y++) {
                for (int x = xmin; x <= xmax; x++) {
                    Vector2 hk = new Vector2(x, y);
                    if (!lcHash.ContainsKey(hk)) {
                        lcHash.Add(hk, new List<LinkCollider>());
                    }
                    List<LinkCollider> cl = lcHash[hk];
                    cl.Add(lc);
                }
            }
        }
    }

    List<LinkCollider> GetLocalLinkColliders(Vector2 p) {
        Vector2 hk = ToLcHashKey(p);
        if (lcHash.ContainsKey(hk)) {
            return lcHash[ToLcHashKey(p)];
        }
        return null;
    }

    [HideInInspector]
    [SerializeField]
    private Vector2 min;
    [HideInInspector]
    [SerializeField]
    private Vector2 max;
    [HideInInspector]
    [SerializeField]
    private Vector2 perlinSize = Vector2.one;

    private void OnValidate() {
        meshSegmentSize = Mathf.Max(meshSegmentSize, chunkSize * 4);
        for (int i = 0; i < links.Count;) {
            if (links[i].a >= zones.Count || links[i].b >= zones.Count) {
                links.RemoveAt(i);
                continue;
            }
            i++;
        }
    }

    public class MeshHelper : UnityEngine.Object {
        class EdgeInteraction {
            Vector2 A;
            Vector2 B;
            float T;

            public EdgeInteraction(Vector2 a, Vector2 b, float t = 0) {
                A = a;
                B = b;
                T = t;
            }

            public float GetEdge(Vector2 v) {
                if (v.Equals(A)) {
                    return T;
                }
                if (v.Equals(B)) {
                    return 1 - T;
                }
                return -0.5f;
            }
        }

        class EdgeKey : System.Object, IEqualityComparer<EdgeKey> {
            Vector2 a, b;
            public EdgeKey(Vector2 a = new Vector2(), Vector2 b = new Vector2()) {
                this.a = a;
                this.b = b;
            }

            public bool Equals(EdgeKey x, EdgeKey y) {
                if (x.a == y.a) {
                    if (x.b == y.b) {
                        return true;
                    }
                } else if (x.a == y.b) {
                    if (x.b == y.a) {
                        return true;
                    }
                }
                return false;
            }

            public int GetHashCode(EdgeKey obj) {
                return obj.a.GetHashCode() + obj.b.GetHashCode();
            }
        }

        List<Vector3> vertex;
        List<Vector2> uv;
        List<int> triangles;
        Map<Vector2, int> simplify;
        Map<EdgeKey, EdgeInteraction> edges;

        List<Vector3> bVertex;
        List<Vector2> bUv;
        List<int> bTriangles;
        Map<Vector2, int> bSimplify;
        Map<EdgeKey, EdgeInteraction> bEdges;

        List<Vector2> isoSurface;
        float invTextureSize;
        TerrainMesh owner;
        public bool background;
        //hacckkk
        IsoSurface retainedSurface;

        public MeshHelper(TerrainMesh owner, bool bg) {
            vertex = new List<Vector3>();
            uv = new List<Vector2>();
            triangles = new List<int>();
            simplify = new Map<Vector2, int>();
            edges = new Map<EdgeKey, EdgeInteraction>(new EdgeKey());

            bVertex = new List<Vector3>();
            bUv = new List<Vector2>();
            bTriangles = new List<int>();
            bSimplify = new Map<Vector2, int>();
            bEdges = new Map<EdgeKey, EdgeInteraction>(new EdgeKey());

            isoSurface = new List<Vector2>();
            invTextureSize = 1 / owner.textureSize;
            background = bg;
            this.owner = owner;
        }

        public void Add(Vector2 vert) {
            int trip;
            if (simplify.ContainsKey(vert)) {
                trip = simplify[vert];
            } else {
                trip = vertex.Count;
                simplify.Add(vert, trip);
                vertex.Add(vert);
                uv.Add(new Vector2(vert.x * invTextureSize, vert.y * invTextureSize));
            }
            triangles.Add(trip);
        }

        internal void AddBack(Vector2 vert) {
            int trip;
            if (bSimplify.ContainsKey(vert)) {
                trip = bSimplify[vert];
            } else {
                trip = bVertex.Count;
                bSimplify.Add(vert, trip);
                bVertex.Add(vert);
                bUv.Add(new Vector2(vert.x * invTextureSize, vert.y * invTextureSize));
            }
            bTriangles.Add(trip);
        }

        public void AddIso(Vector3 a, Vector3 b) {
            isoSurface.Add(new Vector2(a.x, a.y));
            isoSurface.Add(new Vector2(b.x, b.y));
        }

        public Vector3 Locate(Vector3 solid, Vector3 open) {
            EdgeKey key = new EdgeKey(solid, open);
            if (edges.ContainsKey(key)) {
                EdgeInteraction edge = edges[key];
                if (edge != null) {
                    float t = edge.GetEdge(solid);
                    return Vector3.Lerp(solid, open, t);
                }
            }
            float min = 0;
            float max = 1;
            float mid = 0;
            Vector3 result = Vector3.zero;
            while (max - min > 0.001f) {
                mid = (min + max) * 0.5f;
                result = Vector3.LerpUnclamped(solid, open, mid);
                if (owner.TestPoint(result.x, result.y)) {
                    min = mid;
                } else {
                    max = mid;
                }
            }
            edges.Add(new EdgeKey(solid, open), new EdgeInteraction(solid, open, mid));
            return result;
        }

        public Vector3 LocateBackground(Vector3 solid, Vector3 open) {
            EdgeKey key = new EdgeKey(solid, open);
            if (bEdges.ContainsKey(key)) {
                EdgeInteraction edge = bEdges[key];
                if (edge != null) {
                    float t = edge.GetEdge(solid);
                    return Vector3.Lerp(solid, open, t);
                }
            }
            float min = 0;
            float max = 1;
            float mid = 0;
            Vector3 result = Vector3.zero;
            while (max - min > 0.001f) {
                mid = (min + max) * 0.5f;
                result = Vector3.LerpUnclamped(solid, open, mid);
                if (owner.TestPoint(result.x, result.y, false)) {
                    min = mid;
                } else {
                    max = mid;
                }
            }
            bEdges.Add(new EdgeKey(solid, open), new EdgeInteraction(solid, open, mid));
            return result;
        }

        public void ApplyForeground(MeshFilter mf) {
            Mesh m = mf.sharedMesh;
            Color[] colors = new Color[vertex.Count];
            Vector3[] normals = new Vector3[vertex.Count];
            for (int i = 0; i < colors.Length; i++) {
                Vector3 pvec = vertex[i];
                float t = 1 - Mathf.Clamp01((owner.gradientSize - owner.GetPointDistance(pvec)) / owner.gradientSize);
                colors[i] = owner.gradient.Evaluate(t);
                pvec.z -= owner.gradientOffset.Evaluate(t) * owner.gradientDepth;
                vertex[i] = pvec;
                normals[i] = owner.GetPointNormal(pvec);
            }
            //mf.sharedMesh = m;
            m.Clear();
            m.SetVertices(vertex);
            m.SetUVs(0, uv);
            m.colors = colors;
            m.SetTriangles(triangles, 0);
            m.normals = normals;
            //m.RecalculateNormals();
            m.RecalculateBounds();
            //Debug.Log("created foreground mesh with " + m.vertexCount + " verts and " + m.GetIndexCount(0) + " indices");
        }

        public void ApplyBackground(MeshFilter mf) {
            Mesh m = mf.sharedMesh;
            Vector3[] normals = new Vector3[bVertex.Count];
            for (int i = 0; i < bVertex.Count; i++) {
                Vector3 pvec = bVertex[i];
                float id = owner.GetPointInnerDistance(pvec);
                float t = Mathf.Clamp01(1 - id);
                pvec.z = (owner.backgroundCurve.Evaluate(t) * owner.backgroundDepth) + owner.backgroundDistance;
                bVertex[i] = pvec;
                normals[i] = owner.GetPointInnerNormal(pvec);
            }
            //mf.sharedMesh = m;
            m.Clear();
            m.SetVertices(bVertex);
            m.SetUVs(0, bUv);
            m.SetTriangles(bTriangles, 0);
            m.normals = normals;
            //m.RecalculateNormals();
            m.RecalculateBounds();
            //Debug.Log("created background mesh with " + m.vertexCount + " verts and " + m.GetIndexCount(0) + " indices");
        }

        public void ApplyIsoSurf(IsoSurface isoSurf) {
            isoSurf.SetPoints(isoSurface.ToArray());
            retainedSurface = isoSurf;
        }

        public void ApplyCollision(Transform collider) {

            EdgeCollider2D[] edges = collider.GetComponents<EdgeCollider2D>();
            foreach (EdgeCollider2D edge in edges) {
                DestroyImmediate(edge);
            }
            Vector2[] isoPoints = new Vector2[2];
            int isoSegments = isoSurface.Count / 2;
            for (int i = 0; i < isoSegments; i++) {
                int ii = i * 2;
                isoPoints[0] = isoSurface[ii];
                isoPoints[1] = isoSurface[ii + 1];
                EdgeCollider2D col = collider.gameObject.AddComponent<EdgeCollider2D>();
                col.points = isoPoints;
            }

        }

        internal void ApplyShadow(MeshFilter shadow) {
            bool ownsSurf = false;
            if (retainedSurface == null) {
                retainedSurface = owner.gameObject.AddComponent<IsoSurface>();
                ApplyIsoSurf(retainedSurface);
                ownsSurf = true;
            }

            List<Vector3> sv = new List<Vector3>();

            List<int> si = new List<int>();
            Map<Vector3, int> sm = new Map<Vector3, int>();

            foreach (IsoIsland island in retainedSurface.islands) {
                foreach (SurfaceSegment seg in island.segments) {
                    Vector3 a = retainedSurface.points[seg.a];
                    Vector3 b = retainedSurface.points[seg.b];
                    Vector3 ab = a + Vector3.forward * (owner.backgroundDistance + 1);
                    Vector3 bb = b + Vector3.forward * (owner.backgroundDistance + 1);
                    int ai, bi, abi, bbi;

                    if (sm.ContainsKey(a)) {
                        ai = sm[a];
                    } else {
                        ai = sv.Count;
                        sm.Add(a, ai);
                        sv.Add(a);
                    }

                    if (sm.ContainsKey(b)) {
                        bi = sm[b];
                    } else {
                        bi = sv.Count;
                        sm.Add(b, bi);
                        sv.Add(b);
                    }

                    if (sm.ContainsKey(ab)) {
                        abi = sm[ab];
                    } else {
                        abi = sv.Count;
                        sm.Add(ab, abi);
                        sv.Add(ab);
                    }

                    if (sm.ContainsKey(bb)) {
                        bbi = sm[bb];
                    } else {
                        bbi = sv.Count;
                        sm.Add(bb, bbi);
                        sv.Add(bb);
                    }

                    si.Add(ai);
                    si.Add(abi);
                    si.Add(bi);

                    si.Add(bi);
                    si.Add(abi);
                    si.Add(bbi);
                }
            }

            shadow.sharedMesh = new Mesh();
            shadow.sharedMesh.Clear();
            shadow.sharedMesh.SetVertices(sv);
            shadow.sharedMesh.SetTriangles(si, 0);
            shadow.sharedMesh.SetUVs(0, sv);
            shadow.sharedMesh.RecalculateNormals();
            shadow.sharedMesh.RecalculateBounds();


            if (ownsSurf) {
                DestroyImmediate(retainedSurface);
                retainedSurface = null;
            }
        }

        internal void ApplyForeground(Mesh m) {
            Color[] colors = new Color[vertex.Count];
            Vector3[] normals = new Vector3[vertex.Count];
            if (owner.multiMaterial) {
                for (int i = 0; i < colors.Length; i++) {
                    Vector3 pvec = vertex[i];
                    Vector2 multiMat = owner.GetPointMultiMaterial(pvec);
                    float t = 1 - Mathf.Clamp01((owner.gradientSize - owner.GetPointDistance(pvec)) / owner.gradientSize);
                    colors[i] = owner.gradient.Evaluate(t);
                    colors[i].r = multiMat.x;
                    colors[i].b = multiMat.y;
                    pvec.z -= owner.gradientOffset.Evaluate(t) * owner.gradientDepth;
                    vertex[i] = pvec;
                    normals[i] = owner.GetPointNormal(pvec);
                }
            } else {
                for (int i = 0; i < colors.Length; i++) {
                    Vector3 pvec = vertex[i];
                    float t = 1 - Mathf.Clamp01((owner.gradientSize - owner.GetPointDistance(pvec)) / owner.gradientSize);
                    colors[i] = owner.gradient.Evaluate(t);
                    pvec.z -= owner.gradientOffset.Evaluate(t) * owner.gradientDepth;
                    vertex[i] = pvec;
                    normals[i] = owner.GetPointNormal(pvec);
                }
            }

            //mf.sharedMesh = m;

            m.SetVertices(vertex);
            m.SetUVs(0, uv);
            m.colors = colors;
            m.SetTriangles(triangles, 0);
            m.normals = normals;
            //m.RecalculateNormals();
            m.RecalculateBounds();
            //Debug.Log("created foreground mesh with " + m.vertexCount + " verts and " + m.GetIndexCount(0) + " indices");
        }

        internal void ApplyBackground(Mesh m) {
            Vector3[] normals = new Vector3[bVertex.Count];
            for (int i = 0; i < bVertex.Count; i++) {
                Vector3 pvec = bVertex[i];
                float id = owner.GetPointInnerDistance(pvec);
                float t = Mathf.Clamp01(1 - id);
                pvec.z = (owner.backgroundCurve.Evaluate(t) * owner.backgroundDepth) + owner.backgroundDistance;
                bVertex[i] = pvec;
                normals[i] = owner.GetPointInnerNormal(pvec);
            }
            //mf.sharedMesh = m;

            m.SetVertices(bVertex);
            m.SetUVs(0, bUv);
            m.SetTriangles(bTriangles, 0);
            m.normals = normals;
            //m.RecalculateNormals();
            m.RecalculateBounds();
            //Debug.Log("created background mesh with " + m.vertexCount + " verts and " + m.GetIndexCount(0) + " indices");
        }
    }

    public Vector2 GetPointMultiMaterial(Vector2 point) {
        List<LinkCollider> lcl = GetLocalLinkColliders(point);
        if (lcl == null || lcl.Count == 0) {
            return Vector2.zero;
        }

        point = SamplePerlin(point.x, point.y);

        float gcount = 0;
        Vector2 res = Vector2.zero;

        for (int i = 0; i < lcl.Count; i++) {
            LinkCollider l = lcl[i];
            float dist = l.PointDistance(point);
            if (dist < backgroundDistance) {
                float p = dist / backgroundDistance;
                gcount += p;
                Geom.Line line = new Geom.Line(l.A.position, l.B.position);
                res += Vector2.Lerp(l.A.multiMaterial, l.B.multiMaterial, line.GetT(point)) * p;
            }
        }

        if (gcount > 0) {
            return res / gcount;
        } else {
            return Vector2.zero;
        }
    }

    public float GetPointInnerDistance(Vector2 point) {
        Vector2 pPoint = SamplePerlin(point.x, point.y);
        float best = float.MaxValue;
        foreach (Zone z in zones) {
            float dist = z.InnerDistance(pPoint);
            best = Mathf.Min(best, dist);
        }

        foreach (LinkCollider l in linkColliders) {
            float dist = l.InnerDistance(pPoint);
            best = Mathf.Min(best, dist);
        }

        return best;
    }

    public float GetPointDistance(Vector2 point) {
        point = SamplePerlin(point.x, point.y);

        float best = float.MaxValue;

        foreach (Zone z in zones) {
            float dist = z.PointDistance(point);
            best = Mathf.Min(best, dist);
        }

        foreach (LinkCollider l in linkColliders) {
            float dist = l.PointDistance(point);
            if (dist < best) {
                best = dist;
            }
        }

        return best;
    }

    public Vector3 GetPointNormal(Vector2 point) {
        Vector3 res = Vector3.zero;
        const float samplemod = 0.5f;
        Vector2 pointtr = point + new Vector2(chunkSize, chunkSize) * samplemod;
        Vector2 pointtl = point + new Vector2(-chunkSize, chunkSize) * samplemod;
        Vector2 pointbl = point + new Vector2(-chunkSize, -chunkSize) * samplemod;
        Vector2 pointbr = point + new Vector2(chunkSize, -chunkSize) * samplemod;
        float sampletr = GetPointDistance(pointtr);
        float sampletl = GetPointDistance(pointtl);
        float samplebl = GetPointDistance(pointbl);
        float samplebr = GetPointDistance(pointbr);

        float hslope = ((sampletl - samplebr) + (samplebl - sampletr)) / 2;
        float vslope = ((samplebl - sampletr) + (samplebr - sampletl)) / 2;
        res = new Vector3(hslope, vslope, -Mathf.Abs(1 - Mathf.Sqrt((hslope * hslope) + (vslope * vslope))));
        return res.normalized;
    }

    public Vector3 GetPointInnerNormal(Vector2 point) {
        Vector3 res = Vector3.zero;
        const float samplemod = 0.5f;
        Vector2 pointtr = point + new Vector2(chunkSize, chunkSize) * samplemod;
        Vector2 pointtl = point + new Vector2(-chunkSize, chunkSize) * samplemod;
        Vector2 pointbl = point + new Vector2(-chunkSize, -chunkSize) * samplemod;
        Vector2 pointbr = point + new Vector2(chunkSize, -chunkSize) * samplemod;
        float sampletr = GetPointInnerDistance(pointtr);
        float sampletl = GetPointInnerDistance(pointtl);
        float samplebl = GetPointInnerDistance(pointbl);
        float samplebr = GetPointInnerDistance(pointbr);

        float hslope = ((sampletl - samplebr) + (samplebl - sampletr)) / 2 * samplemod;
        float vslope = ((samplebl - sampletr) + (samplebr - sampletl)) / 2 * samplemod;
        res = new Vector3(hslope, vslope, -Mathf.Abs(1 - Mathf.Sqrt((hslope * hslope) + (vslope * vslope))));
        return res.normalized;
    }


    public Vector2 GetPerlinSize() {
        return perlinSize;
    }

    public void SetPerlinSize(Vector2 size) {
        xdistort.SetSize(size.x, size.y);
        ydistort.SetSize(size.x, size.y);
        perlinSize = size;
    }

    public Vector2 SamplePerlin(float x, float y) {
        if (xdistort == null) {
            xdistort = Perlin.Create(perlinSize.x, perlinSize.y);
        }
        if (ydistort == null) {
            ydistort = Perlin.Create(perlinSize.x, perlinSize.y);
        }
        return new Vector2(
           x + (xdistort.Sample(x, y) * 2 - 1) * distort,
           y + (ydistort.Sample(x, y) * 2 - 1) * distort
        );
    }

    public Vector2 GetMin() {
        return min;
    }

    public Vector2 GetMax() {
        return max;
    }

    public void CalcBoundingBox() {
        min = new Vector2(float.MaxValue, float.MaxValue);
        max = new Vector2(float.MinValue, float.MinValue);
        bool isSurface = false;
        foreach (Zone zone in zones) {
            float zoneminx = zone.position.x - zone.radius;
            float zoneminy = zone.position.y - zone.radius;
            float zonemaxx = zone.position.x + zone.radius;
            float zonemaxy = zone.position.y + zone.radius;
            if (zone.aboveGround) {
                isSurface = true;
            }
            if (zoneminx < min.x) {
                min.x = zoneminx;
            }
            if (zoneminy < min.y) {
                min.y = zoneminy;
            }
            if (zonemaxx > max.x) {
                max.x = zonemaxx;
            }
            if (zonemaxy > max.y) {
                max.y = zonemaxy;
            }
        }
        min.x -= margin;
        min.y -= margin;
        max.x += margin;
        max.y += margin;
        if (isSurface) {
            max.y += ceiling;
        }
    }

    public void AddZone(Vector2 position, float radius) {
        Zone zone = ScriptableObject.CreateInstance<Zone>();
        zone.position = position;
        zone.radius = radius;
        zones.Add(zone);
        CalcBoundingBox();
    }

    public void UnLink(Zone z) {
        int ind = zones.IndexOf(z);
        for (int i = 0; i < links.Count;) {
            ZoneLink link = links[i];
            if (link.a == ind || link.b == ind) {
                links.RemoveAt(i);
                continue;
            }
            i++;
        }
    }

    public void RemoveZone(Zone z) {
        int ind = zones.IndexOf(z);
        for (int i = 0; i < links.Count;) {
            ZoneLink link = links[i];
            if (link.a == ind || link.b == ind) {
                links.RemoveAt(i);
                continue;
            }
            if (link.a > ind) {
                link.a--;
            }
            if (link.b > ind) {
                link.b--;
            }
            i++;
        }
        zones.RemoveAt(ind);
        DestroyImmediate(z);
        CalcBoundingBox();
    }

    public Zone GetClosestZone(Vector2 position) {
        if (zones.Count > 0) {
            Zone bestZone = zones[0];
            float bestDistance = (bestZone.position - position).sqrMagnitude;
            for (int i = 1; i < zones.Count; i++) {
                Zone curZone = zones[i];
                float curDistance = (curZone.position - position).sqrMagnitude;
                if (curDistance < bestDistance) {
                    bestZone = curZone;
                    bestDistance = curDistance;
                }
            }
            return bestZone;
        }
        return null;
    }

    public Zone GetClosestOverlappingZone(Vector2 position) {
        if (zones.Count > 0) {
            Zone bestZone = zones[0];
            float bestDistance = (bestZone.position - position).sqrMagnitude;
            for (int i = 1; i < zones.Count; i++) {
                Zone curZone = zones[i];
                float curDistance = (curZone.position - position).sqrMagnitude;
                if (curDistance < bestDistance) {
                    if ((curZone.position - position).magnitude < curZone.radius) {
                        bestZone = curZone;
                        bestDistance = curDistance;
                    }
                }
            }
            if ((bestZone.position - position).magnitude < bestZone.radius) {
                return bestZone;
            }
        }
        return null;
    }

    public bool IsPointClear(Vector2 position) {
        foreach (Zone zone in zones) {
            if ((position - zone.position).magnitude < zone.radius) {
                return false;
            }
        }
        return true;
    }

    public Zone GetZone(int ind) {
        return zones[ind];
    }

    public List<ZoneLink> GetLinks(Zone z, List<ZoneLink> ll) {
        List<ZoneLink> res = new List<ZoneLink>();
        int ind = zones.IndexOf(z);
        foreach (ZoneLink link in ll) {
            if (link.a == ind || link.b == ind) {
                res.Add(link);
            }
        }
        return res;
    }

    public List<ZoneLink> GetLinks(Zone z) {
        return GetLinks(z, links);
    }

    public ZoneLink GetLink(Zone a, Zone b) {
        foreach (ZoneLink link in links) {
            Zone A = GetZone(link.a);
            Zone B = GetZone(link.b);
            if ((A == a && B == b) ||
                  (A == b && B == a)) {
                return link;
            }
        }
        return null;
    }

    public bool LinkExists(Zone a, Zone b) {
        return GetLink(a, b) != null;
    }

    public void CreateLink(Zone a, Zone b) {
        if (!LinkExists(a, b)) {
            ZoneLink link = ScriptableObject.CreateInstance<ZoneLink>();
            link.a = zones.IndexOf(a);
            link.b = zones.IndexOf(b);
            links.Add(link);
        }
    }

    public void DestroyLink(Zone a, Zone b) {
        ZoneLink link = GetLink(a, b);
        if (link != null) {
            links.Remove(link);
            DestroyImmediate(link);
        }
    }


    public bool TestPoint(float x, float y, bool foreground = true) {
        Vector2 point = SamplePerlin(x, y);

        if (foreground == false && backgroundCutout != null) {
            for (int i = 0; i < backgroundColliders.Length; i++) {
                Collider2D coll = backgroundColliders[i];
                if (coll.OverlapPoint(point)) {
                    return true;
                }
            }
        }

        foreach (Zone z in zones) {
            if (foreground) {
                if (z.PointCollides(point)) {
                    return true;
                }
            } else {
                if (z.aboveGround && z.PointCollides(point, backgroundInset)) {
                    return true;
                }
            }
        }

        foreach (LinkCollider l in linkColliders) {
            if (foreground) {
                if (l.PointCollides(point)) {
                    return true;
                }
            } else {
                if (l.aboveGround && l.PointCollides(point, backgroundInset)) {
                    return true;
                }
            }
        }

        //if (x < min.x || x > max.x || y < min.y || y > max.y) {
        //   return true;
        //}

        return false;
    }

    public class SubmeshTask {
        TerrainMesh owner;
        Mesh foreground, background;
        Rect subrect;

        public SubmeshTask(TerrainMesh owner, Mesh foreground, Mesh background, Rect subrect) {
            this.owner = owner;
            this.foreground = foreground;
            this.background = background;
            this.subrect = subrect;
        }

        public void Run() {
            MeshHelper helper = new MeshHelper(owner, true);


            foreach (Triangle t in owner.triangles) {
                if (subrect.Contains(t.Centroid())) {
                    t.GenerateMesh(helper);
                }
            }

            helper.ApplyForeground(foreground);
            helper.ApplyBackground(background);
        }
    };

    public class SubmeshWorker {
        TerrainMesh owner;
        List<SubmeshTask> tasks;
        //Thread worker;
        public float percent;

        void Run() {
            for (int i = 0; i < tasks.Count; i++) {
                try {
                    tasks[i].Run();
                }
                catch (UnityException e) {
                    Cancel();
                    throw (e);
                }

                percent = ((float)i) / tasks.Count;
            }
            owner.smeshworker = null;
        }

        public SubmeshWorker(TerrainMesh owner) {

            this.owner = owner;
            this.tasks = new List<SubmeshTask>();
            //worker = null;
            percent = 0;
        }

        public void AddTask(SubmeshTask task) {
            tasks.Add(task);
        }

        public void Start() {
            Run();
            /*
            if (worker == null) {
                worker = new Thread(Run);
                worker.Start();
            }
            */
        }

        internal void Cancel() {
        }
    }

    SubmeshWorker smeshworker = null;

    public SubmeshWorker worker {
        get {
            return smeshworker;
        }
    }


    public void Apply() {
        if (meshContainer != null) {
            if (smeshworker == null) {
                smeshworker = new SubmeshWorker(this);
            } else {
                smeshworker.Cancel();
                smeshworker = new SubmeshWorker(this);
            }
            while (meshContainer.transform.childCount > 0) {
                DestroyImmediate(meshContainer.transform.GetChild(0).gameObject);
            }
            GameObject foregroundMeshes = new GameObject("ForegroundMeshes");
            GameObject backgroundMeshes = new GameObject("BackgroundMeshes");
            foregroundMeshes.transform.SetParent(meshContainer.transform);
            backgroundMeshes.transform.SetParent(meshContainer.transform);

            Rect subrect = new Rect(0, 0, meshSegmentSize, meshSegmentSize);
            MeshHelper globalHelper = new MeshHelper(this, false);
            int groundLayer = LayerMask.NameToLayer("Ground");
            for (subrect.y = min.y - meshSegmentSize * 0.5f; subrect.y < max.y; subrect.y += meshSegmentSize) {
                for (subrect.x = min.x - meshSegmentSize * 0.5f; subrect.x < max.x; subrect.x += meshSegmentSize) {
                    GameObject fg = new GameObject("mesh");
                    fg.layer = groundLayer;
                    fg.transform.SetParent(foregroundMeshes.transform);
                    MeshFilter fgf = fg.AddComponent<MeshFilter>();
                    fgf.sharedMesh = new Mesh();
                    MeshRenderer fgr = fg.AddComponent<MeshRenderer>();
                    fgr.sharedMaterial = foregroundMaterial;
                    fg.isStatic = true;

                    GameObject bg = new GameObject("mesh");
                    bg.layer = groundLayer;
                    bg.transform.SetParent(backgroundMeshes.transform);
                    MeshFilter bgf = bg.AddComponent<MeshFilter>();
                    bgf.sharedMesh = new Mesh();
                    MeshRenderer bgr = bg.AddComponent<MeshRenderer>();
                    bgr.sharedMaterial = backgroundMaterial;
                    bg.isStatic = true;

                    smeshworker.AddTask(new SubmeshTask(this, fgf.sharedMesh, bgf.sharedMesh, subrect));
                }
            }

            smeshworker.Start();

            foreach (Triangle t in triangles) {
                t.GenerateMesh(globalHelper);
            }

            if (isoSurface != null) {
                globalHelper.ApplyIsoSurf(isoSurface);
            }

            if (shadow != null) {
                globalHelper.ApplyShadow(shadow);
            }

            if (colliderContainer != null) {
                globalHelper.ApplyCollision(colliderContainer);
            }

        } else if (foreground != null) {
            MeshHelper helper = new MeshHelper(this, background != null);
            foreach (Triangle t in triangles) {
                t.GenerateMesh(helper);
            }

            if (isoSurface != null) {
                helper.ApplyIsoSurf(isoSurface);
            }

            helper.ApplyForeground(foreground);
            if (background != null) {
                helper.ApplyBackground(background);
            }
            if (shadow != null) {
                helper.ApplyShadow(shadow);
            }
            if (colliderContainer != null) {
                helper.ApplyCollision(colliderContainer);
            }
        }

        if (pathingMap) {
            pathingMap.CreateFromTerrain(this);
        }

        if (constraints != null) {
            constraints.Clear();
            constraints.min = min;
            constraints.max = max;

            foreach (ZoneLink link in links) {
                Zone a = GetZone(link.a);
                Zone b = GetZone(link.b);
                constraints.AddSegment(a.position, b.position, a.radius, b.radius);
            }
        }
    }

    public void Generate() {
        if (backgroundCutout == null) {
            backgroundColliders = null;
        } else {
            backgroundColliders = backgroundCutout.GetComponentsInChildren<Collider2D>();
            Debug.Log("colliders " + backgroundColliders.Length);
            if (backgroundColliders.Length == 0) {
                backgroundCutout = null;
            }
        }

        if (points == null) {
            points = new List<Point>();
        } else {
            points.Clear();
        }
        if (triangles == null) {
            triangles = new List<Triangle>();
        } else {
            triangles.Clear();
        }
        
        SetuplcHash();

        float hsep = chunkSize;
        float hofs = chunkSize / 2;
        float vsep = hofs * Mathf.Sqrt(3);
        //      float vdist = max.y - min.y;
        //      float hdist = max.x - min.x;
        float startx = min.x - hofs;
        float starty = min.y;
        bool odd = false;
        List<Point> lastLine = new List<Point>();
        List<Point> line = new List<Point>();
        List<Point> temp;
        int vline = 0;
        int llpos;
        for (float y = starty; y < max.y + vsep; y += vsep) {
            llpos = 0;
            for (float x = (odd) ? (startx) : (startx + hofs); x < max.x + hsep; x += hsep) {
                Vector3 position = new Vector3(x, y);
                Point point;
                point = Point.Create(position, TestPoint(x, y), TestPoint(x, y, false));


                points.Add(point);
                line.Add(point);
                if (vline > 0) {
                    Point llpoint = lastLine[llpos];
                    if (llpoint.Position().x < point.Position().x) {
                        Point llpoint2;
                        Point lcpoint;
                        //up triangles
                        if (line.Count > 1) {
                            lcpoint = line[line.Count - 2];
                            Triangle t = Triangle.Create(llpoint, lcpoint, point);
                            triangles.Add(t);
                        }

                        //down triangles
                        if (lastLine.Count > (llpos + 1)) {
                            llpoint2 = lastLine[llpos + 1];
                            Triangle t = Triangle.Create(llpoint2, llpoint, point);
                            triangles.Add(t);
                        }
                        llpos += 1;
                    }
                }
            }
            lastLine.Clear();
            temp = line;
            line = lastLine;
            lastLine = temp;
            odd = !odd;
            vline += 1;
        }
        Apply();
    }

    public void OnDrawGizmosSelected() {
        //if (points != null) {
        //	foreach (Point point in points) {
        //		point.DrawGizmo();
        //	}
        //}
        //if (triangles != null) {
        //   foreach (Triangle triangle in triangles) {
        //      triangle.DrawGizmo();
        //   }
        //}
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TerrainMesh))]
public class TerrainMeshEditor : Editor {
    enum EditState {
        None,
        Points,
        Connections,
        Positions,
        Radii,
        Surface,
        Texture
    };

    EditState mode;
    Zone connectZone = null;
    Zone textureZone = null;



    void OnSceneGUI() {
        TerrainMesh targ = target as TerrainMesh;

        Event e = Event.current;

        if (mode != EditState.Connections) {
            connectZone = null;
        }

        if (mode != EditState.Texture) {
            textureZone = null;
        }

        switch (e.type) {
            case EventType.MouseDown: {
                    if (mode == EditState.Points || mode == EditState.Connections || mode == EditState.Surface || mode == EditState.Texture) {
                        Plane xplane = new Plane(Vector3.forward, Vector3.zero);
                        Camera c = SceneView.lastActiveSceneView.camera;
                        Vector3 mp = new Vector3(e.mousePosition.x, c.pixelHeight - e.mousePosition.y);
                        Ray r = c.ScreenPointToRay(mp);
                        float enter = 0;

                        if (xplane.Raycast(r, out enter)) {
                            Vector3 editPoint = r.GetPoint(enter);
                            Vector2 ep2d = new Vector2(editPoint.x, editPoint.y);
                            if (mode == EditState.Points) {
                                if (e.button == 0) {
                                    if (targ.IsPointClear(ep2d)) {
                                        targ.AddZone(ep2d, 6);
                                    }
                                } else if (e.button == 1) {
                                    Zone zone = targ.GetClosestOverlappingZone(ep2d);
                                    if (zone != null) {
                                        targ.RemoveZone(zone);
                                    } else {
                                        mode = EditState.None;
                                        break;
                                    }
                                }
                            } else if (mode == EditState.Connections) {
                                if (e.button == 0) {
                                    if (connectZone == null) {
                                        connectZone = targ.GetClosestOverlappingZone(ep2d);
                                    } else {
                                        Zone z = targ.GetClosestOverlappingZone(ep2d);
                                        if (z != null) {
                                            if (targ.LinkExists(connectZone, z)) {
                                                targ.DestroyLink(connectZone, z);
                                            } else {
                                                targ.CreateLink(connectZone, z);
                                            }
                                        }
                                        connectZone = null;
                                    }
                                }
                            } else if (mode == EditState.Surface) {
                                if (e.button == 0) {
                                    Zone z = targ.GetClosestOverlappingZone(ep2d);
                                    if (z) {
                                        z.aboveGround = !z.aboveGround;
                                    }
                                }
                            } else if (mode == EditState.Texture) {
                                if (e.button == 0) {
                                    if (textureZone == null) {
                                        textureZone = targ.GetClosestOverlappingZone(ep2d);
                                    }
                                } else {
                                    textureZone = null;
                                }
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }

        
        foreach (Zone zone in targ.zones) {
            if (zone.aboveGround) {
                Handles.color = Color.green;
            } else {
                Handles.color = Color.yellow;
            }
            Vector3 position = new Vector3(zone.position.x, zone.position.y);
            if (mode == EditState.Radii) {
                zone.radius = Handles.RadiusHandle(Quaternion.identity, position, zone.radius);
            } else {
                Handles.DrawWireDisc(position, Vector3.forward, zone.radius);
            }
            if (mode == EditState.Positions) {
                position = Handles.FreeMoveHandle(position, Quaternion.identity, zone.radius * 0.8f, Vector3.zero, Handles.CircleHandleCap);
                zone.position = new Vector2(position.x, position.y);
            }
        }

        if (mode == EditState.Connections) {
            if (connectZone != null) {
                Vector3 position = new Vector3(connectZone.position.x, connectZone.position.y);
                Handles.color = Color.red;
                Handles.DrawWireDisc(position, Vector3.forward, connectZone.radius + 0.1f);
            }
        }

        if (mode == EditState.Texture) {
            if (textureZone != null) {
                const float handleSize = 3;
                Vector2 p = textureZone.position;
                Vector2 scaleOrigin = p - Vector2.one * (handleSize);
                Vector2 handlep = scaleOrigin + textureZone.multiMaterial * handleSize * 2;
                Handles.color = new Color(textureZone.multiMaterial.x, 0, textureZone.multiMaterial.y);
                Handles.DrawLine(p + Vector2.up * handleSize, p + Vector2.down * handleSize);
                Handles.DrawLine(p + Vector2.left * handleSize, p + Vector2.right * handleSize);
                Vector2 rp = Handles.FreeMoveHandle(handlep, Quaternion.identity, handleSize / 6, Vector3.zero, Handles.CircleHandleCap);
                textureZone.multiMaterial = (rp - scaleOrigin) / (handleSize * 2);
                textureZone.multiMaterial.x = Mathf.Clamp01(textureZone.multiMaterial.x);
                textureZone.multiMaterial.y = Mathf.Clamp01(textureZone.multiMaterial.y);
                //textureZone.multiMaterial.Normalize();
            }
        }

        foreach (ZoneLink link in targ.links) {
            Zone a = targ.GetZone(link.a);
            Zone b = targ.GetZone(link.b);
            if (a.aboveGround && b.aboveGround) {
                Handles.color = Color.green;
            } else {
                Handles.color = Color.cyan;
            }
            Vector2 paa;
            Vector2 pab;
            Vector2 pba;
            Vector2 pbb;
            if (Maths.FakePointTangentsOfTwoCircles(a.position, a.radius, b.position, b.radius,
                     out paa, out pab, out pba, out pbb)) {
                Vector3 vaa = Maths.tov3(paa);
                Vector3 vab = Maths.tov3(pab);
                Vector3 vba = Maths.tov3(pba);
                Vector3 vbb = Maths.tov3(pbb);
                Handles.DrawLine(vaa, vba);
                Handles.DrawLine(vab, vbb);
            }
        }

        Handles.color = Color.gray;
        {
            Vector2 min = targ.GetMin();
            Vector2 max = targ.GetMax();
            Vector3 topleft = new Vector3(min.x, max.y);
            Vector3 topright = new Vector3(max.x, max.y);
            Vector3 bottomleft = new Vector3(min.x, min.y);
            Vector3 bottomright = new Vector3(max.x, min.y);
            Handles.DrawLine(topleft, topright);
            Handles.DrawLine(topleft, bottomleft);
            Handles.DrawLine(bottomleft, bottomright);
            Handles.DrawLine(topright, bottomright);
        }
        targ.CalcBoundingBox();
    }

    override public void OnInspectorGUI() {
        serializedObject.Update();
        TerrainMesh targ = (TerrainMesh)target;
        DrawDefaultInspector();

        Vector2 newPerlin = EditorGUILayout.Vector2Field("Perlin Size", targ.GetPerlinSize());
        mode = (EditState)EditorGUILayout.EnumPopup("edit mode", mode);
        if (GUILayout.Button("generate mesh")) {
            ((TerrainMesh)target).Generate();
        }

        if (GUI.changed) {
            if (!newPerlin.Equals(targ.GetPerlinSize())) {
                targ.SetPerlinSize(newPerlin);
            }
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
