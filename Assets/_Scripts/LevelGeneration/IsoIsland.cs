using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoIsland : ScriptableObject {
   [SerializeField]
   public SurfaceSegment[] segments;
   [SerializeField]
   List<Vector2> points;
   public bool closed = false;
   [SerializeField]
   bool draw = true;

   public float SegmentProgress(SurfaceSegment segment, Vector2 point) {
      return Maths.ProgressPointOnLine(point, points[segment.a], points[segment.b]);
   }

   public Vector2 SegmentPoint(SurfaceSegment segment, Vector2 point) {
      return Maths.ClosestPointOnLine(point, points[segment.a], points[segment.b]);
   }

   public float SegmentDistance(SurfaceSegment segment, Vector2 point) {
      return Maths.DistanceToLine(point, points[segment.a], points[segment.b]);
   }

   public Vector2 GetPoint(SurfaceSegment segment, float offset) {
      return Vector2.Lerp(points[segment.a], points[segment.b], offset);
   }

   public void SegmentToPoints(SurfaceSegment segment, out Vector2 a, out Vector2 b) {
      a = points[segment.a];
      b = points[segment.b];
   }

   public SurfaceSegment GetNext(SurfaceSegment segment) {
      foreach (SurfaceSegment seg in segments) {
         if (seg.a == segment.b) {
            return seg;
         }
      }
      return null;
   }

   public SurfaceSegment GetPrevious(SurfaceSegment segment) {
      foreach (SurfaceSegment seg in segments) {
         if (seg.b == segment.a) {
            return seg;
         }
      }
      return null;
   }

   public SurfaceSegment GetClosestSegment(Vector2 point) {
      float bestDist = float.MaxValue;
      SurfaceSegment bestSegment = null;
      for (int i = 0; i < segments.Length; i++) {
         float dist = SegmentDistance(segments[i], point);
         if (dist < bestDist) {
            bestDist = dist;
            bestSegment = segments[i];
         }
      }
      return bestSegment;
   }

   public static IsoIsland Create(SurfaceSegment[] segments, List<Vector2> points) {
      IsoIsland res = ScriptableObject.CreateInstance<IsoIsland>();
      res.segments = segments;
      res.points = points;
      res.closed = segments[0].a == segments[segments.Length - 1].b;
      return res;
   }

   public void Gizmo(Color gc) {
      if (draw) {
         Gizmos.color = gc;
         foreach (SurfaceSegment seg in segments) {
            Gizmos.DrawLine(points[seg.a], points[seg.b]);
         }
      }
   }
}
