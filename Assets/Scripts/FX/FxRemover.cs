using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxRemover : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void FinishEffect() {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
