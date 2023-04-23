using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridUI))]
[CanEditMultipleObjects]
public class GridUIEditor : Editor
{
    private SerializedProperty linePrefab;
    private SerializedProperty numberLineX;
    private SerializedProperty numberLineY;
    private SerializedProperty widthLines;
    private SerializedProperty colorLines;
    private SerializedProperty isVisible;
    private GridUI gridUI;

    private void OnEnable() 
    {
        linePrefab = serializedObject.FindProperty("linePrefab");
        numberLineX = serializedObject.FindProperty("numberLineX");
        numberLineY = serializedObject.FindProperty("numberLineY");
        widthLines = serializedObject.FindProperty("widthLines");
        colorLines = serializedObject.FindProperty("colorLines");
        isVisible = serializedObject.FindProperty("isVisible");

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        gridUI = ((GridUI) target);
    }

    private void OnDisable() 
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            gridUI.ClearAllLines();
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            gridUI.InitializeGrid();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(linePrefab);
        EditorGUILayout.PropertyField(numberLineX);
        EditorGUILayout.PropertyField(numberLineY);
        EditorGUILayout.PropertyField(widthLines);
        EditorGUILayout.PropertyField(colorLines);
        EditorGUILayout.PropertyField(isVisible);

        // Check if a value has changed and update accordingly
        if (!gridUI.GetLinePrefab().Equals((GameObject) linePrefab.objectReferenceValue))
        {
            gridUI.SetLinePrefab((GameObject) linePrefab.objectReferenceValue, true);
        }

        if (!gridUI.GetNumberLineX().Equals(numberLineX.intValue))
        {
            gridUI.SetNumberLineX(numberLineX.intValue, true);
        }

        if (!gridUI.GetNumberLineY().Equals(numberLineY.intValue))
        {
            gridUI.SetNumberLineY(numberLineY.intValue, true);
        }

        if (!gridUI.GetWidthLines().Equals(widthLines.floatValue))
        {
            gridUI.SetWidthLines(widthLines.floatValue, true);
        }

        if (!gridUI.GetColorLines().Equals(colorLines.colorValue))
        {
            gridUI.SetColorLines(colorLines.colorValue, true);
        }

        if (!gridUI.GetGridVisibility().Equals(isVisible.boolValue))
        {
            gridUI.SetGridVisibility(isVisible.boolValue, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
