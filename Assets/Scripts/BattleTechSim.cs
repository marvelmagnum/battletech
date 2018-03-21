using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTechSim : MonoBehaviour
{
    private static BattleTechSim instance = null;
    internal static BattleTechSim Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<BattleTechSim>();
            }
            return instance;
        }
    }

    public Text battleStream;
    public float streamDelay;
    public Transform[] teamrows;
    public GameObject mechPrefab;


    private const int maxLines = 26;
    private const string pageEnd = "--------------------------------------------------------------------------------------------------------------------------------------------------------------------\nContinue...";

    private MechData[] teams;
    private bool isStreaming;
    private bool isWaiting;
    private int lineIndex = 0;
    private float lastStream;
    private List<Mech> turnOrder = new List<Mech>();
    private int turnIndex;

    private Mech attacker;
    private Mech target;
    private int firingCount;
    private int damage;

    internal string streamBuffer;

    private enum State
    {
        None,
        Begin,
        Initiative,
        Movement,
        FindOpponent,
        Weapon,
        Damage,
        NextMech,
        Summary,
        End,
    }
    private State battleState;

    // Use this for initialization
    private void Start()
    {
        isStreaming = false;
        isWaiting = false;
        battleState = State.None;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isStreaming) return;
        if (isWaiting)
        {
            if (!Input.anyKey)
                return;

            battleStream.text = string.Empty;
            lineIndex = 0;
            isWaiting = false;
        }
        if (Time.time - lastStream < streamDelay)
            return;

        switch (battleState)
        {
            // Battle has not started
            case State.None:
                break;

            // Start Battle
            case State.Begin:
                Stream("Battle starts.");
                SetupTeamStatus();  // Setup the mech status hud
                battleState = State.Initiative;
                break;

            // Roll initiative and determine turn order in round
            case State.Initiative:
                GenerateTurnOrder();
                turnIndex = 0;
                battleState = State.Movement;
                break;

            // Moving
            case State.Movement:
                battleState = State.FindOpponent;
                break;

            // Select target and choose weapons
            case State.FindOpponent:
                attacker = turnOrder[turnIndex];
                if (attacker.Destroyed)     // skip to next mech if this one is destroyed.
                {
                    battleState = State.NextMech;
                    break;
                }
                target = GetOpponent(attacker);
                firingCount = attacker.GenerateFiringCount();   // Determine number of weapons mech will fire
                if (firingCount == 0)
                    Stream(attacker.mechType + " has no usable weapons.");
                battleState = firingCount > 0 ? State.Weapon : State.NextMech;  // If no usable weapons, move to next mech in turn. Else use weapons.
                break;

            // Fire weapon
            case State.Weapon:
                if (target.Destroyed)
                {
                    battleState = State.NextMech;
                    break;
                }
                damage = attacker.FireWeapon(target);
                FlushStream();
                battleState = State.Damage;
                break;

            // Assign damage. 
            case State.Damage:
                target.Damage(damage);
                FlushStream();
                battleState = --firingCount > 0 ? State.Weapon : State.NextMech;    // If more weapons to fire, shoot else next mech
                break;

            // next mech in turn order or start next round 
            case State.NextMech:
                if (IsBattleOver())
                {
                    battleState = State.Summary;       // Is battle over?
                    break;
                }

                attacker.RechargeWeapons();     // reset all weapons
                turnIndex++;
                battleState = turnIndex < turnOrder.Count ? State.FindOpponent : State.Initiative;  // if more mech to go this round, then next mech. Else end round.
                break;

            // Battle ends
            case State.Summary:
                Stream("Battle ends. Team " + (GetWinningTeam() == 0 ? "Alpha" : "Beta") + " wins.");
                battleState = State.End;
                break;

            case State.End:
                break;
        }
    }

    private void SetupTeamStatus()
    {
        for (int i = 0; i < teams.Length; ++i)
        {
            for (int j = 0; j < teams[i].mechs.Count; j++)
            {
                GameObject mechItem = GameObject.Instantiate(mechPrefab);
                MechStatus status = mechItem.GetComponent<MechStatus>();
                status.Init(teams[i].mechs[j], i);
                teams[i].mechs[j].AttachStatus(status);
                mechItem.transform.SetParent(teamrows[i]);
                mechItem.transform.localPosition = new Vector3(-120f * j, 0, 0);
            }
        }
    }

    private Mech GetOpponent(Mech thisMech)
    {
        int oppTeam = thisMech.Team ^ 1; // Toggles between 1 and 0.
        Mech opp;
        do
        {
            opp = teams[oppTeam].mechs[UnityEngine.Random.Range(0, teams[oppTeam].mechs.Count)];
        } while (opp.Destroyed);
        return opp;
    }

    private void GenerateTurnOrder()
    {
        MechData firstTeam, secondTeam;
        DetermineInitiative(out firstTeam, out secondTeam); // find first and second teams as per turn order
        turnOrder.Clear();
        int i, j, n;
        i = j = n = 0;
        bool t1 = true;
        while (n < firstTeam.mechs.Count + secondTeam.mechs.Count)
        {
            if (t1)
            {
                if (i < firstTeam.mechs.Count)
                {
                    while (firstTeam.mechs[i].Destroyed)
                    {
                        i++;
                        n++;
                        if (i >= firstTeam.mechs.Count)
                            break;
                    }
                    if (i < firstTeam.mechs.Count)
                    {
                        turnOrder.Add(firstTeam.mechs[i]);
                        i++;
                        n++;
                    }
                }
                t1 = false;
            }
            else
            {
                if (j < secondTeam.mechs.Count)
                {
                    while (secondTeam.mechs[j].Destroyed)
                    {
                        j++;
                        n++;
                        if (j >= secondTeam.mechs.Count)
                            break;
                    }
                    if (j < secondTeam.mechs.Count)
                    {
                        turnOrder.Add(secondTeam.mechs[j]);
                        j++;
                        n++;
                    }
                }
                t1 = true;
            }
        }
        string text = "TurnOrder: ";
        for (int z = 0; z < turnOrder.Count; ++z)
            text += turnOrder[z].mechType + " -> ";
        Stream(text);
    }

    // Determine first and second team as per initiative roll. first team lost initiative roll. second team won.
    private void DetermineInitiative(out MechData firstTeam, out MechData secondTeam)
    {
        int iniA = BattleTechTables.Instance.Roll(2);
        int iniB = BattleTechTables.Instance.Roll(2);
        while (iniA == iniB)
        {
            iniA = BattleTechTables.Instance.Roll(2);
            iniB = BattleTechTables.Instance.Roll(2);
        }
        if (iniA > iniB)
        {
            firstTeam = teams[1];
            secondTeam = teams[0];
        }
        else
        {
            firstTeam = teams[0];
            secondTeam = teams[1];
        }
    }

    // Battle is over if all mechs on any side is destroyed
    private bool IsBattleOver()
    {
        bool over = true;
        foreach (MechData team in teams)
        {
            foreach (Mech mech in team.mechs)
            {
                if (!mech.Destroyed)
                {
                    over = false;
                    break;
                }
            }
            if (over)
                return true;
            else
                over = true;
        }
        return false;
    }

    int GetWinningTeam()
    {
        foreach (Mech mech in teams[0].mechs)
        {
            if (!mech.Destroyed)
                return 0;
        }
        return 1;
    }

    internal void StartSimulation(MechData[] t)
    {
        teams = t;
        isStreaming = true;
        battleState = State.Begin;
    }

    private void FlushStream()
    {
        if (string.IsNullOrEmpty(streamBuffer))
            return;

        Stream(streamBuffer);
        streamBuffer = string.Empty;
    }

    private void Stream(string text)
    {
        battleStream.text += text + "\n";
        lineIndex++;
        lastStream = Time.time;

        if (lineIndex >= maxLines)
        {
            battleStream.text += pageEnd;
            isWaiting = true;
        }
    }
}
