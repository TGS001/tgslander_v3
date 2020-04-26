using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringSerializableEnum<T> where T : struct, System.IConvertible
{
    public T Value
    {
        get { return m_EnumValue; }
        set {
            m_EnumValue = value;
//            m_EnumValueAsString = 
        }
    }

    public string AsString ()
    {
        return m_EnumValueAsString;
    }

    [SerializeField]
    private string m_EnumValueAsString;
    [SerializeField]
    private T m_EnumValue;
}
