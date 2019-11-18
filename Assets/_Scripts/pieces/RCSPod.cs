using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCSPod : MonoBehaviour {

	public float throttleDownTime = 0.5f;
	public float turnSpeed = 45;
	public float linearForce = 1;

	private RCSEngine[] engines;
	private Vector3 thrustDirection;
	private float percent;
	private float timer = 0;

	public void thrust(Vector3 direction, float throttle) {
		thrustDirection = direction;
		percent = throttle;
		timer = throttleDownTime;
      Debug.DrawRay(transform.position, direction.normalized * throttle, Color.red);
	}

	// Use this for initialization
	void Start () {
		engines = GetComponentsInChildren<RCSEngine> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0) {
			float timePercent = (timer / throttleDownTime);
         Vector3 tdn = thrustDirection.normalized;
			foreach (RCSEngine e in engines) {
				if (Vector3.Dot (thrustDirection, e.transform.forward) < 0) {
					e.Enable ();
               float d = Vector3.Dot (tdn, e.transform.forward) * -1;
					e.thrustAmount = percent * timePercent * d;
				} else {
					e.Disable ();
				}
			}
			timer -= Time.deltaTime;
			if (timer <= 0) {
				foreach (RCSEngine e in engines) {
					e.Disable ();
				}
			}
		}
	}
}
