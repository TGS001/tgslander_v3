using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPartCollection : MonoBehaviour
{
    public LanderPieceSelector selector;
    List<DemoPartButton> buttons = new List<DemoPartButton>();
    internal void Add(DemoPartButton demoPartButton)
    {
        buttons.Add(demoPartButton);
        if (selector)
        {
            demoPartButton.picked = selector.IsUsing(demoPartButton.part);
        }
        else
        {
            demoPartButton.picked = false;
        }
    }

    internal void Pick(DemoPartButton demoPartButton)
    {
        selector.SetPart(demoPartButton.part);
        foreach (DemoPartButton b in buttons)
        {
            if (b != demoPartButton && b.part && b.part.SameType(demoPartButton.part))
            {
                b.picked = false;
            }
        }
    }
}
