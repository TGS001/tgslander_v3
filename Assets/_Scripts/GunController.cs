using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
   public float subFireDelay;
   public float shotCooldown;
   public float startFireRandomDelay;
   public float subFireRandomDelay;
    public int volleyCount = 1;
    public float volleyDelay = 0;
   ProjectileEmitter[] emitters;
   float nextShotTime;
	// Use this for initialization
	void Start () {
      emitters = GetComponentsInChildren<ProjectileEmitter>();
      nextShotTime = -1000;
      //Fire(Vector3.zero, null);
	}

   public bool Fire(Vector3 target, Transform t) {
      if (Time.time > nextShotTime) {
         nextShotTime = Time.time + shotCooldown;
         StartCoroutine(DoFiring(target, t));
         return true;
      }
      return false;
   }

   IEnumerator DoFiring(Vector3 target, Transform t) {
      yield return new WaitForSeconds(startFireRandomDelay * Random.value);
        for (int i = 0; i < volleyCount; i++) {
            foreach (ProjectileEmitter e in emitters) {
                e.Fire(target, t);
                yield return new WaitForSeconds(subFireDelay + subFireRandomDelay * Random.value);
            }
            yield return new WaitForSeconds(volleyDelay + subFireRandomDelay * Random.value);
        }
   }
}
