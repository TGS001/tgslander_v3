using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPart : MonoBehaviour {
    public GameObject[] spawnables;
    public float[] frequencies;

    float GetFrequency(int index) {
        if (frequencies.Length > index) {
            return frequencies[index];
        }
        return 1;
    }

	// Use this for initialization
	void Start () {
        float total = 0;
        for (int i = 0; i < spawnables.Length; i++) {
            total += GetFrequency(i);
        }
        total *= Random.value;
        for (int i = 0; i < spawnables.Length; i++) {
            float freq = GetFrequency(i);
            total -= freq;
            if (total <= 0) {
                GameObject g = Instantiate(spawnables[i]);
                g.transform.parent = transform;
                g.transform.localRotation = Quaternion.identity;
                g.transform.localPosition = Vector3.zero;
                break;
            }
        }
        Destroy(this);
    }
}


