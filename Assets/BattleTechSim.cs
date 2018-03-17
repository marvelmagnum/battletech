using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class BattleTechSim : MonoBehaviour
{
    [Header("UI Elements")]
    public Dropdown[] mechDropdowns;
    public Dropdown[] weaponDropdowns;
    public Dropdown[] locationDropdowns;
    public Dropdown[] ammoDropdowns;
    public Text[] ammoCounts;
    public Transform[] buildListContents;
    public Transform[] teamListContents;
    public Text[] teamTonnageValues;
    public GameObject listItemPrefab;

    [HideInInspector]
    public MechData mechData;
    [HideInInspector]
    public WeaponData weaponData;

    const string TypeMech = "[M] ";
    const string TypeWeapon = "[W] ";
    const string TypeAmmo = "[A] ";
    const string PartToken = "#";

    float buildxPos = 5f;
    float buildyPos = -25f;
    float yGap = 15f;

    int itemCountA;
    int itemCountB;

    int mechCountA;
    int mechCountB;

    MechData[] teams = { new MechData(), new MechData() };

    // Use this for initialization
	void Start ()
    {
        LoadData();
        PopulateUI();

        itemCountA = itemCountB = 0;
        mechCountA = mechCountB = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    void LoadData()
    {
        string mechJson = File.ReadAllText(Application.dataPath + "/MechData.txt");
        mechData = JsonUtility.FromJson<MechData>(mechJson);

        string weaponJson = File.ReadAllText(Application.dataPath + "/WeaponData.txt");
        weaponData = JsonUtility.FromJson<WeaponData>(weaponJson);
    }

    void PopulateUI()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach (Mech mech in mechData.mechs)
            options.Add(new Dropdown.OptionData(mech.mechType));
        foreach (Dropdown mechDD in mechDropdowns)
        {
            mechDD.ClearOptions();
            mechDD.AddOptions(options);
        }
        options.Clear();

        foreach (Weapon weapon in weaponData.weapons)
            options.Add(new Dropdown.OptionData(weapon.type));
        foreach (Dropdown weaponDD in weaponDropdowns)
        {
            weaponDD.ClearOptions();
            weaponDD.AddOptions(options);
        }
        options.Clear();

        List<string> mechLocations = Enum.GetNames(typeof(MechLocation)).ToList();
        foreach (string loc in mechLocations)
        {
            char[] result = loc.Where(c => char.IsUpper(c)).ToArray();
            options.Add(new Dropdown.OptionData(new string(result)));
        }
        foreach (Dropdown locDD in locationDropdowns)
        {
            locDD.ClearOptions();
            locDD.AddOptions(options);
        }
        options.Clear();

        foreach (Weapon weapon in weaponData.weapons)
        {
            if (weapon.categoryValue.Equals("Projectile"))
                options.Add(new Dropdown.OptionData(weapon.type));
        }
        foreach (Dropdown ammoDD in ammoDropdowns)
        {
            ammoDD.ClearOptions();
            ammoDD.AddOptions(options);
        }
    }

    public void AddMech(int team)
    {
        GameObject item;
        item = Instantiate(listItemPrefab);
        item.transform.SetParent(buildListContents[team]);
        item.transform.localPosition = new Vector3(buildxPos, buildyPos - ((team == 0 ? itemCountA++ : itemCountB++) * yGap), 0f);
        item.GetComponent<Text>().text = TypeMech + mechDropdowns[team].options[mechDropdowns[team].value].text;
    }

    public void AddWeapon(int team)
    {
        GameObject item;
        item = Instantiate(listItemPrefab);
        item.transform.SetParent(buildListContents[team]);
        item.transform.localPosition = new Vector3(buildxPos, buildyPos - ((team == 0 ? itemCountA++ : itemCountB++) * yGap), 0f);
        item.GetComponent<Text>().text = TypeWeapon + weaponDropdowns[team].options[weaponDropdowns[team].value].text + " (" + locationDropdowns[team].options[locationDropdowns[team].value].text + ")";
    }

    public void AddAmmo(int team)
    {
        if (ammoCounts[team].text == string.Empty) return;      // Ammo amount cannot be 0

        GameObject item;
        item = Instantiate(listItemPrefab);
        item.transform.SetParent(buildListContents[team]);
        item.transform.localPosition = new Vector3(buildxPos, buildyPos - ((team == 0 ? itemCountA++ : itemCountB++) * yGap), 0f);
        item.GetComponent<Text>().text = TypeAmmo + ammoDropdowns[team].options[ammoDropdowns[team].value].text + " (" + ammoCounts[team].text + ")";
    }

    public void BuildMech(int team)
    {
        string selectedMech = string.Empty;
        List<string> selectedWeapons = new List<string>();
        List<string> selectedAmmunitions = new List<string>();

        Text[] items = buildListContents[team].GetComponentsInChildren<Text>();
        foreach(Text buildItem in items)
        {
            if (buildItem.text.Contains(TypeMech))
            {
                if (selectedMech == string.Empty)
                    selectedMech = buildItem.text.Substring(TypeMech.Length);
                else
                {
                    ClearBuildList(team);   // Cannot have more than one mechtype in build list
                    return;
                }
            }

            if (buildItem.text.Contains(TypeWeapon))
            {
                string weaponText = buildItem.text.Substring(TypeWeapon.Length);
                string[] parts = weaponText.Split('(');
                weaponText = parts[0].TrimEnd() + PartToken + parts[1].Substring(0, parts[1].IndexOf(')'));
                selectedWeapons.Add(weaponText);
            }

            if (buildItem.text.Contains(TypeAmmo))
            {
                string ammoText = buildItem.text.Substring(TypeAmmo.Length);
                string[] parts = ammoText.Split('(');
                ammoText = parts[0].TrimEnd() + PartToken + parts[1].Substring(0,parts[1].IndexOf(')'));
                selectedAmmunitions.Add(ammoText);
            }
        }

        ClearBuildList(team);
        ammoCounts[team].GetComponentInParent<InputField>().text = "";

        Mech mech = ConstructMech(selectedMech, selectedWeapons, selectedAmmunitions);
        teams[team].mechs.Add(mech);

        GameObject item;
        item = Instantiate(listItemPrefab);
        item.transform.SetParent(teamListContents[team]);
        item.transform.localPosition = new Vector3(buildxPos, buildyPos - ((team == 0 ? mechCountA++ : mechCountB++) * yGap), 0f);
        item.GetComponent<Text>().text = (team == 0 ? mechCountA : mechCountB).ToString() + ". " + mech.mechType;

        int tonnage = 0;
        foreach (Mech m in teams[team].mechs)
            tonnage += m.tonnage;
        teamTonnageValues[team].text = tonnage.ToString() + " tons";
    }

    Mech ConstructMech(string selectedMech, List<string> selectedWeapons, List<string> selectedAmmunitions)
    {
        // Retrive Chassis
        Mech newMech = GetMech(selectedMech);
        newMech.BuildArmor();
        
        // Attach Weapons
        foreach (string weaponItem in selectedWeapons)
        {
            string[] parts = weaponItem.Split(PartToken.ToCharArray());
            Weapon newWeapon = GetWeapon(parts[0]);
            newWeapon.Assemble(parts[1]);
            newMech.AttachWeapon(newWeapon);
        }

        // Load Ammo
        foreach (string ammoItem in selectedAmmunitions)
        {
            string[] parts = ammoItem.Split(PartToken.ToCharArray());
            Ammo newAmmo = new Ammo();
            newAmmo.ammoType = parts[0];
            newAmmo.amount = Int32.Parse(parts[1]);
            newMech.LoadAmmo(newAmmo);
        }

        return newMech;
    }

    Mech GetMech(string selectedMech)
    {
        Mech ret = new Mech();
        foreach (Mech mech in mechData.mechs)
        {
            if (mech.mechType.Equals(selectedMech))
            {
                ret = mech;
                break;
            }
        }
        return ret;
    }

    private Weapon GetWeapon(string weaponItem)
    {
        Weapon ret = new Weapon();
        foreach (Weapon weapon in weaponData.weapons)
        {
            if (weapon.type.Equals(weaponItem))
            {
                ret = weapon;
                break;
            }
        }
        return ret;
    }

    void ClearBuildList(int team)
    {
        Text[] items = buildListContents[team].GetComponentsInChildren<Text>();
        foreach (Text tr in items)
            Destroy(tr.gameObject);
        if (team == 0)
            itemCountA = 0;
        else
            itemCountB = 0;
    }
}
