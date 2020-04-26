using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {
    public float thrust = 4;
	public Vector3 throttleFactor = new Vector3 (1, 1, 1);
	public Vector3 flickerFactor = new Vector3 (0, 0, 0);
	public Vector3 burstScale = new Vector3 (1, 1, 1);
	public float burstSeconds = 0.5f;
	public float throttleDownSeconds = 0.5f;
	public float rotation = 1;
	public Renderer model;
	public AudioSource soundSource;
	public AudioSource soundSource2;
	public AudioClip igniteClip;
	public AudioClip runClip;
	public AudioClip extinguishClip;
	[Range(0,1)]
	public float fullThrottlePitch = 1;
	[Range(0,1)]
	public float zeroThrottlePitch = 0;

	private float throttle;
	private float throttleTime;
	private float burstTime;
	private bool firing = false;
	private bool wasFiring = false;

	private bool throttles;
	private bool bursts;

	private Vector3 startScale;
	private Vector3 throttleScale;
	private Vector3 flickerScale;

	private AudioSource primary = null;
	private AudioSource secondary = null;
	private bool fading = false;
	private class PendingFade
	{
		public AudioClip clip;
		public float seconds;
	}

	RingBuffer<PendingFade> fades = new RingBuffer<PendingFade> (8);

	private IEnumerator CrossFade(AudioClip crossfadeClip, float seconds) {
        //if (primary && secondary)
        {
            fading = true;
            if (primary == null) {
                primary = soundSource;
                secondary = soundSource2;
            } else {
                AudioSource temp = secondary;
                secondary = primary;
                primary = temp;
            }

            primary.clip = crossfadeClip;
            if (crossfadeClip != null) {
                primary.Play();
            }
            float secondaryVolume = secondary.volume;
            float progress = 0;

            while (progress < seconds) {
                float percent = progress / seconds;
                primary.volume = Mathf.Lerp(0, 1, percent);
                secondary.volume = Mathf.Lerp(secondaryVolume, 0, percent);
                progress += Time.deltaTime;
                yield return null;
            }

            primary.volume = 1;
            secondary.volume = 0;
            secondary.Stop();
            onCrossFadeFinish();
            fading = false;
        }
	}

	void StartCrossFade(AudioClip a, float seconds) {
		if (!fading) {
			StopCoroutine ("CrossFade");
			IEnumerator routine = CrossFade (a, seconds);
			StartCoroutine (routine);
		} else {
			PendingFade fade = new PendingFade ();
			fade.clip = a;
			fade.seconds = seconds;
			fades.push (fade);
		}
	}

	void ClearCrossfades() {
		fades.clear();
		if (primary != null) {
			primary.Stop ();
			secondary.Stop ();
		}
		StopCoroutine ("CrossFade");
	}

	void onCrossFadeFinish() {
		PendingFade fade = fades.pop ();
		if (fade != null) {
			StopCoroutine ("CrossFade");
			IEnumerator routine = CrossFade (fade.clip, fade.seconds);
			StartCoroutine (routine);
		}
	}

	private bool visible() {
		return model.enabled;
	}

	private void setVisible(bool vis) {
		model.enabled = vis;
	}

	float throttleTimeScale() {
		if (throttles) {
			return throttleTime / throttleDownSeconds;
		}
		return 1;
	}

	float burstTimeScale() {
		if (bursts) {
			return burstTime / burstSeconds;
		}
		return 0;
	}

	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
		throttleScale = new Vector3 (
			Mathf.Lerp(0, startScale.x, throttleFactor.x),
			Mathf.Lerp(0, startScale.y, throttleFactor.y),
			Mathf.Lerp(0, startScale.z, throttleFactor.z)
		);
		flickerScale = new Vector3(
			startScale.x * flickerFactor.x,
			startScale.y * flickerFactor.y,
			startScale.z * flickerFactor.z
		);

		setVisible (false);
		if (throttleDownSeconds == 0) {
			throttles = false;
		} else {
			throttles = true;
		}

		if (burstSeconds == 0) {
			bursts = false;
		} else {
			bursts = true;
		}

	}

	float randomScale(float f) {
		return ((Random.value * 2) - 1) * f;
	}

	void doScaling() {
		Vector3 throttleVec = Vector3.Lerp (throttleScale, startScale, throttle * throttleTimeScale());
		Vector3 burstVec = Vector3.Lerp (new Vector3 (1, 1, 1), burstScale, burstTimeScale());
		throttleVec.x *= burstVec.x + randomScale (flickerScale.x);
		throttleVec.y *= burstVec.y + randomScale (flickerScale.y);
		throttleVec.z *= burstVec.z + randomScale (flickerScale.z);
		transform.localScale = throttleVec;

		Quaternion localRotation = Quaternion.AngleAxis (rotation * Time.deltaTime, Vector3.forward);
		transform.localRotation = transform.localRotation * localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (visible ()) {
			doScaling ();
			throttleTime = Mathf.Max (throttleTime - Time.deltaTime, 0);
			burstTime = Mathf.Max (burstTime - Time.deltaTime, 0);
			if (firing != wasFiring) {
				if (firing) {
					float time = burstSeconds * 0.5f;
					float attackTime = time * 0.2f;
					ClearCrossfades ();
					StartCrossFade (igniteClip, attackTime);
					StartCrossFade (igniteClip, time - attackTime);
					StartCrossFade (runClip, time);
				} else {
					float time = throttleDownSeconds * 0.5f;
					StartCrossFade (extinguishClip, time);
					StartCrossFade (null, time);
				}
			}
			wasFiring = firing;
            if (soundSource && soundSource2) {
                soundSource.pitch = Mathf.Lerp(zeroThrottlePitch, fullThrottlePitch, throttle);
                soundSource2.pitch = Mathf.Lerp(zeroThrottlePitch, fullThrottlePitch, throttle);
            }
			firing = false;

			if (throttleTime == 0) {
				setVisible (false);
			}
		}
	}

	public void doThrust(float percent) {
		throttle = percent;
		throttleTime = throttleDownSeconds;
		if (percent > 0) {
			firing = true;
		}
		if (! visible ()) {
			
			burstTime = burstSeconds;

			setVisible (true);
			transform.localScale = new Vector3 ();
		}
	}
}
