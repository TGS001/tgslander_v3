using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : MonoBehaviour {
    public Slider scoreProgress;
    public Slider failProgress;
    public Text scoreDisplay;

    public Image[] medals;
    public Image[] medalPip;
    public float[] medalThresholds;

    private int _score = 0;
    private int maxScore = 0;
    private int fail = 0;
    [SerializeField][HideInInspector]
    Image[] pips;

    public int score {
        get {
            return _score - fail;
        }
    }

    public float scorePercent {
        get {
            return ((float)score) / maxScore;
        }
    }

    public bool levelComplete {
        get {
            if (medals.Length > 0) {
                return scorePercent > medalThresholds[0];
            }
            return true;
        }
    }

    void Refresh() {
        scoreProgress.value = (float)_score / maxScore;
        failProgress.value = (float)fail / maxScore;
        if (scoreDisplay != null && !scoreDisplay.IsDestroyed()) {
            scoreDisplay.text = score.ToString() + "/" + maxScore.ToString();
        }
    }

    public void ChangeScore(int amount) {
        _score += amount;
        Refresh();
    }

    public void ChangeFail(int amount) {
        fail += amount;
        Refresh();
    }

    // Use this for initialization
    void Start() {
        ObjectiveMarker[] markers = FindObjectsOfType<ObjectiveMarker>();
        foreach (ObjectiveMarker marker in markers) {
            marker.owner = this;
            int value;
            bool failOnly;
            marker.Register(out value, out failOnly);
            if (!failOnly) {
                maxScore += value;
            }
        }

        if (pips != null) {
            foreach (Image p in pips) {
                Destroy(p);
            }
        }
        pips = new Image[medalPip.Length];

        for (int i = 0; i < medalPip.Length; i++) {
            pips[i] = Instantiate(medalPip[i], scoreProgress.transform);
            RectTransform t = pips[i].rectTransform;
            t.anchorMax = new Vector2(medalThresholds[i], 1);
            t.anchorMin = t.anchorMax;
        }
        Refresh();
    }
}
