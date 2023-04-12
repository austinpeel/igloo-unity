using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EllipseUI))]
public class EllipseUIEditor : Editor
{
    private SerializedProperty thickness;
    private SerializedProperty q;
    private SerializedProperty einsteinRadius;
    private SerializedProperty angle;
    private SerializedProperty centerPosition;
    private EllipseUI ellipseUI;

    private void OnEnable() 
    {
        thickness = serializedObject.FindProperty("thickness");
        q = serializedObject.FindProperty("q");
        einsteinRadius = serializedObject.FindProperty("einsteinRadius");
        angle = serializedObject.FindProperty("angle");
        centerPosition = serializedObject.FindProperty("centerPosition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(thickness);
        EditorGUILayout.PropertyField(q);
        EditorGUILayout.PropertyField(einsteinRadius);
        EditorGUILayout.PropertyField(angle);
        EditorGUILayout.PropertyField(centerPosition);

        ellipseUI = (EllipseUI) target;

        // Check if a value has changed and update accordingly
        if (!ellipseUI.GetThickness().Equals(thickness.floatValue))
        {
            ellipseUI.SetThickness(thickness.floatValue, true);
        }

        if (!ellipseUI.GetQParameter().Equals(q.floatValue))
        {
            ellipseUI.SetQ(q.floatValue, true);
        }

        if (!ellipseUI.GetEinsteinRadiusParameter().Equals(einsteinRadius.floatValue))
        {
            ellipseUI.SetEinsteinRadius(einsteinRadius.floatValue, true);
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
