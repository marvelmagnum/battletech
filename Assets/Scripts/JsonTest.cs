using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class JsonTest : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        //Mech mech = new Mech();
        //mech.mechType = "HER-2S Hermes II";
        //mech.movementPoint.walk = 6;
        //mech.movementPoint.run = 9;
        //mech.tonnage = 40;
        //mech.mechWarrior.name = "John Doe";
        //mech.mechWarrior.gunnery = 5;
        //mech.mechWarrior.piloting = 4;

        //mech.armorValues = new ArmorValues[8];
        //mech.armorValues[0].location = MechLocation.Head.ToString();
        //mech.armorValues[0].armor = 9;
        //mech.armorValues[1].location = MechLocation.LeftTorso.ToString();
        //mech.armorValues[1].armor = 14;
        //mech.armorValues[2].location = MechLocation.RightTorso.ToString();
        //mech.armorValues[2].armor = 14;
        //mech.armorValues[3].location = MechLocation.CenterTorso.ToString();
        //mech.armorValues[3].armor = 17;
        //mech.armorValues[4].location = MechLocation.LeftArm.ToString();
        //mech.armorValues[4].armor = 11;
        //mech.armorValues[5].location = MechLocation.RightArm.ToString();
        //mech.armorValues[5].armor = 11;
        //mech.armorValues[6].location = MechLocation.LeftLeg.ToString();
        //mech.armorValues[6].armor = 14;
        //mech.armorValues[7].location = MechLocation.RightLeg.ToString();
        //mech.armorValues[7].armor = 14;

        //MechData mechdata = new MechData();
        //mechdata.mechs = new List<Mech>();
        //mechdata.mechs.Add(mech);
        //mechdata.mechs.Add(mech);
        //File.WriteAllText(Application.dataPath + "/MechData.txt", JsonUtility.ToJson(mechdata, true));

        //Weapon weapon = new Weapon();
        //weapon.type = "Autocannon 5";
        //weapon.damage = 5;
        //weapon.range.close = 6;
        //weapon.range.medium = 12;
        //weapon.range.far = 18;
        //weapon.categoryValue = Category.Projectile.ToString();

        //WeaponData weapondata = new WeaponData();
        //weapondata.weapons = new List<Weapon>();
        //weapondata.weapons.Add(weapon);
        //weapondata.weapons.Add(weapon);
        //File.WriteAllText(Application.dataPath + "/WeaponData.txt", JsonUtility.ToJson(weapondata, true));

        string mechJson = File.ReadAllText(Application.dataPath + "/MechData.txt");
        MechData mechs = JsonUtility.FromJson<MechData>(mechJson);

        string weaponJson = File.ReadAllText(Application.dataPath + "/WeaponData.txt");
        WeaponData weapons = JsonUtility.FromJson<WeaponData>(weaponJson);

        Debug.Log("Mechs loaded: " + mechs.mechs.Count);
        Debug.Log("Weapons loaded: " + weapons.weapons.Count);
    }

}

