using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WalkController))]
public class AstronautController : MonoBehaviour
{
    public bool rescuable = true;
    AstronautAction _action = null;
    public SFX rescueEffect;
    public float rescueSize;
    public float rescueAnimTime;

    [ReadOnly]
    public bool _acting = false;
    [ReadOnly]
    public MonoBehaviour _lock;

    public bool acting
    {
        get { return _acting; }
        set { _acting = value; }
    }

    public bool Lock(MonoBehaviour origin) {
        if (_lock != null) {
            return _lock == origin;
        }
        _lock = origin;
        SetAction(null);
        return true;
    }

    public void ForceLock(MonoBehaviour origin) {
        _lock = origin;
        SetAction(null);
    }

    public void Unlock(MonoBehaviour origin) {
        if (_lock == origin) {
            _lock = null;
        }
    }

    public bool interruptable
    {
        get
        {
            if (_lock != null) {
                return false;
            }
            if (_action == null)
            {
                return true;
            }
            if (!action.interruptable)
            {
                return !_acting;
            }
            return true;
        }
    }

    public float priority
    {
        get
        {
            if (_action == null)
            {
                return float.MinValue;
            }
            return _action.priority;
        }
    }

    public AstronautAction action
    {
        get
        {
            return _action;
        }
    }

    public bool isRescued {  get
        {
            if (marker != null)
            {
                return marker.complete;
            }
            else return false;
        }
    }

    private ModularLander lander;
    private WalkController walk;
    private Animator anim;
    private ObjectiveMarker marker;
    Rigidbody2D rb;

    public void Rescue()
    {
        if (rescuable && marker != null && rb != null)
        {
            rescuable = false;
            SetAction(null);
            acting = true;
            if (anim)
            {
                if (Random.value > 0.4f)
                {
                    anim.SetTrigger("salute");
                }
                else
                {
                    anim.SetTrigger("rescue");
                }
            }
            if (rescueEffect)
            {
                SFX fx = SFX.Spawn(rescueEffect, transform.position);
                fx.size = rescueSize;
                fx.magnitude = 1;
            }
            marker.complete = true;
            enabled = false;
            rb.simulated = false;
            Invoke("FinishRescue", rescueAnimTime);
        }
    }

    void FinishRescue()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        walk = GetComponent<WalkController>();
        lander = FindObjectOfType<ModularLander>();
        anim = GetComponentInChildren<Animator>();
        marker = GetComponent<ObjectiveMarker>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Walk(float direction)
    {
        if (walk != null)
        {
            walk.Walk(direction);
        }
    }

    public void SetAction(AstronautAction newAction)
    {
        StopCoroutine("Think");
        if (_action != null)
        {
            _action.FinishAction(this);
            acting = false;
        }
        _action = newAction;
        if (_action != null)
        {
            _action.StartAction(this);
            StartCoroutine("Think");
            acting = true;
        }
    }

    private void FixedUpdate()
    {
        if (lander != null)
        {
            if (rescuable && Vector2.SqrMagnitude(lander.transform.position - transform.position) < rescueSize * rescueSize)
            {
                Rescue();
            }
        }
    }

    private void OnDestroy()
    {
        SetAction(null);
    }

    private IEnumerator Think()
    {
        while (_action != null)
        {
            float delay;
            bool complete;
            _action.TickAction(this, out complete, out delay);

            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }

            if (complete)
            {
                SetAction(null);
            }
        }
        acting = false;
    }
}
