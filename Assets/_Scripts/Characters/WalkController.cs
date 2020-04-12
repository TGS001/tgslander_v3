using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkController : MonoBehaviour {
    public Transform pivot;
    public bool backFacing;
    public float pivotRotation;
    public float pivotSpeed;
    public float walkSpeed;
    public float jumpSpeed;

    Rigidbody2D body;
    Animator anim;

    int walkSpeedID;
    int onGroundID;

    [SerializeField]
    [ReadOnly]
    float targetWalkSpeed = 0;
    [SerializeField]
    [ReadOnly]
    bool jump;
    [ReadOnly]
    public float facing = 0;
    [ReadOnly]
    public bool facingLocked = false;

    public void SetFacingLock(bool flock) {
        facingLocked = flock;
    }

    public void SetFacing(float nf) {
        if (!facingLocked) {
            facing = nf;
        }
    }

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        walkSpeedID = Animator.StringToHash("walkSpeed");
        onGroundID = Animator.StringToHash("onGround");
        if (backFacing && pivot != null)
        {
            Vector3 euler = pivot.localEulerAngles;
            euler.y = 180;
            pivot.localEulerAngles = euler;
        }
	}

    public void Walk(float speed)
    {
        targetWalkSpeed = Mathf.Clamp(speed, -walkSpeed, walkSpeed);
    }

    public void Jump()
    {
        jump = true;
    }

    RaycastHit2D[] raycastResults = new RaycastHit2D[8];

    private void FixedUpdate()
    {
        bool onGround = true;
        if (body != null) { 
            onGround = (body.Cast(Vector2.down, raycastResults, 0.1f) > 0);

            if (onGround)
            {
                Vector2 walkAccel = Vector2.zero;
                walkAccel.x = (targetWalkSpeed - body.velocity.x) * Time.fixedDeltaTime;
                if (jump)
                {
                    walkAccel.y = jumpSpeed;
                }
                body.velocity += walkAccel;
                body.gravityScale = 0f;
                //Debug.DrawRay(transform.position, Vector3.up);
            }
            else
            {
                body.gravityScale = 1;
            }
        }
        jump = false;
        if (Mathf.Abs(targetWalkSpeed) > 0.01f) {
            SetFacing(-Mathf.Sign(targetWalkSpeed) * pivotRotation);
        }

        if (pivot != null)
        {
            float targetAngle;
            targetAngle = facing;
            if (backFacing)
            {
                targetAngle += 180;
            }
            Vector3 euler = pivot.localEulerAngles;
            euler.y = Mathf.MoveTowardsAngle(euler.y, targetAngle, pivotSpeed * Time.fixedDeltaTime);
            pivot.localEulerAngles = euler;
        }

        if (anim != null)
        {
            if (targetWalkSpeed == 0)
            {
                anim.SetFloat(walkSpeedID, 0);
            }
            else
            {
                anim.SetFloat(walkSpeedID, Mathf.Abs(body.velocity.x));
            }
            anim.SetBool(onGroundID, onGround);
        }
    }
}
