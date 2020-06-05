using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxThrusterDirt : MonoBehaviour
{
    public ThrustControl thrustCtlr;
    public List<ParticleSystem> particleSystems;
    public CapsuleCollider2D triggerCollider;
    public Transform dustSource;
    public float dustYOffsetLowest = -7f;
    public float dustYOffsetHighest = -1f;
    public float collisionYOffsetFurthest = -4f;
    public float collisionYOffsetClosest = -1f;
    public float offsetChangeSpeed = .5f;
    public float thrustOffDelay = 1f;

    private float originalDustYOffset;
    private float originalColliderYOffset;

    private Vector3 localOffsetTarget;

    private bool isThrustOn = false;
    private float thrustOffTimer = 0f;
    // This was supposed to handle increasing some properties of the particle system based on thrust time but 
    // adjustments to the particle systems directly seemed to have the same effect
    //private float thrustOnTimer = 0f;
//    public float thrustOffsetEffect = 1f;
    private bool isDustOn = false;

    void Awake()
    {
        originalDustYOffset = Mathf.Clamp(dustSource.localPosition.y, dustYOffsetLowest, dustYOffsetHighest);
        AdjustDustSourcePosition(originalDustYOffset);
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<CapsuleCollider2D>();
        }
        originalColliderYOffset = triggerCollider.offset.y;
        AdjustColliderOffsetPosition(originalColliderYOffset);
    }

    private void AdjustColliderOffsetPosition(float y)
    {
        float yOffset = Mathf.Clamp(y, collisionYOffsetFurthest, collisionYOffsetClosest);

        triggerCollider.offset = new Vector2(0, yOffset);
    }

    private void AdjustDustSourcePosition(float y)
    {
        float yOffset = Mathf.Clamp(y, dustYOffsetLowest, dustYOffsetHighest);

        localOffsetTarget = new Vector3(dustSource.localPosition.x, yOffset, dustSource.localPosition.z);
    }

    void EnableDust(bool isOn)
    {
        bool prevDustOn = isDustOn;
        isDustOn = isOn;
        foreach (ParticleSystem ps in particleSystems)
        {
            if (prevDustOn && !isDustOn)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            else if (!prevDustOn && isDustOn)
            {
                ps.Play();
            }
        }
    }

    bool isContactingGround(Collider2D col)
    {
        return (col.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }

    bool HasThrustedRecently()
    {
        if (thrustCtlr != null)
        {
            return (isThrustOn || (!isThrustOn && thrustOffTimer > 0f));
        }
        else
        {
            return true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isContactingGround(col) && HasThrustedRecently())
        {
            EnableDust(true);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (isContactingGround(col))
        {
            Vector2 closestPt = col.ClosestPoint(new Vector2(transform.position.x, transform.position.y));
            dustSource.position = new Vector3(closestPt.x, closestPt.y, dustSource.position.z);
            EnableDust(HasThrustedRecently());
        }
        else
        {
            EnableDust(false);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        EnableDust(false);

        AdjustDustSourcePosition(originalDustYOffset);
    }

    private void FixedUpdate()
    {
        if (thrustCtlr != null)
        {
            bool prevThrustOn = isThrustOn;
            isThrustOn = thrustCtlr.isThrusting;
            if (isThrustOn)
            {
                AdjustColliderOffsetPosition(collisionYOffsetFurthest);
            }
            else
            {
                AdjustColliderOffsetPosition(collisionYOffsetClosest);
            }
            if (prevThrustOn && !isThrustOn)
            {
                thrustOffTimer = thrustOffDelay;
//                thrustOnTimer = 0f;
            }
            else if (!prevThrustOn && isThrustOn)
            {
                thrustOffTimer = 0f;
//                thrustOnTimer = Time.fixedDeltaTime;
            }
            //if (prevThrustOn && isThrustOn)
            //{
            //    thrustOnTimer += Time.fixedDeltaTime;
            //}
            if (thrustOffTimer > 0f)
            {
                thrustOffTimer -= Time.fixedDeltaTime;
            }
        }
    }
}
