using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maths {
   public const float root2 = 1.4142135623731f;
    public const float ir2conv = 1 / (root2);

    public static float Cross2D(Vector2 a, Vector2 b) {
        return a.x * b.y - a.y * b.x;
    }

    public static void ClosestPoint(Rigidbody2D body, Vector2 point, out float distance, out Vector2 outpoint) {
        Vector2 res = Vector2.zero;
        Collider2D[] bodyColliders = new Collider2D[body.attachedColliderCount];
        body.GetAttachedColliders(bodyColliders);
        float closestDistance = float.MaxValue;
        foreach (Collider2D col in bodyColliders) {
            Vector2 cclose = col.bounds.ClosestPoint(point);
            float cdist = (cclose - point).sqrMagnitude;
            if (cdist < closestDistance) {
                res = cclose;
                closestDistance = cdist;
            }
        }
        outpoint = res;
        distance = Mathf.Sqrt(closestDistance);
    }

    public static float CalculateSize(MonoBehaviour t) {
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);
        Vector3 position = t.transform.position;
        foreach (Renderer r in t.GetComponentsInChildren<Renderer>()) {
            float scale = Maths.Average(r.transform.lossyScale);
            Bounds rb = new Bounds((r.bounds.center - position) * scale, r.bounds.size * scale);
            b.Encapsulate(rb);
        }
        return (b.extents.x + b.extents.y) / (8 * Maths.root2);
    }

	public static float CubicHermite(float p1, float p2, float m1, float m2, float t) {
		float t2 = t * t;
		float t3 = t * t * t;
		return (2 * t3 - 3 * t2 + 1) * p1 + (t3 - 2 * t2 + t) * m1 + (-2 * t3 + 3 * t2) * p2 + (t3 + t2) * m2;
	}

	public static float DistortT(float t, float midpoint) {
		if (midpoint == 0) {
			return t;
		} else {
			return t + ParabolaRange (t) * midpoint;
		}
	}

	public static float ParabolaRange(float t) {
		float lt = (t - 0.5f) * 2;
		return 1 - (lt * lt);
	}

    internal static float Average(Vector3 scale) {
        return (scale.x + scale.y + scale.z) / 3;
    }

    public static Vector2 ThreePointLerp(Vector2 a, Vector2 b, Vector2 c, float t) {
		float maga = (a - b).magnitude;
		float magb = (b - c).magnitude;
		float fa = maga / (maga + magb);
		float fb = 1 - fa;
		if (t < fa) {
			return Vector2.Lerp (a, b, t / fa);
		} else {
			return Vector2.Lerp (b, c, (t - fa)/fb);
		}
	}

	public static Vector2 ThreePointSpline(Vector2 a, Vector2 b, Vector2 c, float t) {
		Vector2 offsetter = (b - a + b - c) * 0.5f;
		float maga = (a - b).magnitude;
		float magb = (b - c).magnitude;
		float fa = maga / (maga + magb);
		float fb = 1 - fa;
		return Vector2.Lerp (a, c, t) + (offsetter * ParabolaRange(t));
	}

	public static bool intersectionPointsOfCircles(Vector2 pa, float ra, Vector2 pb, float rb, out Vector2 pointa, out Vector2 pointb) {
		float rdist = (ra + rb);
		float rinnerdist = Mathf.Abs(ra - rb);
		float cdist = (pa - pb).magnitude;
		float idist = 1 / cdist;
		if (rdist > cdist && rinnerdist < cdist) {
			float a = (ra * ra - rb * rb + cdist) / (2 * cdist);
			float h = Mathf.Sqrt (ra * ra - a * a);
			Vector2 pi = pa + a * (pb - pa) * idist;
			pointa = new Vector2 (
				pi.x + h * (pb.y - pa.y) * idist,
				pi.y - h * (pb.x - pa.x) * idist
			);
			pointb = new Vector2 (
				pi.x - h * (pb.y - pa.y) * idist,
				pi.y + h * (pb.x - pa.x) * idist
			);
			return true;
		}
		pointa = Vector2.zero;
		pointb = Vector2.zero;
		return false;
	}

	public static bool pointTangentsOfCircle(Vector2 center, float radius, Vector2 target, out Vector2 pointa, out Vector2 pointb) {
		float dsquared = (center - target).sqrMagnitude;
		if (dsquared < radius * radius) {
			pointa = Vector2.zero;
			pointb = Vector2.zero;
			return false;
		}
		return intersectionPointsOfCircles (center, radius, target, Mathf.Sqrt (dsquared - (radius * radius)), out pointa, out pointb);
	}

	public static bool FakePointTangentsOfTwoCircles(Vector2 pa, float ra, Vector2 pb, float rb,
		out Vector2 apointa, out Vector2 apointb, out Vector2 bpointa, out Vector2 bpointb) {
		
			Vector2 norm = (pa - pb).normalized;
			Vector2 deltaa = new Vector2(-norm.y, norm.x);
			Vector2 deltab = new Vector2(norm.y, -norm.x);
			apointa = pa + deltaa * ra;
			apointb = pa + deltab * ra;
			bpointa = pb + deltaa * rb;
			bpointb = pb + deltab * rb;
			return true;
		
	}

   public static float ProgressPointOnLine(Vector2 point, Vector2 lineA, Vector2 lineB) {
      float linedist = Vector2.Distance(lineA, lineB);
      return Vector2.Dot((lineB - lineA).normalized, (point - lineA))/linedist;
   }

   public static Vector2 ClosestPointOnLine(Vector2 point, Vector2 lineA, Vector2 lineB) {
      float linedist = Vector2.Distance(lineA, lineB);
      return Vector2.Lerp(lineA, lineB, Vector2.Dot((lineB - lineA).normalized, (point - lineA))/linedist);
   }

   public static float DistanceToLine(Vector2 point, Vector2 lineA, Vector2 lineB) {
      return Vector2.Distance(point, ClosestPointOnLine(point, lineA, lineB));
   }

	public static Vector3 tov3(Vector2 vec) {
		return new Vector3 (vec.x, vec.y);
	}

	public static Vector2 tov2(Vector3 vec) {
		return new Vector2 (vec.x, vec.y);
	}

    public static Vector2 rotatev2(Vector2 vec, float degrees) {
        float c = Mathf.Cos(degrees * Mathf.Deg2Rad);
        float s = Mathf.Sin(degrees * Mathf.Deg2Rad);

        return new Vector2(c * vec.x - s * vec.y, c * vec.y + s * vec.x);
    }
}
