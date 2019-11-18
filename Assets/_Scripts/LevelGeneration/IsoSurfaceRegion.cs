using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IsoSurfaceRegion : MonoBehaviour, ISerializationCallbackReceiver {
   public Color color = Color.cyan;
   public IsoIsland island;
   public SurfaceSegment startSegment;
   public SurfaceSegment endSegment;
   public float startOffset;
   public float endOffset;

   LinkedList<SurfaceSegment> segments;
   [SerializeField]
   SurfaceSegment[] _segments;

   [SerializeField]
   [HideInInspector]
   bool dirty = true;
    [SerializeField]
    [HideInInspector]
   float _length;
   [SerializeField]
   float[] segLengths;
   [SerializeField]
   float[] rawLengths;

   public void SetChanged() {
      if (startSegment.Equals(endSegment)) {
         startOffset = Mathf.Clamp01(startOffset);
         endOffset = Mathf.Clamp(endOffset, startOffset, 1);
      }
      dirty = true;
      BroadcastMessage("OnRegionChanged", SendMessageOptions.DontRequireReceiver);
   }

   public class Stepper
   {
      IsoSurfaceRegion region;
      LinkedListNode<SurfaceSegment> node;
      int index;
      int indexMax;
      float segmentProgress;
      float totalDistance;
      bool _isInRegion;

      Vector2 _position;
      Vector2 _normal;

      public Vector2 position {
         get {
            return _position;
         }
      }

      public Vector2 normal {
         get {
            return _normal;
         }
      }

      public float distance {
         set {
            Step(value - totalDistance);
         }
         get {
            return totalDistance;
         }
      }

      public bool isInRegion {
         get {
            return _isInRegion;
         }
      }

      void Initialize(IsoSurfaceRegion region) {
         this.region = region;
         node = region.segments.First;
         index = 0;
         indexMax = region.segments.Count - 1;
         segmentProgress = region.startOffset;
         totalDistance = 0;
         _isInRegion = true;
      }

      public Stepper(IsoSurfaceRegion region) {
         Initialize(region);
         Step(0);
      }

      public Stepper(Stepper other, float offset) {
         Initialize(other.region);
         Step(other.distance + offset);
      }

      public void Step(float distance) {
         float length = region.length;
         if (Mathf.Abs(distance) > 0) {
            
            float segd = region.rawLengths[index];
            float compd = segd * segmentProgress;
            float newd = compd + distance;
            while ((newd < 0) || (newd > segd)) {
               if (newd < 0) {
                  if (index > 0) {
                     index--;
                     node = node.Previous;
                     segd = region.rawLengths[index];
                     newd += segd;
                  } else {
                     break;
                  }
               } else if (newd > segd) {
                  if (index < indexMax) {
                     newd -= segd;
                     index++;
                     node = node.Next;
                     segd = region.rawLengths[index];
                  } else {
                     break;
                  }
               }
            }

            segmentProgress = newd / segd;

            totalDistance += distance;
            //Debug.Log("total: " + totalDistance + " offset: " + segmentProgress + " segDistance: " + newd + " segLength: " + segd);
         }



         Vector2 a, b;
         region.island.SegmentToPoints(node.Value, out a, out b);
         _position = Vector2.Lerp(a, b, segmentProgress);
         _normal = (b - a).normalized;
         _normal.Set(-_normal.y, _normal.x);
         _isInRegion = (totalDistance >= 0) && (totalDistance <= length);
      }
   }

   public float length {
      get {
         if (dirty) {
            dirty = false;
            _length = 0;
            segLengths = new float[segments.Count];
            rawLengths = new float[segments.Count];
            int index = 0;
            foreach (SurfaceSegment seg in segments) {
               Vector2 a, b;
               island.SegmentToPoints(seg, out a, out b);
               segLengths[index] = Vector2.Distance(a, b);
               rawLengths[index] = segLengths[index];
               index++;
            }
            if (segLengths.Length == 1) {
               segLengths[0] *= endOffset - startOffset;
               _length = segLengths[0];
            } else {
               segLengths[0] *= 1 - startOffset;
               segLengths[segLengths.Length - 1] *= endOffset;
               foreach (float l in segLengths) {
                  _length += l;
               }
            }
         }
         return _length;
      }
   }

   public Vector2 startPoint {
      set {
         float p = island.SegmentProgress(startSegment, value);
         if (p > 1) {
            if (segments.Count > 1) {
               segments.RemoveFirst();
               startSegment = segments.First.Value;
            }
            startOffset = island.SegmentProgress(startSegment, value);
         } else if (p < 0) {
            SurfaceSegment prev = island.GetPrevious(startSegment);
            segments.AddFirst(prev);
            startSegment = prev;
            startOffset = island.SegmentProgress(startSegment, value);
         } else {
            startOffset = p;
         }

         if (startSegment.Equals(endSegment)) {
            if (startOffset > endOffset) {
               startOffset = endOffset;
            }
         }
         startOffset = Mathf.Clamp01(startOffset);
         SetChanged();
      }
      get {
         return island.GetPoint(startSegment, startOffset);
      }
   }

   public Vector2 endPoint {
      set {
         float p = island.SegmentProgress(endSegment, value);
         if (p < 0) {
            if (segments.Count > 1) {
               segments.RemoveLast();
               endSegment = segments.Last.Value;
            }
            endOffset = island.SegmentProgress(endSegment, value);
         } else if (p > 1) {
            SurfaceSegment next = island.GetNext(endSegment);
            segments.AddLast(next);
            endSegment = next;
            endOffset = island.SegmentProgress(endSegment, value);
         } else {
            endOffset = p;
         }
         if (startSegment.Equals(endSegment)) {
            if (startOffset > endOffset) {
               endOffset = startOffset;
            }
         }
         endOffset = Mathf.Clamp01(endOffset);
         SetChanged();
      }
      get {
         return island.GetPoint(endSegment, endOffset);
      }
   }

   public void OnBeforeSerialize() {
      _segments = new SurfaceSegment[segments.Count];
      segments.CopyTo(_segments, 0);
   }

   public void OnAfterDeserialize() {
      segments = new LinkedList<SurfaceSegment>();
      foreach (SurfaceSegment seg in _segments) {
         segments.AddLast(seg);
      }
      _segments = null;
   }

   public void Flip() {
      SurfaceSegment ts = startSegment;
      startSegment = endSegment;
      endSegment = ts;
      float to = startOffset;
      startOffset = endOffset;
      endOffset = to;
   }

   public void FixRegion() {
      while (true) {
         if (segments == null) {
            segments = new LinkedList<SurfaceSegment>();
         } else {
            segments.Clear();
         }

         segments.AddLast(startSegment);
         SurfaceSegment curSegment = segments.Last.Value;
         bool flip = false;
         if (startSegment.Equals(endSegment)) {
            if (endOffset < startOffset) {
               flip = true;
            }
         } else {
            while (true) {
               curSegment = island.GetNext(curSegment);
               if (curSegment == null) {
                  flip = true;
                  break;
               } else if (curSegment.Equals(endSegment)) {
                  segments.AddLast(curSegment);
                  break;
               } else {
                  segments.AddLast(curSegment);
               }
            }
         }

         if (flip) {
            Flip();
         } else {
            break;
         }
      }
      SetChanged();
   }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public void OnDrawGizmosSelected() {
      Gizmos.color = color;
      if (startSegment != endSegment) {
         foreach (SurfaceSegment seg in segments) {
            Vector2 a;
            Vector2 b;
            island.SegmentToPoints(seg, out a, out b);
            if (seg == startSegment) {
               a = Vector2.Lerp(a, b, startOffset);
            } else if (seg == endSegment) {
               b = Vector2.Lerp(a, b, endOffset);
            }
            Gizmos.DrawLine(a, b);
         }
      } else {
         Vector2 a = island.GetPoint(startSegment, startOffset);
         Vector2 b = island.GetPoint(startSegment, endOffset);
         Gizmos.DrawLine(a, b);
      }
   }
}
   
