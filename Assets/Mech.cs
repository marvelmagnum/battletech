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
    public ArmorValues[] armorValues;

    Dictionary<MechLocation, int> armor;
    List<Weapon> weapons = new List<Weapon>();
    List<Ammo> ammunitions = new List<Ammo>();

    List<Weapon> disabledWeapons = new List<Weapon>();

    internal bool Destroyed
    {
        get;
        private set;
    }

    internal int Team
    {
        get; set;
    }

    internal void Init()
    {
        BuildArmor();   // set armor values from json data
        Destroyed = false;
    }

    private void BuildArmor()
    {
        armor = new Dictionary<MechLocation, int>();
        foreach (ArmorValues a in armorValues)
        {
            MechLocation loc = (MechLocation)Enum.Parse(typeof(MechLocation), a.location);
            armor[loc] = a.armor;
        }
    }

    internal void AttachWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    internal void LoadAmmo(Ammo ammo)
    {
        ammunitions.Add(ammo);
    }

    internal int GenerateFiringCount()
    {
        return weapons.Count > 0 ? UnityEngine.Random.Range(1, weapons.Count) : 0;
    }

    internal int FireWeapon(Mech target)
    {
        int weaponIndex = UnityEngine.Random.Range(0, weapons.Count);
        while (weapons[weaponIndex].HasFired)
            weaponIndex = UnityEngine.Random.Range(0, weapons.Count);
        
        BattleTechSim.Instance.streamBuffer += mechType + " fires " + weapons[weaponIndex].type + " at " + target.mechType + ". ";
        if (weapons[weaponIndex].Shoot(ammunitions) == false)  // returns false if weapon is now empty
        {
            disabledWeapons.Add(weapons[weaponIndex]);
            weapons.Remove(weapons[weaponIndex]);
            BattleTechSim.Instance.streamBuffer += mechType + " has exhausted " + weapons[weaponIndex].type + " ammo. ";
        }
        return weapons[weaponIndex].GetDamage();    // Return damage done by weapon
    }

    internal void RechargeWeapons()
    {
        foreach (Weapon weapon in weapons)
            weapon.HasFired = false;
    }

    internal void Damage(int damage)
    {
        MechLocation damageLocation = BattleTechTables.Instance.LookupHitLocation();
        AssignDamage(damageLocation, damage);
    }

    private void AssignDamage(MechLocation location, int damage)
    {
        if (Destroyed) return;
         
        if (armor[location] > 0)    // if location has armor
        {
            if (damage < armor[location])
            {
                armor[location] -= damage;
                BattleTechSim.Instance.streamBuffer += mechType + " takes " + damage + " damage to the " + location.ToString() + ". ";
                return;
            }
            else              // extra damage transfers inward
            {
                if (armor[location] > 0)
                {
                    damage -= armor[location];
                    armor[location] = 0;
                    BattleTechSim.Instance.streamBuffer += location.ToString() + " destroyed. ";
                    DestroyWeapons(location);
                }
                // Check if mech destroyed
                if (location == MechLocation.Head || location == MechLocation.CenterTorso)
                {
                    Destroyed = true;
                    BattleTechSim.Instance.streamBuffer += mechType + " is destroyed. ";
                    return;
                }

                if (damage > 0)
                {
                    MechLocation transferLocation = BattleTechTables.Instance.LookupDamageTransferLocation(location);
                    AssignDamage(transferLocation, damage);
                }
            }           
        }
    }

    private void DestroyWeapons(MechLocation location)
    {
        Weapon wp = null;
        bool found = false;

        foreach(Weapon weapon in weapons)
        {
            if (weapon.Location == location)
            {
                wp = weapon;
                found = true;
                break;
            }
        }

        if (found)
        {
            disabledWeapons.Add(wp);
            weapons.Remove(wp);
            BattleTechSim.Instance.streamBuffer += "The " + wp.type + " has been destroyed. ";
        }
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