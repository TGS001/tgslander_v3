using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour {
	public float radius;
	public float radiusEnd;
	public float rotations;
	public float seconds;

	private float t;
	private float tscale;
	private Vector3 startScale;
	private Vector3 endScale;
	private Color startColor;
	private Color endColor;
	private Renderer ren;
	private int colorid;

	// Use this for initialization
	void Start () {
		t = 0;
		startScale = new Vector3 (radius, radius, radius);
		transform.localScale = startScale;
		transform.localRotation = Quaternion.identity;
		endScale = new Vector3 (radiusEnd, radiusEnd, radiusEnd);
		ren = GetComponentInChildren<Renderer> ();
		colorid = Shader.PropertyToID ("_TintColor");
		startColor = ren.material.GetColor (colorid);
		endColor = Color.clear;
		tscale = 1 / seconds;
	}

	void Update() {
		if (t >= 1) {
			Destroy (gameObject);
		}
		t += Time.deltaTime * tscale;
		transform.localScale = Vector3.Lerp (startScale, endScale, t);
		transform.localRotation = Quaternion.Euler(new Vector3 (0, t * rotations * 360));
		ren.material.SetColor(colorid, Color.Lerp (startColor, endColor, t));
	}
}
