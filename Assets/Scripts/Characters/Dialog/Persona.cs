using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPersona", menuName = "persona")]
public class Persona : ScriptableObject {
    public string displayName;
    public Sprite talk;
    public Sprite silent;
    public SFX indicator;
}
