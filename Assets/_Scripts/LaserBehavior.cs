using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour {

	float liveTime;

	// Use this for initialization
	void Start () {
		liveTime = 2;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position += gameObject.transform.up * Time.deltaTime * 20;
		liveTime -= Time.deltaTime;
		if (liveTime <= 0) {
			Destroy (gameObject);
		}
	}
}
