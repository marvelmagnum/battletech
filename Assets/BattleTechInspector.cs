using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(BattleTechSim))]
public class BattleTechInspector : Editor
{
    bool isActive = false;

    int mechIndex = 0;
    string[] mechOptions;// = new string[] { "Cool", "Great", "Awesome" };
    BattleTechSim sim;

    void Start()
    {

        mechOptions = GetMechNames();

        isActive = true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!isActive) return;

        GUILayout.Label("Team A", "ErrorLabel");
        Rect r = EditorGUILayout.BeginHorizontal();
        mechIndex = EditorGUILayout.Popup("Mech:",
             mechIndex, mechOptions, EditorStyles.toolbarDropDown);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add Mech"))
        {
            
        }

        GUILayout.Space(10);
        GUILayout.Label("Team B", "ErrorLabel");
        if (GUILayout.Button("Add Mech"))
        {

        }

    }

    string[] GetMechNames()
    {
        List<string> mechs = new List<string>();

        foreach (Mech mech in sim.mechData.mechs)
            mechs.Add(mech.mechType);
        return mechs.ToArray();
    }
} 