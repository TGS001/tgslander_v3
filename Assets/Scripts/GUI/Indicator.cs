using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Indicator : MonoBehaviour {
    public Transform target;
    public GameObject onScreen;
    public GameObject offScreen;
    public float offScreenAngle;
    public PuzzleNode node;
    public ObjectiveMarker marker;
    public string instruction;
    Animator anim;
    TextMesh texm;
    bool hadTarget = false;
    bool hadNode = false;
    bool hadMarker = false;
    bool finished = false;

    private void OnEnable() {
        anim = GetComponent<Animator>();
        texm = GetComponentInChildren<TextMesh>();
    }

    // Use this for initialization
    void Start () {
        texm.text = instruction;
    }

    public void Finish() {
        anim.SetBool("done", true);
        finished = true;
    }

    public void Remove() {
        Destroy(gameObject);
    }

    private void LateUpdate() {
        Camera cam = Camera.main;
        Vector2 screenp = cam.WorldToScreenPoint(transform.position);
        bool onscreen;
        if (screenp.x > 0 && screenp.x < cam.pixelWidth && screenp.y > 0 && screenp.y < cam.pixelHeight) {
            onscreen = true;
            if (cam.transform.position.x > transform.position.x) {
                texm.transform.localPosition = new Vector3(0.6f, 0, 0);
                texm.anchor = TextAnchor.LowerLeft;
            } else {
                texm.transform.localPosition = new Vector3(-0.6f, 0, 0);
                texm.anchor = TextAnchor.LowerRight;
            }
            onScreen.SetActive(true);
            offScreen.SetActive(false);
        } else {
            onscreen = false;
            onScreen.SetActive(false);
            offScreen.SetActive(true);
        }

        if (!finished) {
            

            if (target) {
                transform.position = target.position;
                hadTarget = true;
            } else {
                if (hadTarget) {
                    Finish();
                }
            }

            if (!onscreen) {
                Vector2 dif = (transform.position - cam.transform.position).normalized;
                offScreen.transform.position = (Vector3)((Vector2)cam.transform.position + dif * cam.orthographicSize * 0.8f) + Vector3.back * 10;
                offScreen.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg + offScreenAngle);
            }

            if (node) {
                if (node.complete) {
                    Finish();
                }
                hadNode = true;
            } else if (hadNode) {
                Finish();
            }

            if (marker) {
                if (marker.complete) {
                    Finish();
                }
                hadMarker = true;
            } else if (hadMarker) {
                Finish();
            }
        }
    }
}
