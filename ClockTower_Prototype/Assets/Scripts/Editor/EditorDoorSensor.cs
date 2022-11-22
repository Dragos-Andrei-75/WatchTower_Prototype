using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorSensor))]
public class EditorDoorSensor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DoorSensor sensorDoor = (DoorSensor)target;

        EditorGUILayout.LabelField("Sensor Options", EditorStyles.boldLabel);

        if (GUILayout.Button("Sensor Setup") == true) sensorDoor.SensorSetUp();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Door") == true) if (sensorDoor.Doors.Length < 2) sensorDoor.AddDoor();
        if (GUILayout.Button("Remove Door") == true) if (sensorDoor.Doors.Length > 1) sensorDoor.RemoveDoor();
        EditorGUILayout.EndHorizontal();

        sensorDoor.OpenType = (DoorSensor.OpenTypes)EditorGUILayout.EnumPopup("Open Types", sensorDoor.OpenType);
        sensorDoor.SensorArea = EditorGUILayout.FloatField("Sensor Area: ", sensorDoor.SensorArea);

        if (EditorGUI.EndChangeCheck() == true) sensorDoor.SensorSetUp();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Sensor Inspector", EditorStyles.boldLabel);
        DrawDefaultInspector();
    }
}
