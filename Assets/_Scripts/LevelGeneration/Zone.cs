using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


// A class defining the properties of the zones in the terrain editor
public class Zone : ScriptableObject
{

    // Defining the variables
   public Vector2 position;
   public float radius;
   public bool aboveGround = false;
   public Vector2 multiMaterial;

    /// <summary>
    /// NEEDS WORK
    /// </summary>
    public Rect bounds {
        get {
            return new Rect(position, Vector2.one * radius);
        }
    }


    public bool PointCollides(Vector2 point, float inset = 0) {
      if (aboveGround && point.y > position.y) {
         return Mathf.Abs(position.x - point.x) < ((radius + (inset * Maths.root2)) + (point.y - position.y));
      } else {
         return (position - point).magnitude < radius + inset;
      }
   }

   public float PointDistance(Vector2 point) {
      if (aboveGround && point.y > position.y) {
         return Mathf.Abs(position.x - point.x) - (radius * 1.1f + (point.y - position.y));
      } else {
         return Vector2.Distance(point, position) - radius;
      }
   }

    internal float InnerDistance(Vector2 point)
    {
        return Vector2.Distance(point, position) / radius;
    }
};

#if UNITY_EDITOR

[CustomEditor(typeof(Zone))]
public class ZoneEditor : Editor {

    // Allowing the variables to be edited in unity
    SerializedProperty position;
    SerializedProperty radius;
    SerializedProperty aboveGround;
    SerializedProperty multiMaterial;

    // Updating the variables
    void sync() {
        position = serializedObject.FindProperty("position");
        radius = serializedObject.FindProperty("radius");
        aboveGround = serializedObject.FindProperty("aboveGround");
        multiMaterial = serializedObject.FindProperty("multiMaterial");
    }

    //Creating the GUI layout in the inspector
    public override void OnInspectorGUI() {
        serializedObject.Update();
        sync();
        EditorGUILayout.PropertyField(position);
        EditorGUILayout.PropertyField(radius);
        EditorGUILayout.PropertyField(aboveGround);
        EditorGUILayout.PropertyField(multiMaterial);
        serializedObject.ApplyModifiedProperties();
    }


    // Creating the GUI for the editor in the scene
    private void OnSceneGUI() {
        Zone t = (Zone)target;
        if (t.aboveGround) {
            Handles.color = Color.green;
        } else {
            Handles.color = Color.yellow;
        }
        Vector3 position = t.position;
        t.radius = Handles.RadiusHandle(Quaternion.identity, position, t.radius);
        Handles.color = Color.cyan;
        t.position = Handles.FreeMoveHandle(position, Quaternion.identity, t.radius * 0.8f, Vector3.zero, Handles.CircleHandleCap);
    }
}

/*
[CustomPropertyDrawer(typeof(Zone))]
class ZoneDrawer : PropertyDisplay
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        position = DrawBaseProperty(position, property);
        if (!property.isExpanded) {
            return;
        }
        DrawBackgroundRect(position);
        SerializedProperty pos = property.FindPropertyRelative("pos");
        SerializedProperty rad = property.FindPropertyRelative("rad");
        SerializedProperty ag = property.FindPropertyRelative("ag");
        position = DrawSubProperty(position, pos);
        position = DrawSubProperty(position, rad);
        position = DrawSubProperty(position, ag);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            return MeasureBaseProperty(property);
        }
        SerializedProperty pos = property.FindPropertyRelative("pos");
        SerializedProperty rad = property.FindPropertyRelative("rad");
        SerializedProperty ag = property.FindPropertyRelative("ag");
        float res = MeasureBaseProperty(property) + MeasureSubProperty(pos) + MeasureSubProperty(rad) + MeasureSubProperty(ag) + 4;
        return res;
    }
}
*/
#endif