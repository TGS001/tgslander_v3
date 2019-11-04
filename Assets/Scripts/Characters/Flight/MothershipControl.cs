using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlightAI))]
public class MothershipControl : MonoBehaviour {
    public float deployTime = 4.3f;
    public bool isLevelStart = false;
    public bool isLevelEnd = true;
    public Vector3 holdOffset = Vector3.down;
    public Vector3 bayOffset = Vector3.down * 3;
    public float holdSize = 2;
    public GameObject gateGraphic;
    public FlightPath postDeployPath;
    public FlightPath postEvacPath;

    public Sprite evacIcon;
    public string evacReason;
    [TextArea]
    public string evacDescription;

    [HideInInspector]
    [SerializeField]
    bool canstart = true;

    [HideInInspector]
    [SerializeField]
    bool gateOpen = false;

    [HideInInspector]
    [SerializeField]
    bool deploying = false;

    [HideInInspector]
    [SerializeField]
    bool deployed = false;

    [HideInInspector]
    [SerializeField]
    bool evacuating = false;

    [HideInInspector]
    [SerializeField]
    FlightAI ai;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(holdOffset), holdSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(bayOffset), holdSize / 3);
        postDeployPath.DrawGizmo(0);
        postEvacPath.DrawGizmo(0);
    }

    void SetGateOpen(bool state) {
        if (gateOpen != state) {
            gateOpen = state;
            gateGraphic.SetActive(state);
        }
    }

    // Use this for initialization
    void Start() {
        ai = GetComponent<FlightAI>();
    }

    private void OnEnable() {
        StartCoroutine(Tick());
    }

    private bool canEvac {
        get {
            return !ai.pathing && isLevelEnd && PlaySessionControl.CanEvac();
        }
    }

    IEnumerator EvacPlayer() {
        if (!evacuating && !deploying && PlaySessionControl.CanEvac()) {
            evacuating = true;
            PlaySessionControl.SetPlayerControllable(false);
            PlaySessionControl.Invulnerable(true);
            SpringJoint2D tractorConnector = gameObject.AddComponent<SpringJoint2D>();
            tractorConnector.connectedBody = PlaySessionControl.player.GetComponent<Rigidbody2D>();
            tractorConnector.autoConfigureDistance = false;
            float distance = Vector2.Distance(transform.InverseTransformPoint(PlaySessionControl.player.transform.position), holdOffset);
            tractorConnector.autoConfigureConnectedAnchor = false;
            tractorConnector.connectedAnchor = Vector2.zero;
            tractorConnector.dampingRatio = 0.5f;
            float t = 0;
            while (t < 1) {
                t += Time.deltaTime / deployTime;
                tractorConnector.distance = Mathf.Lerp(distance, 0, t);
                tractorConnector.anchor = Vector2.Lerp(holdOffset, bayOffset, t);
                yield return null;
            }
            SetGateOpen(false);
            Destroy(tractorConnector);
            PlaySessionControl.SetPlayerActive(false);
            PlaySessionControl.Evac(evacIcon, evacReason, evacDescription);
            PlaySessionControl.SetCameraTarget(transform);
            ai.ExecutePath(postEvacPath);
        }
    }

    IEnumerator DeployPlayer() {
        if (!deploying && !evacuating && !PlaySessionControl.IsPlayerActive()) {
            deploying = true;
            SetGateOpen(true);
            PlaySessionControl.WarpPlayer(transform.TransformPoint(bayOffset));
            PlaySessionControl.SetPlayerActive(true);
            PlaySessionControl.SetCameraTarget(PlaySessionControl.player.transform);
            SpringJoint2D tractorConnector = gameObject.AddComponent<SpringJoint2D>();
            tractorConnector.connectedBody = PlaySessionControl.player.GetComponent<Rigidbody2D>();
            tractorConnector.autoConfigureDistance = false;
            tractorConnector.distance = 0;
            tractorConnector.autoConfigureConnectedAnchor = false;
            tractorConnector.connectedAnchor = Vector2.zero;
            tractorConnector.dampingRatio = 0.5f;
            float t = 0;
            while (t < 1) {
                t += Time.deltaTime / deployTime;
                tractorConnector.anchor = Vector2.Lerp(bayOffset, holdOffset, t);
                yield return null;
            }
            SetGateOpen(false);
            Destroy(tractorConnector);
            PlaySessionControl.SetPlayerControllable(true);
            deployed = true;
            deploying = false;
            ai.ExecutePath(postDeployPath);
        }
    }

    IEnumerator Tick() {
        yield return new WaitForEndOfFrame();
        if (isLevelStart && canstart) {
            PlaySessionControl.SetPlayerActive(false);
            PlaySessionControl.SetPlayerControllable(false);
            PlaySessionControl.SetCameraTarget(transform);
            canstart = false;
        }

        while (true) {
            if (!deploying && !evacuating && PlaySessionControl.IsPlayerActive()) {
                if (isLevelEnd) {
                    if (canEvac) {
                        SetGateOpen(true);
                        if (Vector2.Distance(transform.TransformPoint(holdOffset), PlaySessionControl.player.transform.position) < holdSize) {
                            StartCoroutine(EvacPlayer());
                        }
                    } else {
                        SetGateOpen(false);
                    }
                }
            } else {
                if (isLevelStart && !deploying && !deployed) {
                    if (!ai.pathing && !PlaySessionControl.IsPlayerActive()) {
                        StartCoroutine(DeployPlayer());
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
