﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartCollection : MonoBehaviour
{
    public LanderPieceSelector selector;
    List<UIPartButton> buttons = new List<UIPartButton>();
    internal void Add(UIPartButton demoPartButton)
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

    internal void Pick(UIPartButton demoPartButton)
    {
        selector.SetPart(demoPartButton.part);
        foreach (UIPartButton b in buttons)
        {
            if (b != demoPartButton && b.part && b.part.SameType(demoPartButton.part))
            {
                b.picked = false;
            }
        }
    }
}
