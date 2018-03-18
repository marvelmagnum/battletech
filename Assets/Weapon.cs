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

    MechLocation location;

    public void Assemble(string locationCode)
    {
        List<string> mechLocations = Enum.GetNames(typeof(MechLocation)).ToList();
        foreach (string loc in mechLocations)
        {
            char[] result = loc.Where(c => char.IsUpper(c)).ToArray();
            if (new string(result).Equals(locationCode))
            {
                location = (MechLocation)Enum.Parse(typeof(MechLocation), loc);
                break;
            }
        }

        category = (Category)Enum.Parse(typeof(Category), categoryValue);
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
    public int amount;
}

[Serializable]
public class WeaponData
{
    public List<Weapon> weapons;
}