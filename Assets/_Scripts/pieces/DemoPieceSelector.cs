using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoPieceSelector : MonoBehaviour
{
    public LanderPreview preview;

    public HullLabel hull;

    public EngineLabel engine;

    public ShieldLabel shield;

    public StrutLabel strut;

    public ThrusterLabel thruster;

    public WeaponLabel weapon;

    public WingLabel wing;

    private void SetupLander(Scene s, LoadSceneMode m)
    {
        ModularLander lander = FindObjectOfType<ModularLander>();
        if (lander)
        {
            if (hull)
                lander.hull = hull;
            if (engine)
                lander.engine = engine;
            if (shield)
                lander.shield = shield;
            if (strut)
                lander.strut = strut;
            if (thruster)
                lander.thruster = thruster;
            if (weapon)
                lander.weapon = weapon;
            if (wing)
                lander.wing = wing;

            lander.Generate();
        }
    }

    internal bool IsUsing(PartInfo part)
    {
        PieceLabel label = part.pieceLabel;
        if (label is EngineLabel)
        {
            return engine.gameObject.name == part.gameObject.name;
        }
        if (label is ShieldLabel)
        {
            return shield.gameObject.name == part.gameObject.name;
        }
        if (label is HullLabel)
        {
            return hull.gameObject.name == part.gameObject.name;
        }
        if (label is StrutLabel)
        {
            return strut.gameObject.name == part.gameObject.name;
        }
        if (label is ThrusterLabel)
        {
            return thruster.gameObject.name == part.gameObject.name;
        }
        if (label is WeaponLabel)
        {
            return weapon.gameObject.name == part.gameObject.name;
        }
        if (label is WingLabel)
        {
            return wing.gameObject.name == part.gameObject.name;
        }
        return false;
    }

    internal void SetPart(PartInfo part)
    {
        PieceLabel label = part.pieceLabel;
        if (label is EngineLabel)
        {
            engine = (EngineLabel)label;
            Preview();
            return;
        }
        if (label is ShieldLabel)
        {
            shield = (ShieldLabel)label;
            Preview();
            return;
        }
        if (label is HullLabel)
        {
            hull = (HullLabel)label;
            Preview();
            return;
        }
        if (label is StrutLabel)
        {
            strut = (StrutLabel)label;
            Preview();
            return;
        }
        if (label is ThrusterLabel)
        {
            thruster = (ThrusterLabel)label;
            Preview();
            return;
        }
        if (label is WeaponLabel)
        {
            weapon = (WeaponLabel)label;
            Preview();
            return;
        }
        if (label is WingLabel)
        {
            wing = (WingLabel)label;
            Preview();
            return;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SetupLander;
    }

    private void Preview() {
        if (preview) {
            preview.Preview(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        DemoPieceSelector[] ps = FindObjectsOfType<DemoPieceSelector>();
        if (ps.Length > 1)
        {
            foreach (DemoPieceSelector s in ps)
            {
                if (!s.Equals(this))
                {
                    DestroyImmediate(s.gameObject);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SetupLander;
        Preview();
    }
}
