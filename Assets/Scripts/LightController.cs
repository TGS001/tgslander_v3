using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {
    public enum FlickerPattern {
        ALL,
        SINGLE_ROTATE,
        SINGLE_ROTATE_REV,
        ALTERNATE
    }

    public GameObject[] lights;
    public FlickerPattern pattern = FlickerPattern.ALL;
    public float frequency = 1;
    public float minDelay = 0;
    public float maxDelay = 0;
    public float minDutyCycle = 0.5f;
    public float maxDutyCycle = 0.5f;
    bool[] lightStatus;


    IEnumerator DoLightTimer(int light) {
        GameObject l = lights[light];
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        l.SetActive(true);
        yield return new WaitForSeconds(Random.Range(minDutyCycle, maxDutyCycle));
        l.SetActive(false);
        lightStatus[light] = true;
    }

    void TryLight(int light) {

        if (light < lightStatus.Length && lightStatus[light]) {
            lightStatus[light] = false;
            StartCoroutine(DoLightTimer(light));
        }
    }

    IEnumerator PatternAll() {
        while (true) {
            for (int i = 0; i < lights.Length; i++) {
                TryLight(i);
            }
            yield return new WaitForSeconds(frequency);
        }
    }

    IEnumerator PatternSingleRotate(int direction) {
        int place = Random.Range(0, lights.Length);
        while (true) {

            TryLight(place);
            place += direction;
            if (place < 0) {
                place = lights.Length - 1;
            }
            if (place >= lights.Length) {
                place = 0;
            }
            yield return new WaitForSeconds(frequency);
        }
    }

    IEnumerator PatternAlternate() {
        int place = Random.Range(0, 1);
        while (true) {

            for (int i = 0; i < lights.Length; i++) {
                if (i % 2 == place)
                    TryLight(i);
            }
            if (place == 0) {
                place = 1;
            } else {
                place = 0;
            }
            yield return new WaitForSeconds(frequency);
        }
    }

    private void Setup() {
        StopAllCoroutines();
        for (int i = 0; i < lights.Length; i++) {
            if (lights[i] != null) {
                lights[i].SetActive(false);
                lightStatus[i] = true;
            }
        }
        switch (pattern) {
            case FlickerPattern.ALL:
                StartCoroutine(PatternAll());
                break;
            case FlickerPattern.SINGLE_ROTATE:
                StartCoroutine(PatternSingleRotate(1));
                break;
            case FlickerPattern.SINGLE_ROTATE_REV:
                StartCoroutine(PatternSingleRotate(-1));
                break;
            case FlickerPattern.ALTERNATE:
                StartCoroutine(PatternAlternate());
                break;
            default:
                break;
        }
    }

    private void OnEnable() {
        lightStatus = new bool[lights.Length];
        Setup();
    }
}
