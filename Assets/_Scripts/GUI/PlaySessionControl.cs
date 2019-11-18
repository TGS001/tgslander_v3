using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySessionControl : MonoBehaviour {
    public static PlaySessionControl session;
    public static ModularLander player;
    public static CameraBehavior cam;
    public enum Mode {
        PLAY,
        PAUSE,
        DEAD,
        DEADSCREEN,
        WIN,
        WINSCREEN
    }
    public RectTransform HUD;

    public static void SetCameraTarget(Transform t) {
        cam.SetTarget(t);
    }

    public static void SetPlayerActive(bool v) {
        if (player)
            player.gameObject.SetActive(v);
    }

    public static bool IsPlayerActive() {
        if (player)
            return player.gameObject.activeInHierarchy;
        return true;
    }

    public static void WarpPlayer(Vector2 position) {
        player.transform.position = position;
        cam.SetTarget(player.transform);
    }

    public RectTransform pauseScreen;

    public static bool CanEvac() {
        if (session) {
            return session.canEvac;
        }
        return true;
    }

    public static void Win(Sprite icon, string reason, string description) {
        if (session) {
            session.mode = Mode.WIN;
            if (session.winIcon)
                session.winIcon.sprite = icon;
            if (session.winReason)
                session.winReason.text = reason;
            if (session.winDescription)
                session.winDescription.text = description;
        }
    }

    public static void Evac(Sprite icon, string reason, string description) {
        if (session && session.mode == Mode.PLAY) {
            if (session.canEvac) {
                Win(icon, reason, description);
            } else {
                Lose(icon, "Left too soon", "You need to complete more of the level");
            }
        }
    }

    public static void AllowEvac(bool s = true) {
        if (session) {
            session.canEvac = s;
        }
    }

    public static bool Invulnerable() {
        return session && !session.canDamage;
    }

    public static void Invulnerable(bool s) {
        if (session) {
            session.canDamage = s;
        }
    }

    public bool canEvac = false;
    public bool canControl = true;
    public bool canDamage = true;
    public RectTransform victoryScreen;
    public RectTransform deathScreen;
    public float deathSlowmo = 0.3f;
    public float deathSlowmoSeconds = 2;
    public float deathPostSlowmoSeconds = 1;
    public float slowmoBlendSeconds = 1;
    public float winSeconds = 5;

    public Image failIcon;
    public Text failReason;
    public Text failDescription;

    public Image winIcon;
    public Text winReason;
    public Text winDescription;

    public ImageFade fader;

    [SerializeField]
    [HideInInspector]
    private Mode _mode = Mode.PLAY;

    IEnumerator slowmoControl = null;
    IEnumerator winDelay = null;

    IEnumerator WinDelay() {
        yield return new WaitForSeconds(winSeconds);
        mode = Mode.WINSCREEN;
    }

    void EndMode(Mode m) {
        switch (m) {
            case Mode.PLAY:
                break;
            case Mode.PAUSE:
                Time.timeScale = 1;
                break;
            case Mode.DEAD:
                if (slowmoControl != null) {
                    StopCoroutine(slowmoControl);
                    slowmoControl = null;
                }
                break;
            case Mode.DEADSCREEN:
                break;
            case Mode.WIN:
                if (winDelay != null) {
                    StopCoroutine(winDelay);
                    winDelay = null;
                }
                break;
            default:
                break;
        }
    }

    void StartMode(Mode m) {
        switch (m) {
            case Mode.PLAY:
                pauseScreen.gameObject.SetActive(false);
                deathScreen.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(false);
                HUD.gameObject.SetActive(true);
                fader.FadeIn();
                break;
            case Mode.PAUSE:
                Time.timeScale = 0;
                pauseScreen.gameObject.SetActive(true);
                deathScreen.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(false);
                HUD.gameObject.SetActive(false);
                fader.FadeIn();
                break;
            case Mode.DEAD:
                pauseScreen.gameObject.SetActive(false);
                deathScreen.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(false);
                HUD.gameObject.SetActive(false);
                slowmoControl = DoDeathSlowmoControl();
                StartCoroutine(slowmoControl);
                fader.FadeIn();
                break;
            case Mode.DEADSCREEN:
                pauseScreen.gameObject.SetActive(false);
                deathScreen.gameObject.SetActive(true);
                victoryScreen.gameObject.SetActive(false);
                HUD.gameObject.SetActive(false);
                fader.FadeOut();
                break;
            case Mode.WIN:
                pauseScreen.gameObject.SetActive(false);
                deathScreen.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(false);
                HUD.gameObject.SetActive(false);
                winDelay = WinDelay();
                StartCoroutine(winDelay);
                fader.FadeIn();
                break;
            case Mode.WINSCREEN:
                pauseScreen.gameObject.SetActive(false);
                deathScreen.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(true);
                HUD.gameObject.SetActive(false);
                fader.FadeIn();
                break;
            default:
                break;
        }
    }

    public Mode mode {
        get {
            return _mode;
        }
        set {
            EndMode(_mode);
            StartMode(value);
            _mode = value;
        }
    }

    public static bool paused {
        get {
            if (session != null) {
                return session.mode == Mode.PAUSE || session.mode == Mode.DEADSCREEN;
            } else {
                return false;
            }
        }
        internal set {
            SetPaused(value);
        }
    }

    public static void SetPaused(bool pause) {
        if (session != null) {
            session.Pause(pause);
        }
    }

    public void Pause(bool pause) {
        if (pause) {
            if (mode == Mode.PLAY) {
                mode = Mode.PAUSE;
            }
        } else {
            if (mode == Mode.PAUSE) {
                mode = Mode.PLAY;
            }
        }
    }

    public void Restart() {
        AsyncLevelLoadController.ReloadLevel();
    }

    public static void ReloadLevel() {
        AsyncLevelLoadController.ReloadLevel();
    }

    public static void TogglePause() {
        if (session != null) {
            SetPaused(session.mode == Mode.PLAY);
        }
    }

    void Start() {
        StartMode(_mode);
        session = this;
    }

    private void OnDestroy() {
        session = null;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    IEnumerator DoDeathSlowmoControl() {
        float t = 1;
        Time.timeScale = deathSlowmo;
        yield return new WaitForSecondsRealtime(deathSlowmoSeconds);
        while (t > 0) {
            t -= Time.unscaledDeltaTime / slowmoBlendSeconds;
            Time.timeScale = Mathf.Lerp(1, deathSlowmo, t);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(deathPostSlowmoSeconds);
        mode = Mode.DEADSCREEN;
    }

    public static void SetPlayerControllable(bool s) {
        if (session) {
            session.canControl = s;
        }
    }

    public static bool PlayerControllable() {
        if (session == null) {
            return false;
        }
        return session.mode == Mode.PLAY && session.canControl;
    }

    public static void Lose(Sprite icon, string reason, string description) {
        if (session != null && session.mode == Mode.PLAY) {
            session.mode = Mode.DEAD;
            session.StartCoroutine(session.DoDeathSlowmoControl());
            if (icon != null && session.failIcon) {
                session.failIcon.sprite = icon;
            }
            if (session.failReason) {
                session.failReason.text = reason;
            }
            if (session.failDescription) {
                session.failDescription.text = description;
            }
        }
        
    }
}
