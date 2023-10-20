using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transporter), true)]
public class EditorTransporter : Editor
{
    public override void OnInspectorGUI()
    {
        Transporter transporter = (Transporter)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Transporter Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Transporter Setup") == true) transporter.Setup();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Position") == true) transporter.PointAdd();
        else if (GUILayout.Button("Remove Position") == true) transporter.PointRemove();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck() == true)
        {
            transporter.Setup();
            EditorUtility.SetDirty(transporter);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Transporter Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
