using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpansionType {
	Hull,
	Engine,
	Thruster,
	Weapon,
	Wing,
	Strut,
	Shield
}

public class ExpansionNode : MonoBehaviour {
	public ExpansionType expansion;
	private bool finished = false;
	public void Finish() {
		finished = true;
	}

   public void Reset() {
      finished = false;
   }

	public bool Finished() {
		return finished;
	}
}
