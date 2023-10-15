using UnityEditor;

[CustomEditor(typeof(Linear), true)]
public class EditorLinear : EditorMobile
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Linear linear = (Linear)target;

        EditorGUILayout.LabelField("Linear Options", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        linear.Distance = EditorGUILayout.FloatField("Distance: ", linear.Distance);
        linear.Direction = (Linear.Directions)EditorGUILayout.EnumPopup("Direction: ", linear.Direction);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Original Linear Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
