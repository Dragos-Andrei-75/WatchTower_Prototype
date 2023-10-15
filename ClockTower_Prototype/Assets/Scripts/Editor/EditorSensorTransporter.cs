using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SensorTransporter), true)]
public class EditorSensorTransporter : EditorSensor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SensorTransporter sensorElevator = (SensorTransporter)target;

        EditorGUILayout.LabelField("Elevator Sensor Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Elevator Sensor Setup") == true) sensorElevator.SetupSensor();

        EditorGUILayout.Space();

        EditorGUILayout.FloatField("Reduction: ", sensorElevator.Reduction);

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck() == true)
        {
            sensorElevator.SetupSensor();
            EditorUtility.SetDirty(sensorElevator);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Sensor Elevator Inspector", EditorStyles.boldLabel);

        DrawDefaultInspector();
    }
}
