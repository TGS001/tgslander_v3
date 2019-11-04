using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpeedMatcher : MonoBehaviour {
    AudioSource[] sources;
    float[] defaultPitches;

	void Start () {
        sources = GetComponents<AudioSource>();
        defaultPitches = new float[sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            defaultPitches[i] = sources[i].pitch;
        }
	}
	
	void LateUpdate () {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].pitch = defaultPitches[i] * Time.timeScale;
        }
	}
}
