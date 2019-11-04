using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoPartButton : MonoBehaviour {
    public DemoPartCollection collection;
    public Image background;
    public Color selected = Color.green;
    public Color notSelected = Color.black;
    public Text label;
    public Image icon;
    public PartInfo part;

    [SerializeField]
    [HideInInspector]
    private bool _picked = false;

    private void Apply()
    {
        if (background)
        {
            background.color = _picked ? selected : notSelected;
        }
        if (part)
        {
            if (part.image && icon)
            {
                icon.sprite = part.image;
            }
            if (label)
            {
                label.text = part.label.ToUpper();
            }
        }
    }

    public bool picked
    {
        get
        {
            return _picked;
        }
        set
        {
            _picked = value;
            Apply();
        }
    }

    public void TryPick()
    {
        
        if (picked == false)
        {
            picked = true;
            if (collection)
            {
                collection.Pick(this);
            }
        }
    }

    private void Start()
    {
        if (collection)
        {
            collection.Add(this);
        }
        else
        {
            Apply();
        }
    }
}
