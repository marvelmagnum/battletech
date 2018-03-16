using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Weapon
{
    public string type;  
    public int damage;
    public Range range;

    public string categoryValue;
    Category category;

    MechLocation location;
}

[Serializable]
public struct Range
{
    public int close;
    public int medium;
    public int far;
}

[Serializable]
public enum Category
{
    Energy,
    Projectile
}

[Serializable]
public class Ammo
{
    public string ammoType;
    public int amount;
}
