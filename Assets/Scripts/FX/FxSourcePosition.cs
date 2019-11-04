using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxSourcePosition : MonoBehaviour {
    SFX fx;
	// Use this for initialization
	void Start () {
        fx = GetComponent<SFX>();
        if (fx.source)
        {
            transform.position = fx.source.transform.position;
        }
        else
        {
            transform.position = fx.position;
        }
	}
	
}
