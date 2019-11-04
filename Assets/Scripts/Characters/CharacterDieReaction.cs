using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(AstronautController))]
[RequireComponent(typeof(WalkController))]
public class CharacterDieReaction : MonoBehaviour {
    public Animator animator;
    public SFX deathFx;
    public float deathSeconds;

    Life life;
    AstronautController ac;
    WalkController wc;
    float deathTime;

	void Start () {
        life = GetComponent<Life>();
        ac = GetComponent<AstronautController>();
        wc = GetComponent<WalkController>();
	}

    void OnMortalDie() {
        ac.ForceLock(this);

        wc.SetFacingLock(true);
        //wc.walkSpeed = 0;
        wc.Walk(0);

        ObjectiveMarker marker = GetComponent<ObjectiveMarker>();
        if (marker) {
            marker.complete = true;
        }

        animator.SetBool("dead", true);
        deathTime = Time.time + deathSeconds;
    }
	
	
	void Update () {
		if (life.Dead() && deathTime < Time.time) {
            SFX fx = SFX.Spawn(deathFx, transform.position);
            Destroy(gameObject);
        }
	}
}
