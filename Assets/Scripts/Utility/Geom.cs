using System;
using UnityEngine;

public class Geom {
    private Geom() { }

    public interface Shape {
        float Distance(Vector2 point);
        bool Contains(Vector2 point);
        Vector2 Closest(Vector2 point);
        void Gizmo();
    }

    [System.Serializable]
    public struct Line : Shape {
        public Vector2 a;
        public Vector2 b;

        public float length {
            get {
                return Vector2.Distance(a, b);
            }
        }

        public Line(Vector2 a, Vector2 b) {
            this.a = a;
            this.b = b;
        }

        public static Line Bridge(Line la, Line lb) {
            Vector2 da = la.b - la.a;
            Vector2 db = lb.b - lb.a;
            Vector2 r = la.a - lb.a;
            float af = da.sqrMagnitude;
            float ef = db.sqrMagnitude;
            float ff = Vector2.Dot(db, r);

            if (af <= float.Epsilon && ef <= float.Epsilon) {
                return new Line(la.a, lb.a);
            }

            if (af <= float.Epsilon) {
                return new Line(la.a, lb.Closest(la.a));
            }

            if (ef <= float.Epsilon) {
                return new Line(la.Closest(lb.a), lb.a);
            }
            float cf = Vector2.Dot(da, r);
            float bf = Vector2.Dot(da, db);
            float denom = af * ef - bf * bf;
            Vector2 pa;
            Vector2 pb;

            if (denom != 0) {
                float t = (bf * ff - cf * ef);
                pa = Vector2.Lerp(la.a, la.b, t);
            } else {
                pa = (la.a + la.b) * 0.5f;
            }

            pb = lb.Closest(pa);

            return new Line(pa, pb);
        }

        public float GetT(Vector2 point) {
            Vector2 pd = point - a;
            Vector2 ln = (b - a).normalized;
            return Vector2.Dot(ln, pd)/Vector2.Distance(a, b);
        }

        public Vector2 Closest(Vector2 point) {
            float t = GetT(point);
            return Vector2.Lerp(a, b, t);
        }

        public float Closest(Line other) {
            Vector2 c1, c2;
            float s, t;
            return Closest(other, out c1, out c2, out s, out t);
        }

        public float Closest(Line other, out Vector2 c1, out Vector2 c2, out float s, out float t) {
            //TODO: textbook code. adapt this better.
            Vector2 q1 = this.b;
            Vector2 p1 = this.a;
            Vector2 q2 = other.b;
            Vector2 p2 = other.a;

            Vector2 d1 = q1 - p1;
            Vector2 d2 = q2 - p2;
            Vector2 r = p1 - p2;

            float a = Vector2.Dot(d1, d1);
            float e = Vector2.Dot(d2, d2);
            float f = Vector2.Dot(d2, r);

            if (a <= float.Epsilon && e <= float.Epsilon) {
                s = 0;
                t = 0;
                c1 = p1;
                c2 = p2;
                return Vector2.Distance(c1, c2);
            }

            if (a <= float.Epsilon) {
                s = 0;
                t = Mathf.Clamp01(f / e);
            } else {
                float c = Vector2.Dot(d1, r);
                if (e <= float.Epsilon) {
                    t = 0;
                    s = Mathf.Clamp01(-c / a);
                } else {
                    float b = Vector2.Dot(d1, d2);
                    float denom = a * e - b * b;
                    if (denom != 0) {
                        s = Mathf.Clamp01((b * f - c * e) / denom);
                    } else {
                        s = 0;
                    }

                    t = (b * s + f) / e;
                    if (t < 0) {
                        t = 0;
                        s = Mathf.Clamp01(-c / a);
                    } else if (t > 1) {
                        t = 1;
                        s = Mathf.Clamp01((b - c) / a);
                    }
                }
            }
            c1 = p1 + d1 * s;
            c2 = p2 + d2 * t;
            return Vector2.Distance(c1, c2);
        }

        public bool Contains(Vector2 point) {
            return false;
        }

        public float Distance(Vector2 point) {
            return (Vector2.Distance(Closest(point), point));
        }

        public void Gizmo() {
            Gizmos.DrawLine(a, b);
        }
    }

    [System.Serializable]
    public struct Capsule : Shape {
        public Line spine;
        public float ra;
        public float rb;

        public Capsule(Vector2 a, float ra, Vector2 b, float rb) {
            spine = new Line(a, b);
            this.ra = ra;
            this.rb = rb;
        }

        public static Capsule Bridge(Capsule ca, Capsule cb) {
            Capsule res = new Capsule();
            Vector2 cap;
            Vector2 cbp;
            float ta;
            float tb;
            float distance = ca.spine.Closest(cb.spine, out cap, out cbp, out ta, out tb);
            res.spine = new Line(cap, cbp);
            res.ra = Mathf.Lerp(ca.ra, ca.rb, ta);
            res.rb = Mathf.Lerp(cb.ra, cb.rb, tb);
            return res;
        }

        public Vector2 Closest(Vector2 point) {
            float t = Mathf.Clamp01(spine.GetT(point));
            Vector2 lc = Vector2.Lerp(spine.a, spine.b, t);
            float rad = Mathf.Lerp(ra, rb, t);
            return (Vector2.MoveTowards(lc, point, rad));
        }

        public bool Contains(Vector2 point) {
            float t = spine.GetT(point);
            Vector2 lc = Vector2.Lerp(spine.a, spine.b, t);
            float rad = Mathf.Lerp(ra, rb, t);
            return (Vector2.Distance(lc, point) < rad);
        }

        public float Distance(Vector2 point) {
            float t = spine.GetT(point);
            Vector2 lc = Vector2.Lerp(spine.a, spine.b, t);
            float rad = Mathf.Lerp(ra, rb, t);
            //Debug.DrawLine(point, lc, Color.blue);
            return (Vector2.Distance(lc, point) - rad);
        }

        public void Gizmo() {
            Gizmos.DrawWireSphere(spine.a, ra);
            Gizmos.DrawWireSphere(spine.b, rb);
            Vector2 ll = (spine.a - spine.b).normalized;
            ll.Set(-ll.y, ll.x);
            Gizmos.DrawLine(spine.a + ll * ra, spine.b + ll * rb);
            Gizmos.DrawLine(spine.a - ll * ra, spine.b - ll * rb);
        }

        internal Vector2 PointInside(Vector2 start, float radius) {
            float t = spine.GetT(start);
            Vector2 sp = Vector2.Lerp(spine.a, spine.b, t);
            float sr = Mathf.Lerp(ra, rb, t);
            if (radius > sr) {
                return sp;
            }
            return Vector2.MoveTowards(sp, start, sr - radius);
        }
    }
}