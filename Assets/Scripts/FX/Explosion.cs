using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
[RequireComponent(typeof(MeshRenderer))]
public class Explosion : MonoBehaviour {
   public SFX smokeEffect;

   SFX sfx;
   MeshRenderer ren;
   int tintID;
   Color startColor;
   Color endColor;
   float startTime;
   Vector3 startScale;
   Vector3 endScale;

	// Use this for initialization
	void Start () {
      sfx = GetComponent<SFX>();
      ren = GetComponent<MeshRenderer>();
      tintID = Shader.PropertyToID("_TintColor");
      startColor = ren.material.GetColor(tintID) * (sfx.magnitude/10);
      endColor = Color.clear;
      startTime = Time.time;
      startScale = sfx.size * Vector3.one * 2;
      endScale = sfx.size * Vector3.one * Maths.root2 * 2;
      transform.position = sfx.position;
      if (smokeEffect != null) {
         SFX smoke = SFX.Spawn(smokeEffect, transform.position);
         smoke.magnitude = sfx.magnitude;
         smoke.size = sfx.size;
      }
	}

   void FinishEffect() {
      Destroy(gameObject);
   }
	
	// Update is called once per frame
	void Update () {
      if (ren.enabled) {
         float progress = (Time.time - startTime) / sfx.time;

         if (progress > 1) {
            ren.enabled = false;
            return;
         }

         ren.material.SetColor(tintID, Color.Lerp(startColor, endColor, progress));
         transform.localScale = Vector3.Lerp(startScale, endScale, progress);
      }
	}
}
