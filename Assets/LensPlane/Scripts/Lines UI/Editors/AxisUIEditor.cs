using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(AxisUI))]
[CanEditMultipleObjects]
public class AxisUIEditor : LineUIEditor
{
    private SerializedProperty labelAxis;
    private SerializedProperty isAxisX;
    private AxisUI axisUI;

    private void OnEnable() 
    {
        labelAxis = serializedObject.FindProperty("labelAxis");
        isAxisX = serializedObject.FindProperty("isAxisX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(labelAxis);
        EditorGUILayout.PropertyField(isAxisX);

        axisUI = (AxisUI) target;

        // Check if a value has changed and update accordingly
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
