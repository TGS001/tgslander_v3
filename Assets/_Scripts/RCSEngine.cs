using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCSEngine : MonoBehaviour {
	public Vector3 flickerScale;
	public float thrustAmount;
	private Vector3 startScale;
	private Vector3 flickerAmounts;
	private Renderer ren;

	public void Enable() {
		if (!ren.enabled) {
			ren.enabled = true;
		}
	}

	public void Disable() {
		if (ren.enabled) {
			ren.enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
      flickerAmounts.x = flickerScale.x * startScale.x;
		flickerAmounts.y = flickerScale.y * startScale.y;
      flickerAmounts.z = flickerScale.z * startScale.z;
		ren = GetComponent<Renderer> ();
		ren.enabled = (thrustAmount > 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (ren.enabled) {
			transform.localScale = new Vector3 (
				startScale.x + flickerAmounts.x * Random.value,
				startScale.y + flickerAmounts.y * Random.value,
				startScale.z + flickerAmounts.z * Random.value) * thrustAmount;
		}
	}
}
