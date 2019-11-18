using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDisplay {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        position = DrawBaseProperty(position, property);
        if (!property.isExpanded) {
            return;
        }
        DrawBackgroundRect(position);
        SerializedProperty type = property.FindPropertyRelative("type");
        Condition.Type t = (Condition.Type)type.enumValueIndex;

        Rect lr = new Rect(position);

        lr = DrawSubProperty(lr, type);
        
        switch (t) {
            case Condition.Type.objectivesComplete: {
                    SerializedProperty objectives = property.FindPropertyRelative("objectives");
                    lr = DrawSubProperty(lr, objectives);
                }
                break;

            case Condition.Type.originExists:
                break;

            case Condition.Type.originIsTarget: {
                    SerializedProperty target = property.FindPropertyRelative("target");
                    lr = DrawSubProperty(lr, target);
                }
                break;

            case Condition.Type.originAboveHealthPercent:
            case Condition.Type.originBelowHealthPercent: {
                    SerializedProperty healthPercent = property.FindPropertyRelative("healthPercent");
                    lr = DrawSubProperty(lr, healthPercent);
                }
                break;

            case Condition.Type.targetAboveHealthPercent:
            case Condition.Type.targetBelowHealthPercent: {
                    SerializedProperty target = property.FindPropertyRelative("target");
                    lr = DrawSubProperty(lr, target);
                    SerializedProperty healthPercent = property.FindPropertyRelative("healthPercent");
                    lr = DrawSubProperty(lr, healthPercent);
                }
                break;
            default:
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            return MeasureBaseProperty(property);
        }
        SerializedProperty type = property.FindPropertyRelative("type");
        Condition.Type t = (Condition.Type)type.enumValueIndex;
        float res = MeasureBaseProperty(property) + MeasureSubProperty(type) + 4;
        switch (t) {
            case Condition.Type.objectivesComplete: {
                    SerializedProperty objectives = property.FindPropertyRelative("objectives");
                    res += MeasureSubProperty(objectives);
                }
                break;

            case Condition.Type.originExists:
                break;

            case Condition.Type.originIsTarget: {
                    SerializedProperty target = property.FindPropertyRelative("target");
                    res += MeasureSubProperty(target);
                }
                break;

            case Condition.Type.originAboveHealthPercent:
            case Condition.Type.originBelowHealthPercent: {
                    SerializedProperty healthPercent = property.FindPropertyRelative("healthPercent");
                    res += MeasureSubProperty(healthPercent);
                }
                break;

            case Condition.Type.targetAboveHealthPercent:
            case Condition.Type.targetBelowHealthPercent: {
                    SerializedProperty target = property.FindPropertyRelative("target");
                    res += MeasureSubProperty(target);
                    SerializedProperty healthPercent = property.FindPropertyRelative("healthPercent");
                    res += MeasureSubProperty(healthPercent);
                }
                break;
            default:
                break;
        }
        return res;
    }
}
#endif

[System.Serializable]
public class Condition {
    public enum Type {
        objectivesComplete,
        originExists,
        originIsTarget,
        originAboveHealthPercent,
        originBelowHealthPercent,
        targetAboveHealthPercent,
        targetBelowHealthPercent,
        canUseComms
    }
    public Type type;
    public GameObject target;
    public List<ObjectiveMarker> objectives;
    public float healthPercent;

    public bool Validate(Trigger trigger, GameObject origin) {
        switch (type) {
            case Type.objectivesComplete:
                foreach (ObjectiveMarker objective in objectives) {
                    if (objective == null) {
                        continue;
                    }
                    if (!objective.complete) {
                        return false;
                    }
                }
                break;

            case Type.originExists:
                return origin != null;

            case Type.originIsTarget:
                return origin == target;

            case Type.originAboveHealthPercent:
            case Type.originBelowHealthPercent:
                if (origin == null) {
                    return false;
                } else {
                    Life life = origin.GetComponentInParent<Life>();
                    if (life) {
                        if (type == Type.originAboveHealthPercent) {
                            return life.percent > healthPercent;
                        } else {
                            return life.percent < healthPercent;
                        }
                    }
                    return false;
                }

            case Type.targetAboveHealthPercent:
            case Type.targetBelowHealthPercent:
                if (target == null) {
                    return false;
                } else {
                    Life life = target.GetComponentInParent<Life>();
                    if (life) {
                        if (type == Type.targetAboveHealthPercent) {
                            return life.percent > healthPercent;
                        } else {
                            return life.percent < healthPercent;
                        }
                    }
                    return false;
                }

            case Type.canUseComms: {
                    return trigger.comms.CanInterrupt(trigger);
                }

            default:
                break;
        }
        return true;
    }
}