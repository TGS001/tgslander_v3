using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
   public Vector3 rotation = Vector3.zero;
	
	// Update is called once per frame
	void Update () {
      Vector3 lea = transform.localEulerAngles;
      lea += rotation * Time.deltaTime;
      transform.localEulerAngles = lea;
	}
}
