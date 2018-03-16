using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BattleTechSim : MonoBehaviour
{
    [HideInInspector]
    public MechData mechData;
    [HideInInspector]
    public WeaponData weaponData;

    //[Header("TeamA")]


	// Use this for initialization
	void Awake ()
    {
        LoadData();	
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    void LoadData()
    {
        string mechJson = File.ReadAllText(Application.dataPath + "/MechData.txt");
        MechData mechs = JsonUtility.FromJson<MechData>(mechJson);

        string weaponJson = File.ReadAllText(Application.dataPath + "/WeaponData.txt");
        WeaponData weapons = JsonUtility.FromJson<WeaponData>(weaponJson);
    }
}
