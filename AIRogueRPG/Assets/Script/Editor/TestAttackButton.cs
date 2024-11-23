using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerStatManager))]
public class TestAttackButton : Editor
{
    public override void OnInspectorGUI() { 
        base.OnInspectorGUI();
        PlayerStatManager statManager = (PlayerStatManager)target; 
        if (GUILayout.Button("Heal")) {
            statManager.TestHealItem();
        } else if(GUILayout.Button("Attack speed")) {
            statManager.TestAttackSpeedItem();
        }
        else if (GUILayout.Button("Movement speed")) {
            statManager.TestMovementSpeedItem();
        }
    }
}
