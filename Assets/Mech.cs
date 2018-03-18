using System.Collections.Generic;
using System;

[Serializable]
public class Mech
{
    public string mechType;
    public MovementPoint movementPoint;
    public int tonnage;
    public MechWarrior mechWarrior;

    Dictionary<MechLocation, int> armor;
    public ArmorValues[] armorValues;

    List<Weapon> weapons = new List<Weapon>();
    List<Ammo> ammunitions = new List<Ammo>();

    public bool Destroyed
    {
        get;
        private set;
    }

    public void BuildArmor()
    {
        armor = new Dictionary<MechLocation, int>();
        foreach (ArmorValues a in armorValues)
        {
            MechLocation loc = (MechLocation)Enum.Parse(typeof(MechLocation), a.location);
            armor[loc] = a.armor;
        }
    }

    public void AttachWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    public void LoadAmmo(Ammo ammo)
    {
        ammunitions.Add(ammo);
    }
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

[Serializable]
public class MechData
{
    public List<Mech> mechs = new List<Mech>();
}