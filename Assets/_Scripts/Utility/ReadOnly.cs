
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label) {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

/*
public class Test {
    [ReadOnly]
    public string a;
    [ReadOnly]
    public int b;
    [ReadOnly]
    public Material c;
    [ReadOnly]
    public List<int> d = new List<int>();
}
*/
#endif

public class ReadOnlyAttribute : PropertyAttribute {

}