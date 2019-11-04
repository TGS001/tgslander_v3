using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLifetimeEmitter : MonoBehaviour {
    public SFX spawnEffect;
    public SFX dieEffect;
    public SFX disableEffect;
    public bool removeOnDie = false;
    private bool dead = false;
    public float magnitude = 1;

    float size {
        get {
            return Maths.CalculateSize(this);
        }
    }
    
    void OnDisable() {
        if (disableEffect != null && !dead) {
            SFX effect = SFX.Spawn(disableEffect, transform.position);
            effect.source = transform;
            effect.size = size;
            effect.magnitude = magnitude;
        }
    }

    void OnDie() {
        if (dieEffect != null) {
            SFX effect = SFX.Spawn(dieEffect, transform.position);
            effect.source = transform;
            effect.size = size;
            effect.magnitude = magnitude;
        }
        dead = true;
        if (removeOnDie) {
            Destroy(gameObject);
        }
    }

    void Start() {
        if (spawnEffect != null) {
            SFX effect = SFX.Spawn(spawnEffect, transform.position);
            effect.source = transform;
            effect.size = size;
            effect.magnitude = magnitude;
        }
        Life m = GetComponentInParent<Life>();
        if (m != null) {
            m.Register(OnDie);
        }
    }

}
