using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KinematicVelocityMatcher : MonoBehaviour {
    Vector3 lastPosition;
    Rigidbody2D body;
	// Use this for initialization
	void Start () {
        lastPosition = transform.position;
        body = GetComponent<Rigidbody2D>();
        if (!body.isKinematic) {
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        body.velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
	}
}
