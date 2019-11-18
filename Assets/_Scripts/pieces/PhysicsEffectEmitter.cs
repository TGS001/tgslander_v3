using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsEffectEmitter : MonoBehaviour {
   public SFX[] effects = new SFX[1];
   public Collider2D[] colliders = new Collider2D[0];
   public bool excludeSelected = false;
   public float effectThreshold = 0;
   public float effectStep = 1;

   bool ColliderIsUsable(Collider2D col) {
      if (colliders.Length == 0) {
         return false;
      }
      foreach (Collider2D c in colliders) {
         if (c != null && c.Equals(col)) {
            return !excludeSelected;
         }
      }
      return excludeSelected;
   }

   void OnCollisionEnter2D(Collision2D coll) {
      if (effects.Length > 0) {
         for (int i = 0; i < coll.contacts.Length; i++) {
            ContactPoint2D c = coll.contacts[i];
            if (ColliderIsUsable(c.otherCollider)) {
               float mag = (coll.relativeVelocity.magnitude - effectThreshold) / effectStep;
               if (mag > 0) {
                  int choice = Mathf.Clamp(Mathf.FloorToInt(mag), 0, effects.Length - 1);
                  if (effects[choice] != null) {
                     SFX f = SFX.Spawn(effects[choice], c.point);
                     f.normal = c.normal;
                     f.magnitude = mag;
                  }
               }
               break;
            }
         }
      }
   }
}
