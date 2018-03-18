using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class Weapon
{
    public string type;  
    public int damage;
    public Range range;
    public string categoryValue;

    Category category;
    internal MechLocation Location
    {
        get;
        private set;
    }

    internal bool HasFired
    {
        get;
        set;
    }

    internal void Assemble(string locationCode)
    {
        List<string> mechLocations = Enum.GetNames(typeof(MechLocation)).ToList();
        foreach (string loc in mechLocations)
        {
            char[] result = loc.Where(c => char.IsUpper(c)).ToArray();
            if (new string(result).Equals(locationCode))
            {
                Location = (MechLocation)Enum.Parse(typeof(MechLocation), loc);
                break;
            }
        }

        category = (Category)Enum.Parse(typeof(Category), categoryValue);
        HasFired = false;
    }

    // Returns false if ammo exhausted
    internal bool Shoot(List<Ammo> ammunitions)
    {
        HasFired = true;   // weapon used this round
        switch (category)
        {
            case Category.Energy:
                return true;

            case Category.Projectile:
                foreach (Ammo ammo in ammunitions)
                {
                    if (ammo.ammoType.Equals(type) && ammo.rounds > 0)
                    {
                        ammo.rounds--;
                        BattleTechSim.Instance.Stream(ammo.rounds + " ammo remains for " + ammo.ammoType + ".");
                        return ammo.rounds > 0 ? true : false;
                    }
                }
                return false;

            default:
                return false;
        }
    }

    internal int GetDamage()
    {
        return damage;
    }
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
    public int rounds;
}

[Serializable]
public class WeaponData
{
    public List<Weapon> weapons;
}