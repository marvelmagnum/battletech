using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Weapon
{
    public string type;
    public MechLocation location;
    public int damage;
    public Range range;
}

public struct Range
{
    public int close;
    public int medium;
    public int far;
}