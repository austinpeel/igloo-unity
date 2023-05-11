using UnityEditor;

[CustomEditor(typeof(EllipseUI))]
public class EllipseUIEditor : Editor
{
    private SerializedProperty ellipseColor;
    private SerializedProperty thickness;
    private SerializedProperty q;
    private SerializedProperty radius;
    private SerializedProperty angle;
    private SerializedProperty centerPosition;
    private EllipseUI ellipseUI;

    private void OnEnable() 
    {
        ellipseColor = serializedObject.FindProperty("ellipseColor");
        thickness = serializedObject.FindProperty("thickness");
        q = serializedObject.FindProperty("q");
        radius = serializedObject.FindProperty("radius");
        angle = serializedObject.FindProperty("angle");
        centerPosition = serializedObject.FindProperty("centerPosition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(ellipseColor);
        EditorGUILayout.PropertyField(thickness);
        EditorGUILayout.PropertyField(q);
        EditorGUILayout.PropertyField(radius);
        EditorGUILayout.PropertyField(angle);
        EditorGUILayout.PropertyField(centerPosition);

        ellipseUI = (EllipseUI) target;

        // Check if a value has changed and update accordingly
        if (!ellipseUI.GetEllipseColor().Equals(ellipseColor.colorValue))
        {
            ellipseUI.SetEllipseColor(ellipseColor.colorValue, true);
        }

        if (!ellipseUI.GetThickness().Equals(thickness.floatValue))
        {
            ellipseUI.SetThickness(thickness.floatValue, true);
        }

        if (!ellipseUI.GetQParameter().Equals(q.floatValue))
        {
            ellipseUI.SetQ(q.floatValue, true);
        }

        if (!ellipseUI.GetRadiusParameter().Equals(radius.floatValue))
        {
            ellipseUI.SetRadius(radius.floatValue, true);
        }

        if (!ellipseUI.GetAngleParameter().Equals(angle.floatValue))
        {
            ellipseUI.SetAngle(angle.floatValue, true);
        }

        if (!ellipseUI.GetCenterPositionParameter().Equals(centerPosition.vector2Value))
        {
            ellipseUI.SetCenterPosition(centerPosition.vector2Value, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
