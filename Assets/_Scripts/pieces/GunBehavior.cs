using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FireType {
	singleShot,
	auto
}
public class GunBehavior : MonoBehaviour {
	
	public int magazine = 1;
	public FireType fireType = FireType.singleShot;
	public int burstSize;

	public float cycleTime = 0.5f;
	public float reloadTime;

	public GameObject projectile;

	private RingBuffer<Vector3> fireTargets = new RingBuffer<Vector3>(8);
	private Vector3 target;
	private float lastFireTime;
	private bool firing;
	private bool hasTargets;

	void advanceTargets() {
		if (!firing) {
			if (fireTargets.empty ()) {
				hasTargets = false;
			} else {
				target = fireTargets.pop ();
			}
		}
	}

	// Use this for initialization
	void Start () {
		burstSize = Mathf.Min (magazine, burstSize);
		firing = false;
		hasTargets = false;
		reloadTime = Mathf.Max (cycleTime, reloadTime);
	}

	bool fireShot() {
		if (Time.time - lastFireTime > cycleTime) {
			GameObject shot = (GameObject)Instantiate (projectile);
			RocketBehavior bhv = shot.GetComponentInChildren<RocketBehavior> ();
			shot.transform.position = transform.position;
			shot.transform.localRotation = transform.rotation;
			if (bhv != null) {
				Vector3 front = transform.up;
				float mag = (target - transform.position).magnitude;
				float ca = Vector3.Dot (front, (target - transform.position).normalized);
				bhv.target = new Vector2 (target.x, target.y);
				if (ca > -0.5f) {
					bhv.midpoint = new Vector2 (
						transform.position.x + front.x * mag * 0.25f, 
						transform.position.y + front.y * mag * 0.25f);
					bhv.travelRate *= 1.1f;
				} else {
					bhv.midpoint = new Vector2 (
						target.x + front.x * mag * 0.75f, 
						target.y + front.y * mag * 0.75f);
				}
			}
			lastFireTime = Time.time;
			return true;
		}
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		if (hasTargets) {
			switch (fireType) {
			case FireType.auto:
				break;
			case FireType.singleShot:
				if (fireShot ()) {
					advanceTargets ();
				}
				break;
			}
		}
	}

	public void fire(Vector3 target) {
		if (!hasTargets) {
			this.target = target;
			hasTargets = true;
		} else {
			fireTargets.push (target);
		}
	}
}
