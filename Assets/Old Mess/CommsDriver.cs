using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommsDriver : MonoBehaviour {
    enum State {
        off,
        reading,
        idling
    }
    public GameObject comms;
    public Animator commsAnimator;
    public Image portrait;
    public Text nametag;
    public Text dialog;
    public float readingSpeed = 0.37f;

    Trigger activeTrigger;
    Persona activePersona;
    State readState = State.off;
    float readTimer = 0;
    float idleTimer = 0;

    static int closedState = Animator.StringToHash("commsClosed");

    public bool Ready(Trigger trig) {
        return trig != activeTrigger || (readState == State.off && commsAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == closedState);
    }

    public bool CanInterrupt(Trigger trig) {
        return activeTrigger == null || activeTrigger.dialogPriority == 0 || trig.dialogPriority > activeTrigger.dialogPriority;
    }

    public bool TryGrab(Trigger trig) {
        if (CanInterrupt(trig)) {
            activeTrigger = trig;
            return true;
        }
        return false;
    }

    public void Drop(Trigger trig) {
        if (trig == activeTrigger) {
            activeTrigger = null;
        }
    }

    public void Message(Trigger trig, Persona per, string text, float idleSeconds, Transform target) {
        if (trig == activeTrigger) {
            StartTalk(per, text);

            if (target != null) {
                SFX indicator = SFX.Spawn(per.indicator, target);
            }

            if (idleSeconds > 0) {
                idleTimer = idleSeconds;
                readTimer = text.Length * readingSpeed;
            } else {
                idleTimer = 0;
                readTimer = (text.Length * readingSpeed) + idleSeconds;
            }
        }
    }

    void StartTalk(Persona per, string text) {
        readState = State.reading;
        activePersona = per;
        portrait.sprite = per.talk;
        nametag.text = per.displayName;
        dialog.text = text;
        //comms.SetActive(true);
        commsAnimator.SetBool("commsOpen", true);
    }

    void EndTalk() {
        readState = State.idling;
        portrait.sprite = activePersona.silent;
    }

    void CloseMessage() {
        readState = State.off;
        //comms.SetActive(false);
        commsAnimator.SetBool("commsOpen", false);
    }

    private void Update() {
        if (Input.GetButtonDown("SkipDialog")) {
            CloseMessage();
        }
        switch (readState) {
            case State.reading:
                if (readTimer <= 0) {
                    EndTalk();
                }
                readTimer -= Time.fixedDeltaTime;
                break;
            case State.idling:
                if (idleTimer <= 0) {
                    CloseMessage();
                }
                idleTimer -= Time.fixedDeltaTime;
                break;
            default:
                break;
        }
    }

    private void Start() {
        CloseMessage();
    }

    private void OnDestroy() {
        CloseMessage();
    }
}
