using UnityEditor;

[CustomEditor(typeof(DoorSliding), true)]
public class EditorDoorSliding : EditorDoor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DoorSliding doorSliding = (DoorSliding)target;

        EditorGUILayout.LabelField("Sliding Door Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        doorSliding.Distance = EditorGUILayout.FloatField("Distance: ", doorSliding.Distance);
        doorSliding.Direction = (DoorSliding.Directions)EditorGUILayout.EnumPopup("Direction: ", doorSliding.Direction);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Door Sliding Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
