using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SFX))]
[RequireComponent(typeof(ParticleSystem))]
public class FxParticleCleanup : MonoBehaviour {
    //SFX fx;
    ParticleSystem ps;
	// Use this for initialization
	void Start () {
        //fx = GetComponent<SFX>();
        ps = GetComponent<ParticleSystem>();
	}

    void FinishEffect() {
        ps.Stop(true);
    }

    // Update is called once per frame
    void Update () {
        if (!ps.IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
