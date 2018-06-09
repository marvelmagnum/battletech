using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechStatus : MonoBehaviour
{
    public Text mechLabel;
    public Image[] mechParts;   // Ordered as per Mech::MechLocation

    internal void Init(Mech mech, int team)
    {
        mechLabel.text = mech.mechType.Split(' ')[1];
        mechLabel.color = team == 0 ? Color.red : Color.yellow;
    }

    internal void UpdateDamage(Mech mech)
    {
        for (int i = 0; i < mechParts.Length; ++i)
        {
            float maxArmor = mech.GetMaxArmor(((MechLocation)i).ToString());
            int armorGap = Mathf.RoundToInt(maxArmor / 2f);
            int partArmor = mech.GetPartArmor((MechLocation)i);
            if (partArmor == 0)
            {
                float maxStructure = mech.GetMaxStructure(((MechLocation)i).ToString());
                int structureGap = Mathf.RoundToInt(maxStructure / 3f);
                int partStructure = mech.GetPartStructure((MechLocation)i);
                if (partStructure == 0)
                    mechParts[i].color = new Color32(25,25,25,255);
                else if (partStructure < structureGap)
                    mechParts[i].color = Color.red;
                else if (partStructure < structureGap * 2)
                    mechParts[i].color = new Color32(255,160,0,255);
                else if (partStructure < maxStructure)
                    mechParts[i].color = Color.yellow;
                else
                    mechParts[i].color = new Color32(105,105,105,255);
            }
            else if (partArmor < armorGap)
                mechParts[i].color = new Color32(105,105,105,255);
            else if (partArmor < maxArmor)
                mechParts[i].color = new Color32(180, 180, 180, 255);
            else
                mechParts[i].color = Color.white;
        }
    }
}
