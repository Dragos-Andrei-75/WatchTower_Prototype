using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door), true)]
public class EditorDoor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        Door door = (Door)target;

        EditorGUILayout.LabelField("Door Options", EditorStyles.boldLabel);

        if (GUILayout.Button("Door Setup") == true) door.Setup();

        door.TimeToMove = EditorGUILayout.FloatField("Time to move: ", door.TimeToMove);

        if (door.DoorType == Door.DoorTypes.HingedDoor)
        {
            HingedDoor hingedDoor = door.GetComponent<HingedDoor>();

            hingedDoor.OpenRotation = EditorGUILayout.FloatField("Open Rotation: ", hingedDoor.OpenRotation);
        }
        else if (door.DoorType == Door.DoorTypes.SlidingDoor)
        {
            SlidingDoor slidingDoor = door.GetComponent<SlidingDoor>();

            slidingDoor.OpenDistance = EditorGUILayout.FloatField("Open Distance: ", slidingDoor.OpenDistance);
            slidingDoor.OpenDirection = (SlidingDoor.OpenDirections)EditorGUILayout.EnumPopup("Open Direction: ", slidingDoor.OpenDirection);
        }

        if (EditorGUI.EndChangeCheck() == true)
        {
            door.Setup();
            EditorUtility.SetDirty(door);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Door Inspector", EditorStyles.boldLabel);

        DrawDefaultInspector();
    }
}
