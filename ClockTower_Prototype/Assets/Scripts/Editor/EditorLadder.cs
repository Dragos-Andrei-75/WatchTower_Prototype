using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ladder))]
public class EditorLadder : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        Ladder ladder = (Ladder)target;

        EditorGUILayout.LabelField("Ladder Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Ladder Setup") == true) ladder.LadderSetUp();

        EditorGUILayout.Space();

        EditorGUIUtility.labelWidth = 100;

        EditorGUILayout.BeginHorizontal();
        ladder.LadderHeight = EditorGUILayout.FloatField("Ladder Height: ", ladder.LadderHeight, GUILayout.MaxWidth(250));
        ladder.RungOffset = EditorGUILayout.FloatField("Rung Offset: ", ladder.RungOffset, GUILayout.MaxWidth(250));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset") == true) ladder.LadderReset();

        if (EditorGUI.EndChangeCheck() == true) ladder.LadderCreate();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Ladder Inspector", EditorStyles.boldLabel);
        DrawDefaultInspector();
    }
}
