using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class EngineEffect : MonoBehaviour {
   public float ratio = 2;
   public float scrollSpeed = 1;
   public float randomWidth;
   public float randomLength;
   public float shutdownTime;
   SFX fx;
   LineRenderer line;
   float activeAmount = 1;
   float scroll = 0;
   int strengthId;
   int scrollId;

	// Use this for initialization
	void Start () {
      fx = GetComponent<SFX>();
      line = GetComponent<LineRenderer>();
      strengthId = Shader.PropertyToID("_Strength");
      scrollId = Shader.PropertyToID("_Scroll");
      if (!fx.source) {
         Destroy(gameObject);
      } else {
            transform.SetPositionAndRotation(fx.source.position, fx.source.rotation);
         transform.SetParent(fx.source);
      }
	}
	
	// Update is called once per frame
	void LateUpdate () {
      float mag = Mathf.Clamp01(fx.magnitude);
      float magsize = mag * fx.size * activeAmount;

      if (fx.source) {
         line.SetPosition(0, fx.source.position);
         line.SetPosition(1, fx.source.position + fx.source.forward * magsize * (ratio + randomLength * Random.value));
      }

      line.startWidth = magsize + randomWidth * Random.value;
      scroll += scrollSpeed * mag * Time.deltaTime;
      line.sharedMaterial.SetFloat(strengthId, mag * activeAmount);
      line.sharedMaterial.SetFloat(scrollId, scroll);
	}

   void FinishEffect() {
      StartCoroutine(ShutdownLoop());
   }

   IEnumerator ShutdownLoop() {
      while (activeAmount > 0) {
         activeAmount -= Time.deltaTime / shutdownTime;
         yield return null;
      }
      Destroy(gameObject);
   }
}
