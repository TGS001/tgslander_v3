using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(IsoSurfaceRegion))]
public class RegionPainter : MonoBehaviour
{
    public float depth = 0;
    public GameObject[] spawnObjects = new GameObject[1];
    [Delayed]
    public float step = 1;
    public float stepScatter = 0;
    public float normalOffset = 0;
    public float normalScatter = 0;
    public AnimationCurve pathOffsetScale = AnimationCurve.Linear(0, 1, 1, 1);
    public bool alignToSurface = true;
    public bool upTowardCamera = true;
    public Vector3 alignScatter = Vector3.zero;
    public AnimationCurve pathScale = AnimationCurve.Linear(0, 1, 1, 1);
    public float baseScale = 10;
    public float scaleScatter = 0;
    public string seed = "painter";
    IsoSurfaceRegion region;

    bool changed = false;
    bool canChange = false;

    void OnValidate()
    {
//        Debug.Log("OnValidate");
        step = Mathf.Max(step, 0.1f);
        stepScatter = Mathf.Abs(stepScatter);
        OnRegionChanged();
    }

    void OnRegionChanged()
    {
//        Debug.Log("OnRegionChanged");
        if (canChange)
        {
            changed = true;
        }
    }

    void DoModelPlacement()
    {
        if (changed && (spawnObjects.Length > 0))
        {
            step = Mathf.Max(step, 0.1f);
            GRand rand = new GRand(seed);
            changed = false;
            while (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
            transform.DetachChildren();
            IsoSurfaceRegion.Stepper stepper = new IsoSurfaceRegion.Stepper(region);
            stepper.Step(step * 0.5f);
            IsoSurfaceRegion.Stepper spawner;
            while (stepper.isInRegion)
            {
                if (stepScatter == 0)
                {
                    spawner = stepper;
                }
                else
                {
                    spawner = new IsoSurfaceRegion.Stepper(stepper, rand.Rangef(-stepScatter, stepScatter));
                }

                if (spawner.isInRegion)
                {
                    float progress = spawner.distance / region.length;
                    if (progress >= 0 && progress < 1)
                    {
                        float curve = pathScale.Evaluate(progress);
                        float offsetCurve = pathOffsetScale.Evaluate(progress);
                        if (curve > 0)
                        {

                            Vector2 position = spawner.position;
                            Vector2 normal = spawner.normal;

                            int choice = Mathf.FloorToInt(rand.Rangef(0, spawnObjects.Length));
                            int schoice = choice;
                            while (spawnObjects[schoice] == null)
                            {
                                schoice = (schoice + 1) % spawnObjects.Length;
                                if (schoice == choice)
                                {
                                    break;
                                }
                            }
                            if (spawnObjects[schoice] != null)
                            {
                                GameObject sp = Instantiate(spawnObjects[schoice], transform);
                                if (alignToSurface)
                                {
                                    Quaternion q;
                                    if (upTowardCamera)
                                    {
                                        q = Quaternion.LookRotation(normal, Vector3.back) *
                                           Quaternion.Euler(
                                              new Vector3(
                                                 rand.Rangef(-alignScatter.x, alignScatter.x),
                                                 rand.Rangef(-alignScatter.y, alignScatter.y),
                                                 rand.Rangef(-alignScatter.z, alignScatter.z)
                                              ));
                                    }
                                    else
                                    {
                                        q = Quaternion.LookRotation(Vector3.back, normal) *
                                           Quaternion.Euler(
                                              new Vector3(
                                                 rand.Rangef(-alignScatter.x, alignScatter.x),
                                                 rand.Rangef(-alignScatter.y, alignScatter.y),
                                                 rand.Rangef(-alignScatter.z, alignScatter.z)
                                              ));
                                    }
                                    sp.transform.rotation = q;
                                }
                                else
                                {
                                    Quaternion q;
                                    if (upTowardCamera)
                                    {
                                        q = Quaternion.LookRotation(Vector3.up, Vector3.back) * Quaternion.Euler(
                                           new Vector3(
                                              rand.Rangef(-alignScatter.x, alignScatter.x),
                                              rand.Rangef(-alignScatter.y, alignScatter.y),
                                              rand.Rangef(-alignScatter.z, alignScatter.z)
                                           ));
                                    }
                                    else
                                    {
                                        q = Quaternion.Euler(
                                           new Vector3(
                                              rand.Rangef(-alignScatter.x, alignScatter.x),
                                              rand.Rangef(-alignScatter.y, alignScatter.y),
                                              rand.Rangef(-alignScatter.z, alignScatter.z)
                                           ));
                                    }

                                    sp.transform.rotation = q;
                                }
                                Vector3 rp = position + normal * (normalOffset + rand.Rangef(-normalScatter, normalScatter)) * offsetCurve;
                                sp.transform.position = Vector3.forward * depth + rp;
                                sp.transform.localScale = Vector3.one * curve * (baseScale + rand.Rangef(-scaleScatter, scaleScatter));
                                sp.name = string.Concat(sp.name, transform.childCount.ToString());
                            }
                        }
                    }
                }
                stepper.Step(step);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if (Application.isPlaying)
        {
            enabled = false;
        }
        else
        {
            region = GetComponent<IsoSurfaceRegion>();
            canChange = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (changed)
        {
            DoModelPlacement();
        }
    }
}
