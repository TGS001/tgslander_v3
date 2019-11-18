using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour {
   public Transform source;
   public Transform destination;
   public Vector3 position;
   public Vector3 normal;
    public Vector3 velocity;
    public Vector3 offset;
   public float size;
   public float magnitude;
   public float time;
   public AllyGroup faction;

    public static SFX Spawn(SFX fx, Vector3 position) {
        if (fx == null) {
            return null;
        }
        FxConditionalReplace fcr = fx.GetComponent<FxConditionalReplace>();
        int recursions = 8;
        while (recursions > 0 && fcr && fcr.shouldReplace(position)) {
            fx = fcr.replacement;
            fcr = fx.GetComponent<FxConditionalReplace>();
            recursions -= 1;
        }
        fx = Instantiate(fx);
        fx.position = position;
        return fx;
    }

    public static SFX Spawn(SFX fx, Transform source) {
        SFX res = Spawn(fx, source.position);
        if (res == null) {
            return null;
        }
        res.source = source;
        return res;
    }

    public enum ScalarField {
        size,
        magnitude,
        time,
        completePercent,
        positionMagnitude,
        normalMagnitude,
        velocityMagnitude,
        offsetMagnitude
    }
    public enum VectorField {
        sourcePosition,
        sourceForward,
        destinationPosition,
        destinationForward,
        position,
        normal,
        velocity,
        offset
    }

    public float getScalar(ScalarField field) {
        switch (field) {
            case ScalarField.size:
                return size;
            case ScalarField.magnitude:
                return magnitude;
            case ScalarField.time:
                return time;
            case ScalarField.completePercent:
                return completePercent;
            case ScalarField.positionMagnitude:
                return position.magnitude;
            case ScalarField.normalMagnitude:
                return normal.magnitude;
            case ScalarField.velocityMagnitude:
                return velocity.magnitude;
            case ScalarField.offsetMagnitude:
                return offset.magnitude;
            default:
                break;
        }
        return 0;
    }

    public Vector3 getVector(VectorField field) {
        switch (field) {
            case VectorField.sourceForward:
                if (source) {
                    return source.forward;
                }
                break;
            case VectorField.sourcePosition:
                if (source) {
                    return source.position;
                }
                break;
            case VectorField.destinationForward:
                if (destination) {
                    return destination.forward;
                }
                break;
            case VectorField.destinationPosition:
                if (destination) {
                    return destination.position;
                }
                break;
            case VectorField.position:
                return position;
            case VectorField.normal:
                return normal;
            case VectorField.velocity:
                return velocity;
            case VectorField.offset:
                return offset;
            default:
                break;
        }
        return Vector3.zero;
    }

    float timer = 0;
    float startTime = 0;
   public void Stop() {
      SendMessage("FinishEffect", SendMessageOptions.DontRequireReceiver);
   }
	// Use this for initialization
	void Start () {
      if (time != 0) {
            timer = time;
            startTime = Time.time;
         Invoke("Stop", time);
      }
	}

    public float completePercent
    {
        get
        {
            if (timer > 0)
            {
                return Mathf.Clamp01((Time.time - startTime) / (timer));
            }
            return 0;
        }
    }
}
