using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SensorMobile), true)]
public class EditorSensorMobile : EditorSensor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SensorMobile sensorMobile = (SensorMobile)target;

        EditorGUILayout.LabelField("Mobile Sensor Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Mobile Sensor Setup") == true) sensorMobile.SetupSensor();

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck() == true)
        {
            sensorMobile.SetupSensor();
            EditorUtility.SetDirty(sensorMobile);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Sensor Mobile Inspector", EditorStyles.boldLabel);

        DrawDefaultInspector();
    }
}
