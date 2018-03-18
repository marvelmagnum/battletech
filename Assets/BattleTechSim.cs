using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTechSim : MonoBehaviour
{
    public Text battleStream;
    public float streamDelay;

    const int maxLines = 26;
    const string pageEnd = "--------------------------------------------------------------------------------------------------------------------------------------------------------------------\nContinue...";

    MechData[] teams;
    bool isStreaming;
    bool isWaiting;
    int lineIndex = 26;
    float lastStream;
    List<Mech> turnOrder = new List<Mech>();
    int turnIndex;

    enum State
    {
        Begin,
        Initiative,
        Movement,
        Weapon,
        End,
    }
    State battleState;

	// Use this for initialization
	void Start ()
    {
        isStreaming = false;
        isWaiting = false;

        battleState = State.Begin;
	}
	
	// Update is called once per frame
	void Update ()
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

        while (!IsBattleOver())
        {
            switch (battleState)
            {
                case State.Begin:
                    Debug.Log("battle start");
                    Stream("Battle start.");
                    battleState = State.Initiative;
                    break;

                case State.Initiative:
                    Debug.Log("generate turn order.");
                    GenerateTurnOrder();
                    turnIndex = 0;
                    battleState = State.Movement;
                    break;

                case State.Movement:
                    Debug.Log("Moving mech no. " + turnIndex);
                    Move(turnIndex++);
                    if (turnIndex >= turnOrder.Count)
                        battleState = State.Weapon;
                    break;

                case State.Weapon:
                    break;
            }
        }
        lastStream = Time.time;
    }

    void Move(int i)
    {
        Stream(turnOrder[i].mechType + " moves.");
    }

    int Roll(int numDice = 1)
    {
        return Random.Range(numDice, (numDice * 6) + 1);
    }

    private void GenerateTurnOrder()
    {
        MechData firstTeam, secondTeam;
        DetermineInitiative(out firstTeam, out secondTeam);

        for (int i = 0; i < firstTeam.mechs.Count; ++i)
        {
            turnOrder.Add(firstTeam.mechs[i]);
            if (i < secondTeam.mechs.Count)
                turnOrder.Add(secondTeam.mechs[i]);
        }
    }

    void DetermineInitiative(out MechData firstTeam, out MechData secondTeam)
    {
        int iniA = Roll(2);
        int iniB = Roll(2);
        while (iniA == iniB)
        {
            iniA = Roll(2);
            iniB = Roll(2);
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

    bool IsBattleOver()
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

    internal void StartSimulation(MechData[] t)
    {
        teams = t;
        isStreaming = true;
    }

    void Stream(string text)
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
