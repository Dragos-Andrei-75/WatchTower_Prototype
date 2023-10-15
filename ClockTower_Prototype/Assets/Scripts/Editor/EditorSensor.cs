using UnityEditor;

[CustomEditor(typeof(Sensor), true)]
public class EditorSensor : Editor
{
    public override void OnInspectorGUI()
    {
        Sensor sensor = (Sensor)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Sensor Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        sensor.SensorArea = EditorGUILayout.FloatField("Sensor Area: ", sensor.SensorArea);

        EditorGUILayout.Space();
    }
}
