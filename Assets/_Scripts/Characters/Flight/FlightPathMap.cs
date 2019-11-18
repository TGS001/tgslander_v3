using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightPathMap : MonoBehaviour {
    [System.Serializable]
    public class Region {
        public Geom.Capsule area;
        public List<int> portals;
        public int id;
        public bool visited;
        public bool open;
        public float distance;

        public Region(Geom.Capsule area, int id) {
            this.area = area;
            portals = new List<int>();
            this.id = id;
            visited = false;
            open = false;
            distance = 0;
        }

        internal float Distance(Region a) {
            return Vector2.Distance((area.spine.a + area.spine.b) * 0.5f, (a.area.spine.a + a.area.spine.b) * 0.5f);
        }

        internal float Distance(Vector2 start) {
            throw new NotImplementedException();
        }

        internal Vector2 PointInside(Vector2 start, float radius) {
            return area.PointInside(start, radius);
        }
    }

    [System.Serializable]
    public class Portal {
        public Geom.Capsule area;
        public int zonea;
        public int zoneb;
        public int id;

        public Portal(Geom.Capsule area, int zonea, int zoneb, int id) {
            this.area = area;
            this.zonea = zonea;
            this.zoneb = zoneb;
            this.id = id;
        }

        internal float Distance(Portal p) {
            return Vector2.Distance((area.spine.a + area.spine.b) * 0.5f, (p.area.spine.a + p.area.spine.b) * 0.5f);
        }

        internal bool Fits(float radius) {
            return radius < area.ra && radius < area.rb;
        }
    }

    public List<Region> regions;
    public List<Portal> portals;
    //public Vector2 debugStart;
    //public Vector2 debugEnd;
    //public float debugRadius;
    //public FlightPath dpath;


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        foreach (Region cap in regions) {
            cap.area.Gizmo();
        }

        Gizmos.color = Color.yellow;
        foreach (Portal cap in portals) {
            cap.area.Gizmo();
        }
    }

    private void Clear() {
        if (regions == null) {
            regions = new List<Region>();
        }
        regions.Clear();

        if (portals == null) {
            portals = new List<Portal>();
        }
        portals.Clear();
    }

    private void CreateLinks() {
        for (int i = 0; i < regions.Count; i++) {
            Region reg = regions[i];
            Geom.Capsule capa = reg.area;
            for (int j = i + 1; j < regions.Count; j++) {
                Region other = regions[j];
                Geom.Capsule capb = other.area;
                Geom.Capsule bridge = Geom.Capsule.Bridge(capa, capb);
                if ((bridge.ra + bridge.rb) >= bridge.spine.length) {
                    reg.portals.Add(portals.Count);
                    other.portals.Add(portals.Count);
                    portals.Add(new Portal(bridge, i, j, portals.Count));
                }
            }
        }
    }

    public void CreateFromTerrain(TerrainMesh terrain) {
        Clear();
        bool[] zonesUsed = new bool[terrain.zones.Count];
        foreach (ZoneLink link in terrain.links) {
            zonesUsed[link.a] = true;
            zonesUsed[link.b] = true;
            Zone za = terrain.zones[link.a];
            Zone zb = terrain.zones[link.b];
            Geom.Capsule cap = new Geom.Capsule(za.position, za.radius, zb.position, zb.radius);
            regions.Add(new Region(cap, regions.Count));
        }

        for (int i = 0; i < zonesUsed.Length; i++) {
            if (!zonesUsed[i]) {
                Zone z = terrain.zones[i];
                Geom.Capsule cap = new Geom.Capsule(z.position, z.radius, z.position, z.radius);
                regions.Add(new Region(cap, regions.Count));
            }
        }

        CreateLinks();
    }

    public Region ClosestRegion(Vector2 point) {
        if (regions.Count == 0) {
            return null;
        }
        Region bestRegion = regions[0];
        float bestDistance = bestRegion.area.Distance(point);
        for (int i = 1; i < regions.Count; i++) {
            float distance = regions[i].area.Distance(point);
            if (distance < bestDistance) {
                bestDistance = distance;
                bestRegion = regions[i];
            }
        }
        return bestRegion;
    }

    public void OpenNeighbors(Region r, ref LinkedList<Region> list) {
        if (r.visited) {
            return;
        }
        for (int i = 0; i < r.portals.Count; i++) {
            Portal p = portals[r.portals[i]];
            Region a = regions[p.zonea];
            Region b = regions[p.zoneb];
            if (!(a.id == r.id) && a.open == false && a.visited == false) {
                a.open = true;

                list.AddLast(a);
                //Debug.DrawLine(a.area.spine.a, a.area.spine.b, Color.magenta);
            }

            if (!(b.id == r.id) && b.open == false && b.visited == false) {
                b.open = true;

                list.AddLast(b);
                //Debug.DrawLine(b.area.spine.a, b.area.spine.b, Color.magenta);
            }

            if (a.id != r.id)
                a.distance = Mathf.Min(a.distance, r.distance + 1);

            if (b.id != r.id)
                b.distance = Mathf.Min(b.distance, r.distance + 1);
        }
        r.visited = true;
        list.Remove(r);
    }

    private Portal GetForwardPortal(Region r) {
        Portal bestPortal = null;
        float bestDistance = r.distance;
        foreach (int p in r.portals) {
            Portal cp = portals[p];
            Region or;
            if (cp.zonea == r.id) {
                or = regions[cp.zoneb];
            } else {
                or = regions[cp.zonea];
            }
            if (or.distance < bestDistance) {
                bestDistance = or.distance;
                bestPortal = cp;
            }
        }
        return bestPortal;
    }

    public FlightPath CreatePath(Vector2 start, Vector2 end, float radius, float cruiseSpeed, bool includeTargetPosition = true) {
        Region sr = ClosestRegion(start);
        start = sr.PointInside(start, radius);
        Region er = ClosestRegion(end);
        if (sr.id == er.id) {
            FlightPath path = new FlightPath();
            path.nodes = new List<FlightPathNode>();
            path.nodes.Add(new FlightPathNode(FlightAI.FlightMode.followPath, end, cruiseSpeed, 0));
            return path;
        }
        if (sr != null && er != null) {
            //Debug.DrawLine(sr.area.spine.a, sr.area.spine.b, Color.green);
            //Debug.DrawLine(er.area.spine.a, er.area.spine.b, Color.cyan);
            foreach (Region r in regions) {
                r.visited = false;
                r.open = false;
                r.distance = float.MaxValue;
            }
            er.open = true;
            er.distance = 0;

            LinkedList<Region> openList = new LinkedList<Region>();
            OpenNeighbors(er, ref openList);
            while (sr.visited == false && openList.Count > 0) {
                Region bestRegion = null;
                float bestDistance = float.MaxValue;
                foreach (Region r in openList) {
                    if (r.distance < bestDistance) {
                        bestRegion = r;
                        bestDistance = r.distance;
                    }
                }
                OpenNeighbors(bestRegion, ref openList);
            }

            Region cur = sr;
            FlightPath path = new FlightPath();
            path.nodes = new List<FlightPathNode>();
            while (cur != er) {
                Portal bestPortal = null;
                Region bestRegion = null;
                float bestDistance = cur.distance;
                foreach (int portal in cur.portals) {
                    Portal cp = portals[portal];
                    if (cp.zonea != cur.id) {
                        Region z = regions[cp.zonea];
                        if (z.distance < bestDistance) {
                            bestDistance = z.distance;
                            bestPortal = cp;
                            bestRegion = z;
                        }
                    } else if (cp.zoneb != cur.id) {
                        Region z = regions[cp.zoneb];
                        if (z.distance < bestDistance) {
                            bestDistance = z.distance;
                            bestPortal = cp;
                            bestRegion = z;
                        }
                    }
                }
                if (bestPortal == null) {
                    break;
                }
                Vector2 pointa;
                Vector2 pointb;
                float rada;
                float radb;
                if (bestPortal.zonea == cur.id) {
                    pointa = bestPortal.area.spine.a;
                    pointb = bestPortal.area.spine.b;
                    rada = bestPortal.area.ra;
                    radb = bestPortal.area.rb;
                } else {
                    pointa = bestPortal.area.spine.b;
                    pointb = bestPortal.area.spine.a;
                    rada = bestPortal.area.rb;
                    radb = bestPortal.area.ra;
                }

                if (pointa == pointb) {
                    path.nodes.Add(new FlightPathNode(FlightAI.FlightMode.followPath, pointa, cruiseSpeed, rada));
                } else {
                    path.nodes.Add(new FlightPathNode(FlightAI.FlightMode.followPath, pointa, cruiseSpeed, rada));
                    path.nodes.Add(new FlightPathNode(FlightAI.FlightMode.followPath, pointb, cruiseSpeed, radb));
                }

                cur = bestRegion;
            }
            if (includeTargetPosition)
                path.nodes.Add(new FlightPathNode(FlightAI.FlightMode.followPath, end, cruiseSpeed, 0));
            return path;
        }
        return new FlightPath();
    }

    // Update is called once per frame
    void Update() {

    }
}
