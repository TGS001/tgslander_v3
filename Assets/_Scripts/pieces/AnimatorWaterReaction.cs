using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatorWaterReaction : MonoBehaviour {
    public Animator anim;
    public Vector3 offset;
    public string booleanParameter;
    public float frequency;

    private void OnEnable() {
        StartCoroutine(WaterTest());
    }

    bool Active() {
        return this.isActiveAndEnabled;
    }

    IEnumerator WaterTest() {
        yield return new WaitUntil(Active);
        int pid = Animator.StringToHash(booleanParameter);
        while (true) {
            yield return new WaitForSeconds(1 / frequency);
            if (anim) {
                Vector2 testpoint = transform.TransformPoint(offset);
                anim.SetBool(pid, Water.Submerged(testpoint));
            }
        }
    }
}
