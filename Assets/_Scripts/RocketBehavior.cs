using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavior : MonoBehaviour {

	public Vector2 target;
	public float travelRate;
	public Vector2 midpoint;
	[Range(-0.25f,0.25f)]
	public float accelerate;
	public float waver;
	public float waverCycleDistance;
	public float explodeRadius;
	public int damage;
	public GameObject explosion;

	private Vector2 origin;
	private float distance;
	private float t;
	private float deltaT;
	private float waverScale;
	private float waverT, waverG;
	private float Z;
	private Vector3 lastPrimeOffset;
	private Vector3 primeOffset;

	float waverCycleT() {
		return (t / waverScale) + waverT;
	}

	float waverCycleG() {
		return (t / waverScale) + waverG;
	}

	Vector2 waverOffset(float ot) {
		Vector2 right = new Vector2 (primeOffset.x - lastPrimeOffset.x, primeOffset.y - lastPrimeOffset.y);
		right = new Vector2 (right.y, -right.x);
		float T = waverCycleT () * Mathf.PI * 2;
		float G = waverCycleG () * Mathf.PI * 2;
	
		Vector2 res = right.normalized * (Mathf.Cos (T) + Mathf.Sin (G)) * 0.5f * waver * Maths.ParabolaRange(ot);
		return res;
	}

	// Use this for initialization
	void Start () {
		origin.x = transform.position.x;
		origin.y = transform.position.y;
		Z = transform.position.z;

		distance = Mathf.Max((origin - midpoint).magnitude + (target - midpoint).magnitude, 0.001f);
		deltaT = travelRate/distance;
		waverScale = 1/(distance / waverCycleDistance);
		waverT = Random.value;
		waverG = Random.value;
		Vector2 normal = (origin - target).normalized;
		primeOffset = transform.position;
		lastPrimeOffset = transform.position + new Vector3 (normal.x, normal.y);
		Debug.DrawLine (
			new Vector3 (origin.x, origin.y), 
			new Vector3 (midpoint.x, midpoint.y),
			Color.red,
			5
		);
		Debug.DrawLine (
			new Vector3 (target.x, target.y), 
			new Vector3 (midpoint.x, midpoint.y),
			Color.red,
			5
		);
	}

	void explode() {
		Vector2 position2d = new Vector2 (transform.position.x, transform.position.y);
		Collider2D[] objects = Physics2D.OverlapCircleAll (position2d, explodeRadius);
		foreach (Collider2D c in objects) {
			if (!Alliance.Exists (gameObject, c.gameObject)) {
				Life.DoDamage (c.gameObject, damage);
			}
		}

		GameObject exp = (GameObject)Instantiate (explosion);
		ExplosionBehavior expb = exp.GetComponentInChildren<ExplosionBehavior> ();
		expb.radius = explodeRadius;
		expb.radiusEnd = explodeRadius + 1f;
		expb.rotations = 1/explodeRadius;
		expb.seconds = 0.2f;
		exp.transform.position = transform.position;

		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!Alliance.Exists (gameObject, other.gameObject)) {
			explode ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (t < 1) {
			t = Mathf.Min (t + deltaT * Time.deltaTime, 1);
			float ot = Maths.DistortT (t, accelerate);
			//Vector2 frameResult = Vector2.Lerp (origin, target, offsetT ()) + waverOffset ();
			//lastPrimeOffset = transform.position;
			Vector2 frameResult = Maths.ThreePointSpline(origin, midpoint, target, ot);
			lastPrimeOffset = primeOffset;
			primeOffset = new Vector3(frameResult.x, frameResult.y);
			if (waver > 0) {
				frameResult += waverOffset (ot);
			}

			transform.localRotation = Quaternion.LookRotation (primeOffset - lastPrimeOffset, new Vector3(0, 0, 1));
			transform.position = new Vector3 (frameResult.x, frameResult.y, Z);
			
		} else {
			explode ();
		}
	}
}
