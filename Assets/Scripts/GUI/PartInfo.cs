using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartInfo : MonoBehaviour {
    public Sprite image;
    public string label;
    private PieceLabel pl;
    public PieceLabel pieceLabel
    {
        get
        {
            if (pl == null)
            {
                pl = GetComponent<PieceLabel>();
            }
            return pl;
        }
    }

    public bool SameType(PartInfo other)
    {
        if (other == null ||
            other.pieceLabel.GetType() != pieceLabel.GetType())
        {
            return false;
        }
        return true;
    }
}
