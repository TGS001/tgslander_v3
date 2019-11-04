using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyGroup {
	None,
	Player,
	Enemy,
	Neutral
}

public class Alliance : MonoBehaviour {
	public AllyGroup faction = AllyGroup.None;

   public static bool IsPartOf(GameObject a, AllyGroup g) {
      Alliance A = (Alliance)a.GetComponentInParent<Alliance> ();
      return (A != null && A.faction == g);
   }

    internal static AllyGroup GetAlliance(MonoBehaviour a) {
        Alliance A = (Alliance)a.GetComponentInParent<Alliance>();
        if (A) {
            return A.faction;
        }
        return AllyGroup.None;
    }

    public static bool Exists(GameObject a, GameObject b) {
		Alliance A = (Alliance)a.GetComponentInParent<Alliance> ();
      Alliance B = (Alliance)b.GetComponentInParent<Alliance> ();
		if (A != null && B != null && A.faction != AllyGroup.None && B.faction != AllyGroup.None) {
			return A.faction == B.faction;
		}   
		return false;
	}
}
