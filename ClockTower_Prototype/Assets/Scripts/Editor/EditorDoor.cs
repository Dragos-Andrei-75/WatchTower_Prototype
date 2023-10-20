using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door), true)]
public class EditorDoor : Editor
{
    public override void OnInspectorGUI()
    {
        Door door = (Door)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Door Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Door Setup") == true) door.Setup();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Mechanism") == true) door.MechanismAdd();
        else if (GUILayout.Button("Remove Mechanism") == true) door.MechanismRemove();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        door.TimeToMove = EditorGUILayout.FloatField("Time to move: ", door.TimeToMove);

        EditorGUILayout.Space();
    }
}
