using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentBehavior : MonoBehaviour {
   public Transform fragmentParent;
   public float baseRadius = 1;
   public float maxRotation = 180;
   public float minRotation = 90;
   public float maxVerticalSpeed = 3;
   public float minVerticalSpeed = 1;
   public float finalScale = 0.5f;
   public float explodeTime = 1;
   public float variance = 0.1f;

   private Transform[] fragments;
   private Quaternion[] angleVelocities;
   private Vector3[] velocities;
   private Vector3[] startScales;
   private Vector3[] endScales;

   private float startTime;

	// Use this for initialization
	void Start () {
      if (fragmentParent == null) {
         fragmentParent = transform;
      }
      int childCount = fragmentParent.childCount;
      fragments = new Transform[childCount];
      angleVelocities = new Quaternion[childCount];
      velocities = new Vector3[childCount];
      startScales = new Vector3[childCount];
      endScales = new Vector3[childCount];

      for (int i = 0; i < childCount; i++) {
         Transform current = fragmentParent.GetChild(i);
         fragments[i] = current;
         Vector3 local = current.localPosition;
         float percent = Mathf.Min(((new Vector2(local.x, local.z)).magnitude / baseRadius) + (Random.value * variance) - (variance * 0.5f), 1);
         Vector3 rotationAxis = Vector3.Cross(Vector3.up, current.localPosition);
         angleVelocities[i] = Quaternion.AngleAxis(Mathf.Lerp(minRotation, maxRotation, percent), rotationAxis);
         velocities[i] = new Vector3(0, Mathf.Lerp(maxVerticalSpeed, minVerticalSpeed, percent), 0);
         startScales[i] = current.localScale;
         endScales[i] = current.localScale * finalScale;
      }

      startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
      float percent = (Time.time - startTime) / explodeTime;

      for (int i = 0; i < fragments.Length; i++) {
         Transform current = fragments[i];
         current.localPosition += velocities[i] * Time.deltaTime;
         current.localRotation = Quaternion.LerpUnclamped(current.localRotation, current.localRotation * angleVelocities[i], Time.deltaTime);
         current.localScale = Vector3.Lerp(startScales[i], endScales[i], percent);
      }

      if (percent > 1) {
         Destroy(gameObject);
      }
	}
}
