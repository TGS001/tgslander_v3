using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData {
	protected string _pk;
	public string pk { get { return _pk; } }

	public BaseData (string newPk) {
		_pk = newPk;
	}
}
