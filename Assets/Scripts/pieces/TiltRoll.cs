using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TiltRoll : MonoBehaviour {
   public Transform source;
   public Transform destination;
   public float offset;
	
	// Update is called once per frame
	void Update () {
      Vector3 sourceRotation = source.localEulerAngles;
      Vector3 destRotation = destination.localEulerAngles;
      destRotation.y = -sourceRotation.z + offset;
      destination.localEulerAngles = destRotation;
	}
}
