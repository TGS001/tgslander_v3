using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxSubSpawner : MonoBehaviour {
    public bool useEmitterAsSource = false;
    public bool setSpawnedEffectTime = false;
    public Transform emitter;
    public SFX effect;
    public SFX.ScalarField size;
    public float sizeScale;
    public SFX.ScalarField magnitude;
    public float magnitudeScale;
    public SFX.ScalarField time;
    public float timeScale;

    public SFX.VectorField position;
    public SFX.VectorField normal;
    public SFX.VectorField velocity;
    public SFX.VectorField offset;
    public float delayMin = 0;
    public float delayMax = 0;
    public int emissionCount;
    public float subDelayMin = 1;
    public float subDelayMax = 1;
    SFX fx;

    private void Start() {
        fx = GetComponent<SFX>();
        StartCoroutine(doSpawn());
    }

    IEnumerator doSpawn() {
        float delay = Random.Range(delayMin, delayMax);
        if (delay > 0) {
            yield return new WaitForSeconds(delay);
        }
        Transform em;
        if (emitter) {
            em = emitter;
        } else {
            em = transform;
        }
        for (int i = 0; i < emissionCount; i++) {
            SFX nfx = SFX.Spawn(effect, fx.getVector(position));
            if (useEmitterAsSource) {
                nfx.source = em;
            } else {
                nfx.source = fx.source;
            }
            nfx.destination = fx.destination;
            nfx.size = fx.getScalar(size) * sizeScale;
            nfx.magnitude = fx.getScalar(magnitude) * magnitudeScale;
            if (setSpawnedEffectTime) {
                nfx.time = fx.getScalar(time) * timeScale;
            }
            nfx.normal = fx.getVector(normal);
            nfx.velocity = fx.getVector(velocity);
            nfx.offset = fx.getVector(offset);
            yield return new WaitForSeconds(Random.Range(subDelayMin, subDelayMax));
        }
    }
}
