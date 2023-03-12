using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorSensor))]
public class EditorDoorSensor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DoorSensor doorSensor = (DoorSensor)target;

        EditorGUILayout.LabelField("Sensor Options", EditorStyles.boldLabel);

        if (GUILayout.Button("Sensor Setup") == true) doorSensor.SensorSetUp();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Door") == true) if (doorSensor.Doors.Length < 2) doorSensor.AddDoor();
        if (GUILayout.Button("Remove Door") == true) if (doorSensor.Doors.Length > 1) doorSensor.RemoveDoor();
        EditorGUILayout.EndHorizontal();

        doorSensor.OpenType = (DoorSensor.OpenTypes)EditorGUILayout.EnumPopup("Open Types", doorSensor.OpenType);
        doorSensor.SensorArea = EditorGUILayout.FloatField("Sensor Area: ", doorSensor.SensorArea);

        if (EditorGUI.EndChangeCheck() == true) doorSensor.SensorSetUp();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Sensor Inspector", EditorStyles.boldLabel);
        DrawDefaultInspector();
    }
}
