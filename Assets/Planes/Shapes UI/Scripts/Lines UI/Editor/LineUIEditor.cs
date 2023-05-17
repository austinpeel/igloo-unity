using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineUI))]
[CanEditMultipleObjects]
public class LineUIEditor : Editor
{
    private SerializedProperty positionStart;
    private SerializedProperty positionEnd;
    private SerializedProperty width;
    private SerializedProperty lineColor;
    private LineUI lineUI;

    private void OnEnable() 
    {
        positionStart = serializedObject.FindProperty("positionStart");
        positionEnd = serializedObject.FindProperty("positionEnd");
        width = serializedObject.FindProperty("width");
        lineColor = serializedObject.FindProperty("lineColor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(positionStart);
        EditorGUILayout.PropertyField(positionEnd);
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(lineColor);

        lineUI = (LineUI) target;

        // Check if a value has changed and update accordingly
        if (!lineUI.GetPositionStart().Equals(positionStart.vector2Value))
        {
            lineUI.SetPositionStart(positionStart.vector2Value, true);
        }

        if (!lineUI.GetPositionEnd().Equals(positionEnd.vector2Value))
        {
            lineUI.SetPositionEnd(positionEnd.vector2Value, true);
        }

        if (!lineUI.GetWidth().Equals(width.floatValue))
        {
            lineUI.SetWidth(width.floatValue, true);
        }

        if (!lineUI.GetColor().Equals(lineColor.colorValue))
        {
            lineUI.SetColor(lineColor.colorValue, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
