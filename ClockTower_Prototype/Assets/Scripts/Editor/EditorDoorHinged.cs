using UnityEditor;

[CustomEditor(typeof(DoorHinged), true)]
public class EditorDoorHinged : EditorDoor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DoorHinged doorHinged = (DoorHinged)target;

        EditorGUILayout.LabelField("Hinged Door Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        doorHinged.OpenRotation = EditorGUILayout.FloatField("Rotation: ", doorHinged.OpenRotation);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Door Hinged Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
