﻿using System.Collections.Generic;
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

    private Dictionary<MechLocation, int> armor = new Dictionary<MechLocation, int>();
    private Dictionary<MechLocation, int> structure = new Dictionary<MechLocation, int>();
    private List<Weapon> weapons = new List<Weapon>();
    private List<Ammo> ammunitions = new List<Ammo>();
    private List<Weapon> disabledWeapons = new List<Weapon>();
    private MechStatus status;

    internal bool Destroyed
    {
        get;
        private set;
    }

    internal float GetMaxArmor(string locationName)
    {
        foreach (ArmorValues aValues in armorValues)
        {
            if (aValues.location.Equals(locationName))
                return aValues.armor;
        }
        return -1;
    }

    internal float GetMaxStructure(string locationName)
    {
        foreach (ArmorValues aValues in armorValues)
        {
            if (aValues.location.Equals(locationName))
                return aValues.structure;
        }
        return -1;
    }

    internal int Team
    {
        get; set;
    }

    internal Mech BuildMech()
    {
        BuildArmorStructure();       // set armor values from json data

        Mech newMech = (Mech)this.MemberwiseClone();
        newMech.mechType = string.Copy(mechType);
        newMech.structure = new Dictionary<MechLocation, int>(structure);
        newMech.armor = new Dictionary<MechLocation, int>(armor);
        
        newMech.Destroyed = false;

        return newMech;
    }

    private void BuildArmorStructure()
    {
        structure.Clear();
        armor.Clear();
        foreach (ArmorValues a in armorValues)
        {
            MechLocation loc = (MechLocation)Enum.Parse(typeof(MechLocation), a.location);
            structure[loc] = a.structure;
            armor[loc] = a.armor;
        }
    }

    internal int GetPartArmor(MechLocation location)
    {
        return armor[location];
    }

    internal int GetPartStructure(MechLocation location)
    {
        return structure[location];
    }

    internal void AttachWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    internal void LoadAmmo(Ammo ammo)
    {
        ammunitions.Add(ammo);
    }

    internal void AttachStatus(MechStatus mStatus)
    {
        status = mStatus;
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
            BattleTechSim.Instance.streamBuffer += weapons[weaponIndex].type + " ammo exhausted. ";
            disabledWeapons.Add(weapons[weaponIndex]);
            weapons.Remove(weapons[weaponIndex]);
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
        status.UpdateDamage(this);
    }

    private void AssignDamage(MechLocation location, int damage)
    {
        if (Destroyed) return;

        if (damage <= armor[location])
        {
            armor[location] -= damage;
            BattleTechSim.Instance.streamBuffer += mechType + " takes " + damage + " damage to the " + location.ToString() + ". ";
            if (armor[location] == 0)
                BattleTechSim.Instance.streamBuffer += location.ToString() + " armor depleted. ";
            return;
        }
        else              // extra damage transfers inward
        {
            if (armor[location] > 0)
            {
                damage -= armor[location];
                armor[location] = 0;
                BattleTechSim.Instance.streamBuffer += location.ToString() + " armor depleted. ";
            }
        }

        if (damage < structure[location])
        {
            structure[location] -= damage;
            BattleTechSim.Instance.streamBuffer += mechType + " takes " + damage + " internal damage to the " + location.ToString() + ". ";
            return;
        }
        else
        {
            if (structure[location] > 0)
            {
                damage -= structure[location];
                structure[location] = 0;
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
    public int structure;
    public int armor;
}

[Serializable]
public class MechData
{
    public List<Mech> mechs = new List<Mech>();
}