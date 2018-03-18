using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleTechTables
{
    private static BattleTechTables instance = null;
    internal static BattleTechTables Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BattleTechTables();
            }
            return instance;
        }
    }

    Dictionary<int, MechLocation> hitLocationTable;
    Dictionary<MechLocation, MechLocation> damageTransferTable;

    private BattleTechTables()
    {
        Init();
    }

    private void Init()
    {
        InitHitLocationTable();
        InitDamageTransferTable();
    }

    private void InitHitLocationTable()
    {
        hitLocationTable = new Dictionary<int, MechLocation>();
        hitLocationTable.Add(2, MechLocation.CenterTorso);
        hitLocationTable.Add(3, MechLocation.RightArm);
        hitLocationTable.Add(4, MechLocation.RightArm);
        hitLocationTable.Add(5, MechLocation.RightLeg);
        hitLocationTable.Add(6, MechLocation.RightTorso);
        hitLocationTable.Add(7, MechLocation.CenterTorso);
        hitLocationTable.Add(8, MechLocation.LeftTorso);
        hitLocationTable.Add(9, MechLocation.RightLeg);
        hitLocationTable.Add(10, MechLocation.LeftArm);
        hitLocationTable.Add(11, MechLocation.LeftArm);
        hitLocationTable.Add(12, MechLocation.Head);
    }

    private void InitDamageTransferTable()
    {
        damageTransferTable = new Dictionary<MechLocation, MechLocation>();
        damageTransferTable.Add(MechLocation.LeftArm, MechLocation.LeftTorso);
        damageTransferTable.Add(MechLocation.RightArm, MechLocation.RightTorso);
        damageTransferTable.Add(MechLocation.LeftTorso, MechLocation.CenterTorso);
        damageTransferTable.Add(MechLocation.RightTorso, MechLocation.CenterTorso);
        damageTransferTable.Add(MechLocation.LeftLeg, MechLocation.LeftTorso);
        damageTransferTable.Add(MechLocation.RightLeg, MechLocation.RightTorso);
    }

    internal MechLocation LookupHitLocation()
    {
        return hitLocationTable[Roll(2)];
    }

    internal int Roll(int numDice = 1)
    {
        return UnityEngine.Random.Range(numDice, (numDice * 6) + 1);
    }

    internal MechLocation LookupDamageTransferLocation(MechLocation location)
    {
        return damageTransferTable[location];
    }
}