#if UNITY_EDITOR
[CustomEditor(typeof(IsoSurfaceRegion))]
public class IsoSurfaceRegionEditor : Editor
{
   public void OnSceneGUI() {
      IsoSurfaceRegion region = (IsoSurfaceRegion)target;
      Handles.color = Color.green;
      Vector2 start = region.startPoint;
      Vector2 end = region.endPoint;

        Vector2 nstart = Handles.FreeMoveHandle(start, Quaternion.identity, 0.5f, Vector3.zero, Handles.CircleHandleCap);
      Vector2 nend = Handles.FreeMoveHandle(end, Quaternion.identity, 0.5f, Vector3.zero, Handles.CircleHandleCap);

      bool update = false;

      if (!start.Equals(nstart)) {
         region.startPoint = nstart;
         update = true;
      }
      if (!end.Equals(nend)) {
         region.endPoint = nend;
         update = true;
      }
      if (update) {
         EditorUtility.SetDirty(target);
      }
   }

   override public void OnInspectorGUI() {
      IsoSurfaceRegion region = (IsoSurfaceRegion)target;
      region.color = EditorGUILayout.ColorField(region.color);
      if (region.island.closed) {
         if (GUILayout.Button("Flip")) {
            region.Flip();
            region.FixRegion();
            EditorUtility.SetDirty(region);
         }
      }
   }
}
#endif