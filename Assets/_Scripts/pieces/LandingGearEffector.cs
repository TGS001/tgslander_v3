using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LandingGearEffector : MonoBehaviour {
   public delegate void TouchdownDelegate(bool touch, GameObject surface);
   
   [HideInInspector]
   public Vector3 origin = Vector3.zero;
   [HideInInspector]
   public Vector3 end = -Vector3.up;
   [HideInInspector]
   public float radius = 0.01f;
   public float springConstant = 1;
   public float springDamping = 1;
   public float friction = 1;
   public Transform display;
   public TouchdownDelegate listener {
      set {
         _listener = value;
      }
   }
   private TouchdownDelegate _listener;

   //public Vector3 displayFix = Vector3.one;

   private Rigidbody2D body;

   public float castScale {
      get {
         if (transform.hasChanged || _castScale == 0) {
            Vector3 globalScale = transform.lossyScale;
            _castScale = Mathf.Max(globalScale.x, globalScale.y, globalScale.z);
         }
         return _castScale;
      }
   }
   private float _castScale;

   private Vector3 displayEffector;
   private Vector3 displayLocalP;
   private float fraction;
   private float castRadius;
   private float strutLength;
   private float lastFraction;

   [SerializeField]
   [HideInInspector]
   private bool _landed = false;

   public bool landed {
      get {
         return _landed;
      }
   }

	// Use this for initialization
	void Start () {
      if (display != null) {
         displayLocalP = display.localPosition;
         displayEffector = origin - end;
         //displayEffector = display.InverseTransformPoint(transform.TransformPoint(origin - end));
         //displayEffector.x *= displayFix.x;
         //displayEffector.y *= displayFix.y;
         //displayEffector.z *= displayFix.z;
      }
      strutLength = (transform.TransformPoint(origin) - transform.TransformPoint(end)).magnitude;
      body = GetComponentInParent<Rigidbody2D>();
	}

   void FixedUpdate () {
      if (body != null && display != null) {
         if (body.IsAwake()) {
            Vector2 castOrigin = Maths.tov2(transform.TransformPoint(origin));
            Vector2 castEnd = Maths.tov2(transform.TransformPoint(end));

            Vector2 castDirection = (castEnd - castOrigin);
            float castMagnitude = castDirection.magnitude;
            castRadius = radius * castScale;


            RaycastHit2D ray = Physics2D.CircleCast(castOrigin, castRadius, castDirection, castMagnitude,
                               Physics2D.GetLayerCollisionMask(14));

            //Debug.DrawRay(castOrigin, castDirection, Color.cyan);

            if (ray.collider != null) {
               fraction = ray.fraction;
               if (body != null) {
                  if (!_landed) {
                     _landed = true;
                     if (_listener != null) {
                        _listener(true, ray.collider.gameObject);
                     }
                  }
               
                  Vector2 otherRvel = Vector2.zero;
                  if (ray.rigidbody != null) {
                     otherRvel = ray.rigidbody.GetPointVelocity(ray.point);
                  }
                  Vector2 rvel = body.GetPointVelocity(ray.point);

                  Vector2 collideVelocity = rvel - otherRvel;
                  Vector2 cdn = castDirection.normalized;
                  float compression = (1 - fraction) * strutLength * springConstant;
                  float damping = (fraction - lastFraction) * springDamping * strutLength * (1/Time.fixedDeltaTime);//Vector2.Dot(castDirection.normalized, collideVelocity) * springDamping;
                  Vector2 compressionEffect = cdn * -compression;
                  Vector2 dampingEffect = cdn * damping;
                  Vector2 rightNormal = new Vector2(ray.normal.y, -ray.normal.x);
                  Vector2 pressure = compressionEffect + dampingEffect;
                  //Vector2 pressure = cdn * compression * damping * -1;
                  float pressureClamp = Mathf.Max(Vector2.Dot(pressure, ray.normal), 0);

                  Vector2 force = ray.normal * pressureClamp;
                  Vector2 drag = Vector2.zero;

                  if (friction > 0) {
                     drag = Vector2.Dot(collideVelocity, rightNormal) * rightNormal * friction;
                  }

                  body.AddForceAtPosition(force - drag, ray.point);
                  if (ray.rigidbody != null) {
                     ray.rigidbody.AddForceAtPosition(-force + drag, ray.point);
                  }

                  //Debug.DrawRay(ray.point, compressionEffect, Color.green);
                  //Debug.DrawRay(ray.point, dampingEffect, Color.magenta);
                  //Debug.DrawRay(ray.point, ray.normal * pressureClamp, Color.blue);
                  //Debug.DrawRay(ray.point, drag * 10, Color.red);
               }
               lastFraction = fraction;
               if (display != null) {
                  display.localPosition = displayLocalP + (displayEffector * (1 - fraction));
               }
               //Debug.DrawLine(castOrigin, Maths.tov3(ray.point), Color.red);
               //Debug.DrawRay(ray.point, ray.normal, Color.yellow);
            } else {
               fraction = 1;
               lastFraction = 1;
               if (display != null) {
                  display.localPosition = displayLocalP;
               }
               if (_landed) {
                  _landed = false;
                  if (_listener != null) {
                     _listener(false, null);
                  }
               }
            }
         }
      }
   }

   void OnDrawGizmosSelected() {
      if (Application.isPlaying) {
         Gizmos.color = Color.red;
         Vector3 torigin = transform.TransformPoint(origin);
         Vector3 tend = transform.TransformPoint(end);
         Vector3 tofs = (tend - torigin) * fraction;
         Gizmos.DrawRay(torigin, tofs);
         Gizmos.DrawWireSphere(torigin + tofs, castRadius);
      } else {
         Gizmos.color = Color.red;
         Vector3 torigin = transform.TransformPoint(origin);
         Vector3 tend = transform.TransformPoint(end);
         Gizmos.DrawLine(torigin, tend);
         Gizmos.DrawWireSphere(tend, radius * castScale);
      }
   }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LandingGearEffector))]
public class LGEEditor : Editor {
   void OnSceneGUI() {
      LandingGearEffector targ = (LandingGearEffector)target;
      Vector3 torigin = targ.transform.TransformPoint(targ.origin);
      Vector3 tend = targ.transform.TransformPoint(targ.end);
      float castScale = targ.castScale;
      float tradius = targ.radius * castScale;
      Handles.color = Color.green;
      Vector3 norigin = Handles.FreeMoveHandle(torigin, Quaternion.identity, 0.005f * castScale, Vector3.zero, Handles.CircleHandleCap);
      Vector3 nend = Handles.FreeMoveHandle(tend, Quaternion.identity, 0.005f * castScale, Vector3.zero, Handles.CircleHandleCap);
      float nradius = Handles.RadiusHandle(Quaternion.identity, tend, tradius);
      if (!norigin.Equals(torigin)) {
         Vector3 tx = norigin - torigin;
         targ.transform.InverseTransformVector(tx);
         targ.origin += tx;
      }
      if (!nend.Equals(tend)) {
         Vector3 tx = nend - tend;
         targ.transform.InverseTransformVector(tx);
         targ.end += tx;
      }
      if (nradius != tradius) {
         targ.radius = nradius / castScale;
      }
   }

   public override void OnInspectorGUI() {
      DrawDefaultInspector();
   }
}
#endif