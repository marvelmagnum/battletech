using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Mech
{
    public string mechType;
    public MovementPoint movementPoint;
    public int tonnage;
    public MechWarrior mechWarrior;

    Dictionary<MechLocation, int> armor;
    public ArmorValues [] armorValues;

    List<Weapon> weapons;
}

[Serializable]
public enum MechLocation
{
    Head,
    LeftTorso,
    CenterTorso,
    RightTorso,
    LeftArm,
    RightArm,
    LeftLeg,
    RightLeg
}

[Serializable]
public struct MovementPoint
{
    public int walk;
    public int run;
}

[Serializable]
public struct MechWarrior
{
    public string name;
    public int gunnery;
    public int piloting;
}

[Serializable]
public struct ArmorValues
{
    public string location;
    public int armor;
}