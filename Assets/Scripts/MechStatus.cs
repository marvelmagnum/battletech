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
            int gap = Mathf.RoundToInt(maxArmor / 3f);
            int partArmor = mech.GetPartArmor((MechLocation)i);
            if (partArmor == 0)
                mechParts[i].color = new Color32(50,50,50,255);
            else if (partArmor < gap)
                mechParts[i].color = Color.red;
            else if (partArmor < gap * 2)
                mechParts[i].color = new Color32(255,160,0,255);
            else if (partArmor < maxArmor)
                mechParts[i].color = Color.yellow;
            else
                mechParts[i].color = Color.white;
        }
    }
}
