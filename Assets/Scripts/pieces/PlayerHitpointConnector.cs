using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class PlayerHitpointConnector : MonoBehaviour {
	// Use this for initialization
	void Start () {
      Slider targ = GetComponent<Slider>();
      ModularLander lander = FindObjectOfType<ModularLander>();
      if (lander != null) {
         Life m = lander.GetComponent<Life>();
         m.displaySlider = targ;
      }
	}
}
