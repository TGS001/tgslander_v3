#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PropertyDisplay : PropertyDrawer {
    protected float MeasureSubProperty(SerializedProperty property) {
        GUIContent label = new GUIContent(property.name, null, property.tooltip);
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected float MeasureBaseProperty(SerializedProperty property) {
        GUIContent label = new GUIContent(property.name, null, property.tooltip);
        return EditorGUI.GetPropertyHeight(property, label, false);
    }

    protected Rect DrawSubProperty(Rect position, SerializedProperty property) {
        position.height = MeasureSubProperty(property);
        EditorGUI.PropertyField(position, property, true);
        position.y += position.height;
        return position;
    }

    protected Rect DrawBaseProperty(Rect position, SerializedProperty property) {
        float ph = MeasureBaseProperty(property);
        Rect propPosition = new Rect(position);
        propPosition.height = ph;
        EditorGUI.PropertyField(propPosition, property, false);
        position.height -= ph;
        position.y += ph;
        return position;
    }

    protected void DrawBackgroundRect(Rect bkg) {
        bkg.height -= 4;
        EditorGUI.DrawRect(bkg, Color.white * 0.7f);
    }
}
#endif