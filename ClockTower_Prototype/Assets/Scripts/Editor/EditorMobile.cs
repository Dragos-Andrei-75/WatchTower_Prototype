using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mobile), true)]
public class EditorMobile : Editor
{
    public override void OnInspectorGUI()
    {
        Mobile mobile = (Mobile)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Mobile Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Mobile Setup") == true) mobile.Setup();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Mechanism") == true) mobile.MechanismAdd();
        else if (GUILayout.Button("Remove Mechanism") == true) mobile.MechanismRemove();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        mobile.TimeToMove = EditorGUILayout.FloatField("Time to move: ", mobile.TimeToMove);

        EditorGUILayout.Space();
    }
}
