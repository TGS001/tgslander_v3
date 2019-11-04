using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour {
    public Image target;
    public float fadeSeconds;
    public float partFade = 0.5f;

    public void FadeIn() {
        target.CrossFadeAlpha(0, fadeSeconds, true);
    }

    public void FadeOut() {
        target.CrossFadeAlpha(1, fadeSeconds, true);
    }

    public void FadePart() {
        target.CrossFadeAlpha(partFade, fadeSeconds * partFade, true);
    }
}
