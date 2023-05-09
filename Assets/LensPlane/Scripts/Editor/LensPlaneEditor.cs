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
    private SerializedProperty convergenceColorScale;
    private SerializedProperty colorScaleOutline;
    private SerializedProperty displayConvergenceColorScale;
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
        convergenceColorScale = serializedObject.FindProperty("convergenceColorScale");
        colorScaleOutline = serializedObject.FindProperty("colorScaleOutline");
        displayConvergenceColorScale = serializedObject.FindProperty("displayConvergenceColorScale");
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
        EditorGUILayout.PropertyField(convergenceColorScale);
        EditorGUILayout.PropertyField(colorScaleOutline);
        EditorGUILayout.PropertyField(displayConvergenceColorScale);
        EditorGUILayout.PropertyField(ellipsesKappaParent);
        EditorGUILayout.PropertyField(ellipsePrefab);
        EditorGUILayout.PropertyField(displayEllipsesConvergenceMap);

        lensPlane = (LensPlane) target;

        if (!lensPlane.GetLensEllipseUI() != ((EllipseUI) lensEllipseUI.objectReferenceValue))
        {
            lensPlane.SetLensEllipseUI((LensEllipseUI) lensEllipseUI.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetXCoordinateMax().Equals(xCoordinateMax.floatValue))
        {
            lensPlane.SetXCoordinateMax(xCoordinateMax.floatValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetYCoordinateMax().Equals(yCoordinateMax.floatValue))
        {
            lensPlane.SetYCoordinateMax(yCoordinateMax.floatValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetGridUI() != ((GridUI) gridUI.objectReferenceValue))
        {
            lensPlane.SetGridUI((GridUI) gridUI.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetYAxis() != ((AxisUI) yAxis.objectReferenceValue))
        {
            lensPlane.SetYAxis((AxisUI) yAxis.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetXAxis() != ((AxisUI) xAxis.objectReferenceValue))
        {
            lensPlane.SetXAxis((AxisUI) xAxis.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetCurrentModeText() != ((TextMeshProUGUI) currentModeText.objectReferenceValue))
        {
            lensPlane.SetCurrentModeText((TextMeshProUGUI) currentModeText.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetBoundaryX().Equals(boundaryX.floatValue))
        {
            lensPlane.SetBoundaryX(boundaryX.floatValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetBoundaryY().Equals(boundaryY.floatValue))
        {
            lensPlane.SetBoundaryY(boundaryY.floatValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (lensPlane.GetConvergenceMap() != ((Image) convergenceMap.objectReferenceValue))
        {
            lensPlane.SetConvergenceMap((Image) convergenceMap.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetDisplayConvergenceMap().Equals(displayConvergenceMap.boolValue))
        {
            lensPlane.SetDisplayConvergenceMap(displayConvergenceMap.boolValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (lensPlane.GetConvergenceColorScale() != ((Image) convergenceColorScale.objectReferenceValue))
        {
            lensPlane.SetConvergenceColorScale((Image) convergenceColorScale.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (lensPlane.GetColorScaleOutline() != ((GameObject) colorScaleOutline.objectReferenceValue))
        {
            lensPlane.SetColorScaleOutline((GameObject) colorScaleOutline.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetDisplayConvergenceColorScale().Equals(displayConvergenceColorScale.boolValue))
        {
            lensPlane.SetDisplayConvergenceColorScale(displayConvergenceColorScale.boolValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (lensPlane.GetEllipseKappaParent() != ((GameObject) ellipsesKappaParent.objectReferenceValue))
        {
            lensPlane.SetEllipsesKappaParent((GameObject) ellipsesKappaParent.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (lensPlane.GetEllipsePrefab() != ((GameObject) ellipsePrefab.objectReferenceValue))
        {
            lensPlane.SetEllipsePrefab((GameObject) ellipsePrefab.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        if (!lensPlane.GetDisplayEllipsesConvergenceMap().Equals(displayEllipsesConvergenceMap.boolValue))
        {
            lensPlane.SetDisplayEllipsesConvergenceMap(displayEllipsesConvergenceMap.boolValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(lensPlane);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
