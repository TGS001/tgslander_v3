using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderPreview : MonoBehaviour {
    [ReadOnly]
    public bool expanded;

    void expandNode(ExpansionNode node, GameObject part) {
        GameObject xo = Instantiate(part);
        xo.transform.position = node.transform.position;
        xo.transform.rotation = node.transform.rotation;

        for (int i = 0; i < xo.transform.childCount; i++) {
            Transform t = xo.transform.GetChild(i);
            t.SetParent(node.transform);
        }
        DestroyImmediate(xo);
    }

    void expand(LanderPieceSelector selector) {
        if (!expanded) {
            ExpansionNode[] nodes = GetComponentsInChildren<ExpansionNode>();
            int lastNodeCount = 1;
            int lastFinishedCount;

            while (lastNodeCount > 0) {
                lastNodeCount = 0;
                lastFinishedCount = 0;
                foreach (ExpansionNode node in nodes) {
                    if (!node.Finished()) {
                        switch (node.expansion) {
                            case ExpansionType.Engine:
                                if (selector.engine != null) {
                                    expandNode(node, selector.engine.gameObject);
                                }
                                break;
                            case ExpansionType.Hull:
                                if (selector.hull != null) {
                                    expandNode(node, selector.hull.gameObject);
                                }
                                break;
                            case ExpansionType.Shield:
                                if (selector.shield != null) {
                                    expandNode(node, selector.shield.gameObject);
                                }
                                break;
                            case ExpansionType.Strut:
                                if (selector.strut != null) {
                                    expandNode(node, selector.strut.gameObject);
                                }
                                break;
                            case ExpansionType.Thruster:
                                if (selector.thruster != null) {
                                    expandNode(node, selector.thruster.gameObject);
                                }
                                break;
                            case ExpansionType.Weapon:
                                if (selector.weapon != null) {
                                    expandNode(node, selector.weapon.gameObject);
                                }
                                break;
                            case ExpansionType.Wing:
                                if (selector.wing != null) {
                                    expandNode(node, selector.wing.gameObject);
                                }
                                break;
                        }
                        node.Finish();
                        lastNodeCount++;
                    } else {
                        lastFinishedCount++;
                    }
                }
                nodes = GetComponentsInChildren<ExpansionNode>();
            }
            expanded = true;
        }
    }

    public void Clear() {
        while (transform.childCount > 0) {
            foreach (Transform child in transform) {
                DestroyImmediate(child.gameObject);
            }
        }
        expanded = false;
        ExpansionNode rn = GetComponent<ExpansionNode>();
        rn.Reset();
    }

    public void Generate(LanderPieceSelector selector) {
        Clear();
        Vector3 startScale = transform.localScale;
        transform.localScale = Vector3.one;
        expand(selector);
        transform.localScale = startScale;
    }

    public void Preview(LanderPieceSelector selector) {
        Generate(selector);
    }
}
