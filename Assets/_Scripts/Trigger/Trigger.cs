using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Trigger))]
public class TriggerEditor : Editor {

}
#endif

public class Trigger : MonoBehaviour {
    private static List<Trigger> triggers = new List<Trigger>();

    public static void ScoreChanged(GameObject origin) {
        ActivateTypes(origin, Type.scoreChanged);
    }

    public static void DamageTaken(GameObject origin) {
        ActivateTypes(origin, Type.damageTaken);
    }

    private static void ActivateTypes(GameObject origin, Type t) {
        foreach (Trigger trig in triggers) {
            if (trig.trigger == t) {
                trig.Activate(origin);
            }
        }
    }

    public int activationsRemaining = -1;
    public int dialogPriority = 0;
    public Type trigger;
    public Condition[] conditions;
    public Action[] actions;

    public IEnumerator operation = null;

    [HideInInspector]
    public CommsDriver comms;

    public enum Type {
        passive,
        levelStart,
        scoreChanged,
        damageTaken
    }

    private void OnEnable() {
        triggers.Add(this);
        comms = FindObjectOfType<CommsDriver>();
    }

    private void OnDisable() {
        triggers.Remove(this);
    }

    private void Start() {
        if (trigger == Type.levelStart) {
            Activate(null);
        }
    }

    public void Activate(GameObject origin) {
        if (activationsRemaining == 0 || operation != null) {
            return;
        }
        foreach (Condition con in conditions) {
            if (!con.Validate(this, origin)) {
                return;
            }
        }
        if (comms) {
            comms.TryGrab(this);
        }
        if (activationsRemaining > 0) {
            activationsRemaining -= 1;
        }
        operation = Run(origin);
        StartCoroutine(operation);
    }

    IEnumerator Run(GameObject origin) {
        for (int i = 0; i < actions.Length; i++) {
            Action ca = actions[i];
            ca.Act(this, origin);
            while (!ca.Ready(this)) {
                yield return null;
            }
        }
        yield return null;
        operation = null;
        if (comms) {
            comms.Drop(this);
        }
    }
}
