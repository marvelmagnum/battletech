using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class JsonWriter : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Mech mech = new Mech();
        mech.mechType = "HER-2S Hermes II";
        mech.movementPoint.walk = 6;
        mech.movementPoint.run = 9;
        mech.tonnage = 40;
        mech.mechWarrior.name = "John Doe";
        mech.mechWarrior.gunnery = 5;
        mech.mechWarrior.piloting = 4;

        mech.armorValues = new ArmorValues[8];
        mech.armorValues[0].location = MechLocation.Head;
        mech.armorValues[0].armor = 9;
        mech.armorValues[1].location = MechLocation.LeftTorso;
        mech.armorValues[1].armor = 14;
        mech.armorValues[2].location = MechLocation.RightTorso;
        mech.armorValues[2].armor = 14;
        mech.armorValues[3].location = MechLocation.CenterTorso;
        mech.armorValues[3].armor = 17;
        mech.armorValues[4].location = MechLocation.LeftArm;
        mech.armorValues[4].armor = 11;
        mech.armorValues[5].location = MechLocation.RightArm;
        mech.armorValues[5].armor = 11;
        mech.armorValues[6].location = MechLocation.LeftLeg;
        mech.armorValues[6].armor = 14;
        mech.armorValues[7].location = MechLocation.RightLeg;
        mech.armorValues[7].armor = 14;

        File.WriteAllText(Application.dataPath + "/MechData.txt", JsonUtility.ToJson(mech, true));
    }

}

