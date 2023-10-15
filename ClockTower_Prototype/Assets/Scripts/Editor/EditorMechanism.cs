using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mechanism), true)]
public class EditorMechanism : Editor
{
    public override void OnInspectorGUI()
    {
        Mechanism mechanism = (Mechanism)target;
        Mechanism.Arrangements arrangement;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Mechanism Editor", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Mechanism Setup") == true) mechanism.SetupMechanism();

        EditorGUILayout.Space();

        if (GUILayout.Button("Allign") == true) mechanism.Allign();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Mobile") == true) mechanism.MobileAdd();
        else if (GUILayout.Button("Remove Mobile") == true) mechanism.MobileRemove();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Link Mobile") == true) mechanism.Link();
        else if (GUILayout.Button("Unlink Mobile") == true) mechanism.Unlink();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Switch") == true) mechanism.SwitchAdd();
        else if (GUILayout.Button("Remove Switch") == true) mechanism.SwitchRemove();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Sensor") == true) mechanism.SensorAdd();
        else if (GUILayout.Button("Remove Sensor") == true) mechanism.SensorRemove();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        arrangement = (Mechanism.Arrangements)EditorGUILayout.EnumPopup("Arrangement: ", mechanism.Arrangement);
        if (mechanism.Arrangement != arrangement) mechanism.Arrangement = arrangement;

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck() == true)
        {
            EditorUtility.SetDirty(mechanism);
            mechanism.SetupMechanism();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Mechanism Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
