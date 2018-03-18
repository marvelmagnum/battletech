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
        End,
    }
    private State battleState;

    // Use this for initialization
    private void Start ()
    {
        isStreaming = false;
        isWaiting = false;
        battleState = State.None;
    }

    // Update is called once per frame
    private void Update ()
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
                Debug.Log("battle starts.");
                Stream("Battle starts.");
                battleState = State.Initiative;
                break;

            // Roll initiative and determine turn order in round
            case State.Initiative:
                Debug.Log("generate turn order.");
                GenerateTurnOrder();
                turnIndex = 0;
                battleState = State.Movement;
                break;

            // Moving
            case State.Movement:
                Debug.Log("moving.");
                battleState = State.FindOpponent;
                break;

            // Select target and choose weapons
            case State.FindOpponent:
                Debug.Log("finding opponent.");
                attacker = turnOrder[turnIndex];
                target = GetOpponent(attacker);
                firingCount = attacker.GenerateFiringCount();   // Determine number of weapons mech will fire
                battleState = firingCount > 0 ? State.Weapon : State.NextMech;  // If no usable weapons, move to next mech in turn. Else use weapons.
                break;

            // Fire weapon
            case State.Weapon:
                Debug.Log("firing.");
                damage = attacker.FireWeapon(target);
                battleState = State.Damage;
                break;

            // Assign damage. 
            case State.Damage:
                Debug.Log("damage.");
                target.Damage(damage);
                battleState = --firingCount > 0 ? State.Weapon : State.NextMech;    // If more weapons to fire, shoot else next mech
                break;

            // next mech in turn order or start next round 
            case State.NextMech:
                Debug.Log("next mech.");
                if (IsBattleOver())
                {
                    battleState = State.End;       // Is battle over?
                    break;
                }

                attacker.RechargeWeapons();     // reset all weapons
                turnIndex++;
                battleState = turnIndex < turnOrder.Count ? State.FindOpponent : State.Initiative;  // if more mech to go this round, then next mech. Else end round.
                break;

            // Battle ends
            case State.End:
                Debug.Log("battle ends.");
                Stream("Battle ends. Team " + (GetWinningTeam() == 0 ? "Alpha" : "Beta") + " wins.");
                break;
        }
    }

    private Mech GetOpponent(Mech thisMech)
    {
        int oppTeam = thisMech.Team ^ 1; // Toggles between 1 and 0.
        return teams[oppTeam].mechs[Random.Range(0, teams[oppTeam].mechs.Count)];
    }

    private void GenerateTurnOrder()
    {
        MechData firstTeam, secondTeam;
        DetermineInitiative(out firstTeam, out secondTeam); // find first and second teams as per turn order

        for (int i = 0; i < firstTeam.mechs.Count; ++i)
        {
            turnOrder.Add(firstTeam.mechs[i]);
            if (i < secondTeam.mechs.Count)
                turnOrder.Add(secondTeam.mechs[i]);
        }
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
        foreach(Mech mech in teams[0].mechs)
        {
            if (!mech.Destroyed)
                return 0;
        }
        return 1;
    }

    internal void StartSimulation(MechData[] t)
    {
        Debug.Log("Start sim.");
        teams = t;
        isStreaming = true;
        battleState = State.Begin;
    }

    internal void Stream(string text)
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
