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
        if (GUILayout.Button("Weak Attack")) {
            statManager.TestWeakAttack();
        } else if(GUILayout.Button("Strong Attack")) {
            statManager.TestStrongAttack();
        }
    }
}
