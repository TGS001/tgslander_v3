using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZoneLink : ScriptableObject
{
   public int a;
   public int b;
};

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ZoneLink))]
class ZoneLinkDrawer : PropertyDisplay
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = DrawBaseProperty(position, property);
        if (!property.isExpanded) {
            return;
        }
        DrawBackgroundRect(position);
        SerializedProperty a = property.FindPropertyRelative("a");
        SerializedProperty b = property.FindPropertyRelative("b");
        position = DrawSubProperty(position, a);
        position = DrawSubProperty(position, b);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) {
            return MeasureBaseProperty(property);
        }
        SerializedProperty a = property.FindPropertyRelative("a");
        SerializedProperty b = property.FindPropertyRelative("b");
        float res = MeasureBaseProperty(property) + MeasureSubProperty(a) + MeasureSubProperty(b) + 4;
        return res;
    }
}
#endif