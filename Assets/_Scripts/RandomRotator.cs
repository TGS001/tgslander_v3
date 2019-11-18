using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour {
    public float rotationSpeed = 90;
    public float directionChangeTime = 1;
    Vector3 rotation;
    Vector3 lastRotation;
    float timer;

	// Use this for initialization
	void Start () {
        rotation = Random.onUnitSphere * rotationSpeed;
        lastRotation = Random.onUnitSphere * rotationSpeed;
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (timer > directionChangeTime) {
            lastRotation = rotation;
            rotation = Random.onUnitSphere * rotationSpeed;
        }
        while (timer > directionChangeTime) {
            timer -= directionChangeTime;
        }
        float t = timer / directionChangeTime;
        Vector3 spin = Vector3.Lerp(lastRotation, rotation, t) * Time.deltaTime;
        transform.Rotate(spin);
        timer += Time.deltaTime;
	}
}
