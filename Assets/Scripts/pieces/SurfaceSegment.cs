using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSegment : ScriptableObject {
   public int a;
   public int b;
   public static SurfaceSegment Create(IsoSurface.Segment s) {
      SurfaceSegment seg = ScriptableObject.CreateInstance<SurfaceSegment>();
      seg.a = s.a;
      seg.b = s.b;
      return seg;
   }
}
