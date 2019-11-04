using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraints : MonoBehaviour {
    public Vector2 min;
    public Vector2 max;

    [SerializeField]
    private List<CameraSegment> segments;
    private ModularLander player;
    //[SerializeField]
    //[HideInInspector]
    //private CameraSegment closest = null;
    //private float closestSetTime;

    //private Vector2 ctarget;
    //private Vector2 ltarget;
    //private Vector2 intermediate;
    //private bool initialized = false;

    private void Start() {
        player = FindObjectOfType<ModularLander>();
    }

    private void FixedUpdate() {
        Vector2 position = player.transform.position;
        if (position.x < min.x || position.x > max.x || position.y < min.y || position.y > max.y) {
            PlaySessionControl.Evac(null, "Level complete!", "You did all the things and then you left.");
        }
    }

    public void AddSegment(Vector2 a, Vector2 b, float ra, float rb) {
        if (segments == null) {
            segments = new List<CameraSegment>();
        }
        CameraSegment seg = ScriptableObject.CreateInstance<CameraSegment>();
        seg.positionA = a;
        seg.positionB = b;
        seg.radiusA = ra;
        seg.radiusB = rb;
        segments.Add(seg);
    }

    /*
    void SetupClosest(Vector2 position) {
        int segCount = segments.Count;
        CameraSegment bestSeg = segments[0];
        float bestDistance = bestSeg.Distance(position);
        for (int i = 1; i < segCount; i++) {
            CameraSegment curSeg = segments[i];
            float curDistance = curSeg.Distance(position);
            if (curDistance < bestDistance) {
                bestSeg = curSeg;
                bestDistance = curDistance;
            }
        }
        if (bestSeg != closest) {
            closestSetTime = Time.time;
            ltarget = intermediate;
            closest = bestSeg;
        }
    }
    */

    public Vector2 GetCameraTarget(Vector2 position, Vector2 scaledVelocity, float radius) {
        float bestdistance = float.MaxValue;
        Vector2 bestModifiedPosition = position;
        foreach (CameraSegment segment in segments) {
            if (segment.InBounds(position, radius)) {
                Vector2 cmp = segment.ModifyPosition(position, radius);
                float cd = Vector2.Distance(cmp, position);
                if (cd == 0) {
                    return cmp;
                } else if (cd < bestdistance) {
                    bestdistance = cd;
                    bestModifiedPosition = cmp;
                }
            }
        }
        return bestModifiedPosition;
        /*
        SetupClosest(position);

        ctarget = closest.GetCameraTarget(position);

        if (!initialized) {
            initialized = true;
            ltarget = ctarget;
        }

        intermediate = Vector2.Lerp(
           ltarget,
           ctarget,
           Time.time - closestSetTime);
        ltarget += scaledVelocity;
        return intermediate;
        */
    }

    public void Clear() {
        segments.Clear();
    }

    void OnDrawGizmosSelected() {
        if (!Application.isPlaying) {
            Vector2 mid = (min + max) * 0.5f;
            Vector3 origin = new Vector3(mid.x, mid.y);
            Vector3 outsideSize = new Vector3((max.x - mid.x) * 2, (max.y - mid.y) * 2, 1);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(origin, outsideSize);
            foreach (CameraSegment segment in segments) {
                if (segment)
                    segment.Gizmo(Color.cyan);
            }
        } else {
            /*
            if (closest != null) {

                closest.Gizmo(Color.cyan);
                Vector3 c = Maths.tov3(ctarget);
                Gizmos.DrawWireSphere(c, 1);

                Gizmos.color = Color.gray;
                Vector3 l = Maths.tov3(ltarget);
                Gizmos.DrawWireSphere(l, 1);
            }
            */
        }
    }
}
