using UnityEditor;

[CustomEditor(typeof(Rotary), true)]
public class EditorRotary : EditorMobile
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Rotary rotary = (Rotary)target;

        EditorGUILayout.LabelField("Rotary Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        rotary.OpenRotation = EditorGUILayout.FloatField("Rotation: ", rotary.OpenRotation);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Rotary Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
