using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(AxisUI))]
[CanEditMultipleObjects]
public class AxisUIEditor : Editor
{
    private SerializedProperty width;
    private SerializedProperty lineColor;
    private SerializedProperty labelAxis;
    private SerializedProperty isAxisX;
    private AxisUI axisUI;
    private LineUI lineUI;

    private void OnEnable() 
    {
        width = serializedObject.FindProperty("width");
        lineColor = serializedObject.FindProperty("lineColor");
        labelAxis = serializedObject.FindProperty("labelAxis");
        isAxisX = serializedObject.FindProperty("isAxisX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(lineColor);
        EditorGUILayout.PropertyField(labelAxis);
        EditorGUILayout.PropertyField(isAxisX);

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

        if (!axisUI.GetLabelAxis().Equals((Image) labelAxis.objectReferenceValue))
        {
            axisUI.SetLabelAxis((Image) labelAxis.objectReferenceValue, true);
        }

        if (!axisUI.GetIsAxisX().Equals(isAxisX.boolValue))
        {
            axisUI.SetIsAxisX(isAxisX.boolValue, true);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
