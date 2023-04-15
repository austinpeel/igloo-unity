using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(LensPlane))]
public class LensPlaneEditor : Editor
{
    private SerializedProperty lensEllipseUI;
    private SerializedProperty xCoordinateMax;
    private SerializedProperty yCoordinateMax;
    private SerializedProperty gridUI;
    private SerializedProperty yAxis;
    private SerializedProperty xAxis;
    private SerializedProperty currentModeText;
    private SerializedProperty boundaryX;
    private SerializedProperty boundaryY;

    private SerializedProperty convergenceMap;
    private SerializedProperty displayConvergenceMap;
    private SerializedProperty ellipsesKappaParent;
    private SerializedProperty ellipsePrefab;
    private SerializedProperty displayEllipsesConvergenceMap;

    private LensPlane lensPlane;

    private void OnEnable() 
    {
        lensEllipseUI = serializedObject.FindProperty("lensEllipseUI");
        xCoordinateMax = serializedObject.FindProperty("xCoordinateMax");
        yCoordinateMax = serializedObject.FindProperty("yCoordinateMax");
        gridUI = serializedObject.FindProperty("gridUI");
        yAxis = serializedObject.FindProperty("yAxis");
        xAxis = serializedObject.FindProperty("xAxis");
        currentModeText = serializedObject.FindProperty("currentModeText");
        boundaryX = serializedObject.FindProperty("boundaryX");
        boundaryY = serializedObject.FindProperty("boundaryY");

        // Convergence Kappa Part
        convergenceMap = serializedObject.FindProperty("convergenceMap");
        displayConvergenceMap = serializedObject.FindProperty("displayConvergenceMap");
        ellipsesKappaParent = serializedObject.FindProperty("ellipsesKappaParent");
        ellipsePrefab = serializedObject.FindProperty("ellipsePrefab");
        displayEllipsesConvergenceMap = serializedObject.FindProperty("displayEllipsesConvergenceMap");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(lensEllipseUI);
        EditorGUILayout.PropertyField(xCoordinateMax);
        EditorGUILayout.PropertyField(yCoordinateMax);
        EditorGUILayout.PropertyField(gridUI);
        EditorGUILayout.PropertyField(yAxis);
        EditorGUILayout.PropertyField(xAxis);
        EditorGUILayout.PropertyField(currentModeText);
        EditorGUILayout.PropertyField(boundaryX);
        EditorGUILayout.PropertyField(boundaryY);

        // Convergence Kappa Part
        EditorGUILayout.PropertyField(convergenceMap);
        EditorGUILayout.PropertyField(displayConvergenceMap);
        EditorGUILayout.PropertyField(ellipsesKappaParent);
        EditorGUILayout.PropertyField(ellipsePrefab);
        EditorGUILayout.PropertyField(displayEllipsesConvergenceMap);

        lensPlane = (LensPlane) target;

        if (!lensPlane.GetLensEllipseUI().Equals((EllipseUI) lensEllipseUI.objectReferenceValue))
        {
            lensPlane.SetLensEllipseUI((LensEllipseUI) lensEllipseUI.objectReferenceValue);
        }

        if (!lensPlane.GetXCoordinateMax().Equals(xCoordinateMax.floatValue))
        {
            lensPlane.SetXCoordinateMax(xCoordinateMax.floatValue, true);
        }

        if (!lensPlane.GetYCoordinateMax().Equals(yCoordinateMax.floatValue))
        {
            lensPlane.SetYCoordinateMax(yCoordinateMax.floatValue, true);
        }

        if (!lensPlane.GetGridUI().Equals((GridUI) gridUI.objectReferenceValue))
        {
            lensPlane.SetGridUI((GridUI) gridUI.objectReferenceValue);
        }

        if (!lensPlane.GetYAxis().Equals((AxisUI) yAxis.objectReferenceValue))
        {
            lensPlane.SetYAxis((AxisUI) yAxis.objectReferenceValue);
        }

        if (!lensPlane.GetXAxis().Equals((AxisUI) xAxis.objectReferenceValue))
        {
            lensPlane.SetXAxis((AxisUI) xAxis.objectReferenceValue);
        }

        if (!lensPlane.GetCurrentModeText().Equals((TextMeshProUGUI) currentModeText.objectReferenceValue))
        {
            lensPlane.SetCurrentModeText((TextMeshProUGUI) currentModeText.objectReferenceValue, true);
        }

        if (!lensPlane.GetBoundaryX().Equals(boundaryX.floatValue))
        {
            lensPlane.SetBoundaryX(boundaryX.floatValue);
        }

        if (!lensPlane.GetBoundaryY().Equals(boundaryY.floatValue))
        {
            lensPlane.SetBoundaryY(boundaryY.floatValue);
        }

        if (!lensPlane.GetConvergenceMap().Equals((Image) convergenceMap.objectReferenceValue))
        {
            lensPlane.SetConvergenceMap((Image) convergenceMap.objectReferenceValue, true);
        }

        if (!lensPlane.GetDisplayConvergenceMap().Equals(displayConvergenceMap.boolValue))
        {
            lensPlane.SetDisplayConvergenceMap(displayConvergenceMap.boolValue, true);
        }

        if (!lensPlane.GetEllipseKappaParent().Equals((GameObject) ellipsesKappaParent.objectReferenceValue))
        {
            lensPlane.SetEllipsesKappaParent((GameObject) ellipsesKappaParent.objectReferenceValue, true);
        }

        if (!lensPlane.GetEllipsePrefab().Equals((GameObject) ellipsePrefab.objectReferenceValue))
        {
            lensPlane.SetEllipsePrefab((GameObject) ellipsePrefab.objectReferenceValue, true);
        }

        if (!lensPlane.GetDisplayEllipsesConvergenceMap().Equals(displayEllipsesConvergenceMap.boolValue))
        {
            lensPlane.SetDisplayEllipsesConvergenceMap(displayEllipsesConvergenceMap.boolValue, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
