using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxFaceForward : MonoBehaviour {
    public SFX.VectorField field = SFX.VectorField.normal;

    SFX fx;
	void Start () {
        fx = GetComponent<SFX>();
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, fx.getVector(field).normalized);
	}
}
