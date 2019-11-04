using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkCollider : ScriptableObject
{
    public Zone A;
    public Zone B;
   Vector2 pointa;
   float rada;
   Vector2 pointb;
   float radb;
   Vector2 n;
   float dist;
   public bool aboveGround;

    public Vector2 min {
        get {
            Vector2 res = new Vector2();
            res.x = Mathf.Min(A.position.x - A.radius, B.position.x - B.radius);
            res.y = Mathf.Min(A.position.y - A.radius, B.position.y - B.radius);
            return res;
        }
    }

    public Vector2 max {
        get {
            Vector2 res = new Vector2();
            res.x = Mathf.Max(A.position.x + A.radius, B.position.x + B.radius);
            res.y = Mathf.Max(A.position.y + A.radius, B.position.y + B.radius);
            return res;
        }
    }

   public static LinkCollider Create(Zone a, Zone b) {
      LinkCollider res = ScriptableObject.CreateInstance<LinkCollider>();
        res.A = a;
        res.B = b;
      res.pointa = a.position;
      res.pointb = b.position;
      res.rada = a.radius;
      res.radb = b.radius;
      res.n = (res.pointb - res.pointa).normalized;
      res.dist = (res.pointb - res.pointa).magnitude;
      res.aboveGround = a.aboveGround && b.aboveGround;
      return res;
   }

   public bool PointCollides(Vector2 point, float inset= 0) {
      if (aboveGround) {
         float hdist = Mathf.Abs(pointa.x - pointb.x);
         float hpos = (pointa.x + pointb.x)* 0.5f;
         if (Mathf.Abs(point.x - hpos) < hdist * 0.5) {
            float f = Mathf.Abs(point.x - pointa.x) / hdist;
            if (f > 0 && f < 1) {
               float midy = Mathf.Lerp(pointa.y, pointb.y, f);
               if (point.y > midy) {
                  return true;
               }
            }
         }
      }
      float t = Vector2.Dot(n, point - pointa);
      if (t >= 0 && t <= dist) {
         Vector2 mid = Vector2.Lerp(pointa, pointb, t / dist);
         float rad = Mathf.Lerp(rada, radb, t / dist);
         return (mid - point).magnitude < rad + inset;
      }
      return false;
   }

   public float PointDistance(Vector2 point) {
      float t = Mathf.Clamp01(Vector2.Dot(n, point - pointa)/dist);
      Vector2 mid = Vector2.Lerp(pointa, pointb, t);
      float rad = Mathf.Lerp(rada, radb, t);
      return Vector2.Distance(point, mid) - rad;
   }

    internal float InnerDistance(Vector2 point)
    {
        float t = Mathf.Clamp01(Vector2.Dot(n, point - pointa) / dist);
        Vector2 mid = Vector2.Lerp(pointa, pointb, t);
        float rad = Mathf.Lerp(rada, radb, t);
        return Vector2.Distance(point, mid) / rad;
    }
};
