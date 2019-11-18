using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Paralax : MonoBehaviour {
   public Vector2 paralaxScale = Vector2.one * 0.5f;

   void TryStart() {
      
   }

	// Use this for initialization
	void Start () {
      TryStart();
	}

   void OnEnable() {
      TryStart();
   }
	
	// Update is called once per frame
	void LateUpdate () {
      Vector3 tp = Camera.main.transform.position;
        tp.x *= 1 - paralaxScale.x;
        tp.y *= 1 - paralaxScale.y;
      tp.z = transform.position.z;
      transform.position = tp;
	}
}
