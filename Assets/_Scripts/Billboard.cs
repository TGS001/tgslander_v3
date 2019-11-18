using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
    public enum Axis {
        up, right, forward
    };

    public Axis axis;

    static Quaternion billboardUp =
        Quaternion.LookRotation(Vector3.up, -Vector3.forward);
    static Quaternion billboardRight =
        Quaternion.LookRotation(Vector3.right, -Vector3.forward);
    static Quaternion billboardForward =
        Quaternion.LookRotation(Vector3.forward, -Vector3.forward);
    private void LateUpdate() {
        switch (axis) {
            case Axis.up:
                transform.rotation = billboardUp;
                break;
            case Axis.right:
                transform.rotation = billboardRight;
                break;
            case Axis.forward:
                transform.rotation = billboardForward;
                break;
            default:
                break;
        }
    }
}
