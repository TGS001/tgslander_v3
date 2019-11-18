using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaEffector2D))]
public class EffectorDirectionFixer : MonoBehaviour {
    public float addAngle;
    AreaEffector2D effector;
	// Use this for initialization
	void Start () {
        effector = GetComponent<AreaEffector2D>();
        effector.useGlobalAngle = true;
        FixAngle();
	}

    void FixAngle() {
        effector.forceAngle = (Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg) + addAngle;
    }
	
	// Update is called once per frame
	void Update () {
        FixAngle();
	}
}
