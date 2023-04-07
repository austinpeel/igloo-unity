using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(AxisUI))]
[CanEditMultipleObjects]
public class AxisUIEditor : Editor
{
    private SerializedProperty linePrefab;
    private SerializedProperty width;
    private SerializedProperty lineColor;
    private SerializedProperty labelAxis;
    private SerializedProperty isAxisX;
    private SerializedProperty drawTickMarks;
    private AxisUI axisUI;
    private LineUI lineUI;

    private void OnEnable() 
    {
        linePrefab = serializedObject.FindProperty("linePrefab");
        width = serializedObject.FindProperty("width");
        lineColor = serializedObject.FindProperty("lineColor");
        labelAxis = serializedObject.FindProperty("labelAxis");
        isAxisX = serializedObject.FindProperty("isAxisX");
        drawTickMarks = serializedObject.FindProperty("drawTickMarks");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(linePrefab);
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(lineColor);
        EditorGUILayout.PropertyField(labelAxis);
        EditorGUILayout.PropertyField(isAxisX);
        EditorGUILayout.PropertyField(drawTickMarks);

        lineUI = (LineUI) target;
        
        if (!lineUI.GetWidth().Equals(width.floatValue))
        {
            lineUI.SetWidth(width.floatValue, true);
        }

        if (!lineUI.GetColor().Equals(lineColor.colorValue))
        {
            lineUI.SetColor(lineColor.colorValue, true);
        }

         axisUI = (AxisUI) target;
        
        if (!axisUI.GetLinePrefab().Equals((GameObject) linePrefab.objectReferenceValue))
        {
            axisUI.SetLinePrefab((GameObject) linePrefab.objectReferenceValue, true);
        }

        if (!axisUI.GetLabelAxis().Equals((Image) labelAxis.objectReferenceValue))
        {
            axisUI.SetLabelAxis((Image) labelAxis.objectReferenceValue, true);
        }

        if (!axisUI.GetIsAxisX().Equals(isAxisX.boolValue))
        {
            axisUI.SetIsAxisX(isAxisX.boolValue, true);
        }

        if (!axisUI.GetDrawTickMarks().Equals(drawTickMarks.boolValue))
        {
            axisUI.SetDrawTickMarks(drawTickMarks.boolValue, true);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
