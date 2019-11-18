using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxScale : MonoBehaviour {
    public bool continuous;
    public SFX.ScalarField source;
    public float proportion = 1;
    SFX fx;

    private void Start() {
        fx = GetComponent<SFX>();
        float scale = fx.getScalar(source) * proportion;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnEnable() {
        if (continuous)
            StartCoroutine(ContinuousScale());
    }

    IEnumerator ContinuousScale() {
        yield return null;
        while (true) {
            float scale = fx.getScalar(source) * proportion;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
}
