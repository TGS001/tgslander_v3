using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SFX))]
public class BeamLink : MonoBehaviour
{

    SFX sfx;
    LineRenderer line;
    [SerializeField]
    bool fading;

    IEnumerator DoFade(bool fadein)
    {
        float fade;
        float fadeTarget;
        if (fadein)
        {
            fade = 0;
            fadeTarget = 1;
        }
        else
        {
            fade = line.material.GetFloat("_Strength");
            fadeTarget = 0;
        }

        while (fade != fadeTarget)
        {
            fade = Mathf.MoveTowards(fade, fadeTarget, Time.deltaTime * sfx.magnitude);
            line.material.SetFloat("_Strength", fade);
            yield return null;
        }

        if (!fadein)
        {
            Destroy(gameObject);
        }
    }

    void Fade(bool fadein)
    {
        if (fading)
        {
            StopAllCoroutines();
            fading = false;
        }
        StartCoroutine(DoFade(fadein));
    }

    // Use this for initialization
    void Start()
    {
        sfx = GetComponent<SFX>();
        line = GetComponent<LineRenderer>();
        line.startWidth = sfx.size;
        Fade(true);
    }

    void FinishEffect()
    {
        Fade(false);
    }

    void LateUpdate()
    {
        if (sfx.source && sfx.destination)
        {
            line.SetPosition(0, sfx.source.position);
            line.SetPosition(1, sfx.destination.position);
        }
        else
        {
            if (!fading)
            {
                Fade(false);
            }
        }
    }
}
