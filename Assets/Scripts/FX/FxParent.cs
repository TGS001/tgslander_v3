using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxParent : MonoBehaviour {
    SFX fx;
	// Use this for initialization
	void Start () {
        fx = GetComponent<SFX>();
        transform.SetParent(fx.source);
	}
}
