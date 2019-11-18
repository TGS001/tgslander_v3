using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxExplosion : MonoBehaviour {
    public float sizeScale = 1;
    public float magScale = 1;
    public float physicsFactor = 1;
    SFX fx;

    private void OnEnable() {
        fx = GetComponent<SFX>();
    }

    // Use this for initialization
    void Start () {
        if (magScale > 0) {
            Collider2D[] overlap = Physics2D.OverlapCircleAll(transform.position, fx.size * sizeScale);
            HashSet<Rigidbody2D> bodies = new HashSet<Rigidbody2D>();
            foreach (Collider2D coll in overlap) {
                if (coll.attachedRigidbody) {
                    bodies.Add(coll.attachedRigidbody);
                }
            }


            foreach (Rigidbody2D body in bodies) {
                if (!Alliance.IsPartOf(body.gameObject, fx.faction)) {
                    Life m = body.GetComponentInParent<Life>();
                    if (m) {
                        m.Afflict((fx.magnitude * magScale), 1, body.transform.position - transform.position);
                    }
                }
            }

            if (physicsFactor > 0) {
                foreach (Rigidbody2D body in bodies) {
                    Vector2 bodyPos;
                    float bodyDistance;
                    Maths.ClosestPoint(body, transform.position, out bodyDistance, out bodyPos);
                    body.AddForceAtPosition(((bodyPos - (Vector2)transform.position) * fx.magnitude * magScale * physicsFactor) / Mathf.Max(bodyDistance, 0.1f), bodyPos);
                }
            }
        }
	}

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fx.size * sizeScale);
    }
}
