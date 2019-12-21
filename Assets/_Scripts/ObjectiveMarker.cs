using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveMarker : MonoBehaviour {
    [Tooltip("the message to display when this objective is completed.")]
    public string scoreText;
    [Tooltip("the message to display when this objective is failed.")]
    public string failText;
    [Tooltip("the value of this objective.")]
    public int scoreValue;
    [Tooltip("if true, destroying this marker completes it.")]
    public bool completeOnDestroy;
    [Tooltip("if true, this marker can only be failed.")]
    public bool failOnly;
    [HideInInspector]
    public GoalController owner;
    [ReadOnly]
    [SerializeField]
    private bool _complete = false;
    [ReadOnly]
    [SerializeField]
    private bool _failed = false;
    // Use this for initialization
    public bool complete {
        get {
            return _complete;
        }
        set {
            SetComplete(value);
        }
    }

    public bool failed {
        get {
            return _failed;
        }
        set {
            SetFailed(value);
        }
    }

    private void OnDrawGizmos() {
        
    }

    public void Register(out int value, out bool failOnly) {
        value = scoreValue;
        failOnly = this.failOnly;
    }

    void Start() {
    }

    private void SetComplete(bool completion) {
        if (!_failed) {
            if (_complete != completion) {
                if (completion) {
                    owner.ChangeScore(scoreValue);
                    NoteControl nc = FindObjectOfType<NoteControl>();
                    if (nc)
                        nc.SendScoreNote(transform.position, scoreText, scoreValue);
                } else {
                    owner.ChangeScore(-scoreValue);
                }
                
                _complete = completion;
                Trigger.ScoreChanged(gameObject);
            }
        }
    }

    void SetFailed(bool failure) {
        if (!_complete) {
            if (_failed != failure) {

                if (failure) {
                    owner.ChangeFail(scoreValue);
                    NoteControl nc = FindObjectOfType<NoteControl>();
                    if (nc)
                        nc.SendScoreNote(transform.position, scoreText, -scoreValue);
                } else {
                    owner.ChangeFail(-scoreValue);
                }

                _failed = failure;
                Trigger.ScoreChanged(gameObject);
            }
        }
    }

    void OnDestroy() {
        //if (!failOnly) {
        //    if (completeOnDestroy) {
        //        SetComplete(true);
        //    } else {
        //        if (!_complete) {
        //            SetFailed(true);
        //        }
        //    }
        //} else {
        //    SetFailed(true);
        //}
    }
}
