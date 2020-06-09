using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
[RequireComponent(typeof(ParticleSystem))]
public class FxParticleScale : MonoBehaviour {
    public enum EffectSource {
        size,
        magnitude
    }
    public EffectSource source;
    public float defaultSource = 1;
    public bool affectShape = true;
    public bool affectSize = false;
    public bool affectCount = false;
    public bool affectSpeed = false;

    /*
    void ScaleCurve(ref ParticleSystem.MinMaxCurve curve, float scale)
    {
        AnimationCurve ac;
        switch (curve.mode)
        {
            case ParticleSystemCurveMode.Constant:
                curve.constant *= scale;
                break;
            case ParticleSystemCurveMode.Curve:
                ac = curve.curve;
                for (int i = 0; i < ac.keys.Length; i++)
                {
                    Keyframe f = ac.keys[i];
                    f.value *= scale;
                    ac.keys[i] = f;
                }
                break;
            case ParticleSystemCurveMode.TwoCurves:
                ac = curve.curveMin;
                for (int i = 0; i < ac.keys.Length; i++)
                {
                    Keyframe f = ac.keys[i];
                    f.value *= scale;
                    ac.keys[i] = f;
                }
                ac = curve.curveMax;
                for (int i = 0; i < ac.keys.Length; i++)
                {
                    Keyframe f = ac.keys[i];
                    f.value *= scale;
                    ac.keys[i] = f;
                }
                break;
            case ParticleSystemCurveMode.TwoConstants:
                curve.constantMin *= scale;
                curve.constantMax *= scale;
                break;
            default:
                break;
        }
    }
    */

    void ScaleSystem(ParticleSystem ps, float scale)
    {
        if (affectSpeed)
        {
            ParticleSystem.MainModule main = ps.main;
            main.startSpeedMultiplier = scale;
        }
        if (affectCount)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTimeMultiplier *= scale;
            emission.rateOverDistanceMultiplier *= scale;
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
            int count = emission.GetBursts(bursts);
            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Burst b = bursts[i];
                b.maxCount = (short)(b.maxCount * scale);
                b.minCount = (short)(b.minCount * scale);
                bursts[i] = b;
            }
            emission.SetBursts(bursts);
        }
        if (affectSize)
        {
            ParticleSystem.MainModule main = ps.main;
            main.startSizeMultiplier *= scale;
            main.startSizeXMultiplier *= scale;
            main.startSizeYMultiplier *= scale;
            main.startSizeZMultiplier *= scale;
        }
        if (affectShape)
        {
            ParticleSystem.ShapeModule shape = ps.shape;
            shape.radius *= scale;
            shape.scale *= scale;
            //shape.meshScale *= scale;
        }
    }
	void Start () {
	    if (affectCount || affectShape || affectSize || affectSpeed)
        {
            SFX fx = GetComponent<SFX>();
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ps.Stop(true);
            ps.Clear(true);
            float scale;
            switch (source)
            {
                case EffectSource.size:
                    scale = fx.size / defaultSource;
                    break;
                case EffectSource.magnitude:
                    scale = fx.magnitude / defaultSource;
                    break;
                default:
                    scale = 1;
                    break;
            }
            foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>())
            {
                ScaleSystem(system, scale);
            }
            ps.Play(true);
        }
        Destroy(this);
	}
}
