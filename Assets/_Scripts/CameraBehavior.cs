using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

    public Transform target;
    CameraConstraints constraints;
    public Camera cam;

    [Range(0, 1)]
    public float screenFraction = 0.3f;

    [SerializeField]
    private Vector2 retainedVelocity;
    [SerializeField]
    private Vector2 retainedPosition;
    [SerializeField]
    private Vector2 targetPosition;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;
    private bool cameraIsShaking = false;

    public float halfHeight {
        get {
            return cam.orthographicSize;
        }
    }

    internal void SetTarget(Transform t) {
        target = t;
        retainedPosition = t.position;
        retainedVelocity = Vector2.zero;
        position = t.position;
    }

    public float height {
        get {
            return 2 * cam.orthographicSize;
        }
    }

    public float width {
        get {
            return cam.aspect * height;
        }
    }

    public float halfWidth {
        get {
            return halfHeight * cam.aspect;
        }
    }

    public float left {
        get {
            return transform.position.x - halfWidth;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.x = value + halfWidth;
            transform.position = newpos;
        }
    }

    public float right {
        get {
            return transform.position.x + halfWidth;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.x = value - halfWidth;
            transform.position = newpos;
        }
    }

    public float top {
        get {
            return transform.position.y + halfHeight;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.y = value - halfHeight;
            transform.position = newpos;
        }
    }

    public float bottom {
        get {
            return transform.position.y - halfHeight;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.y = value + halfHeight;
            transform.position = newpos;
        }
    }

    public float innerLeft {
        get {
            return transform.position.x - halfWidth * screenFraction;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.x = value + halfWidth * screenFraction;
            transform.position = newpos;
        }
    }

    public float innerRight {
        get {
            return transform.position.x + halfWidth * screenFraction;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.x = value - halfWidth * screenFraction;
            transform.position = newpos;
        }
    }

    public float innerTop {
        get {
            return transform.position.y + halfHeight * screenFraction;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.y = value - halfHeight * screenFraction;
            transform.position = newpos;
        }
    }

    public float innerBottom {
        get {
            return transform.position.y - halfHeight * screenFraction;
        }

        set {
            Vector3 newpos = transform.position;
            newpos.y = value + halfHeight * screenFraction;
            transform.position = newpos;
        }
    }

    public Vector2 position {
        get {
            return transform.position;
        }

        set {
            Vector3 x = transform.position;
            x.x = value.x;
            x.y = value.y;
            x.z = transform.position.z;
            transform.position = x;
        }
    }



    // Use this for initialization
    void Start() {
        constraints = FindObjectOfType<CameraConstraints>();
        PlaySessionControl.cam = this;
        SetTarget(target);
    }

    void Awake() {
    }

    void OnDrawGizmosSelected() {
        if (cam != null) {
            Vector3 sfraction = new Vector3(screenFraction * width, screenFraction * height, 1);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(gameObject.transform.position, sfraction);
        }
    }
    void Update()
    {
        // Calculate a fake delta time, so we can Shake while game is paused.
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame;
    }

    public void Shake(float duration, float amount)
    {
        cameraIsShaking = true;
        _originalPos = transform.localPosition;
        StopAllCoroutines();
        StartCoroutine(cShake(duration, amount));
    }

    public IEnumerator cShake(float duration, float amount)
    {
        float endTime = Time.time + duration;

        while (duration > 0)
        {
            transform.localPosition = _originalPos + UnityEngine.Random.insideUnitSphere * amount;

            duration -= _fakeDelta;

            yield return null;
        }

        transform.localPosition = _originalPos;
        cameraIsShaking = false;
    }
    // Update is called once per frame
    void LateUpdate() {
        if (target != null && !PlaySessionControl.paused && !cameraIsShaking) {
            targetPosition = target.transform.position;
            //targetPosition = Vector2.Lerp(retainedPosition, targetPosition, Time.deltaTime);
            Vector2 scaledVelocity = (targetPosition - retainedPosition);
            Vector2 frameVelocity;
            if (Time.deltaTime > 0) {
                frameVelocity = scaledVelocity * (1 / Time.deltaTime);
            } else {
                return;
            }
            retainedPosition = targetPosition;

            retainedVelocity = Vector2.Lerp(retainedVelocity, frameVelocity, Time.deltaTime);
            targetPosition = constraints.GetCameraTarget(targetPosition + retainedVelocity, retainedVelocity * Time.deltaTime, cam.orthographicSize);

            //Vector2 ccp = Camera.main.WorldToScreenPoint(targetPosition);
            //ccp.x = Mathf.Floor(ccp.x);
            //ccp.y = Mathf.Floor(ccp.y);
            //targetPosition = Camera.main.ScreenToWorldPoint(ccp);

            position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * Mathf.Max((frameVelocity.magnitude * 2), 1));


            if (target.position.x < innerLeft) {
                innerLeft = target.position.x;
            }

            if (target.position.x > innerRight) {
                innerRight = target.position.x;
            }

            if (target.position.y < innerBottom) {
                innerBottom = target.position.y;
            }

            if (target.position.y > innerTop) {
                innerTop = target.position.y;
            }

            if (left < constraints.min.x) {
                left = constraints.min.x;
            }

            if (right > constraints.max.x) {
                right = constraints.max.x;
            }

            if (top > constraints.max.y) {
                top = constraints.max.y;
            }

            if (bottom < constraints.min.y) {
                bottom = constraints.min.y;
            }



        }
    }
}
