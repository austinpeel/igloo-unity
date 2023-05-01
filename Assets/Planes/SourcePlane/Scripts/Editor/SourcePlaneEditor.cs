using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(SourcePlane))]
public class SourcePlaneEditor : Editor
{
    private SerializedProperty interactEllipseUI;
    private SerializedProperty xCoordinateMax;
    private SerializedProperty yCoordinateMax;
    private SerializedProperty gridUI;
    private SerializedProperty yAxis;
    private SerializedProperty xAxis;
    private SerializedProperty currentModeText;
    private SerializedProperty boundaryX;
    private SerializedProperty boundaryY;

    private SerializedProperty sliderSersicIndex;
    private SerializedProperty sliderAmplitude;

    private SerializedProperty colorBrightnessMap;
    private SerializedProperty brightnessMap;
    private SerializedProperty displayBrightnessMap;
    private SerializedProperty brightnessColorScale;
    private SerializedProperty colorScaleOutline;
    private SerializedProperty displayBrightnessColorScale;

    private SourcePlane sourcePlane;

    private void OnEnable() 
    {
        interactEllipseUI = serializedObject.FindProperty("interactEllipseUI");
        xCoordinateMax = serializedObject.FindProperty("xCoordinateMax");
        yCoordinateMax = serializedObject.FindProperty("yCoordinateMax");
        gridUI = serializedObject.FindProperty("gridUI");
        yAxis = serializedObject.FindProperty("yAxis");
        xAxis = serializedObject.FindProperty("xAxis");
        currentModeText = serializedObject.FindProperty("currentModeText");
        boundaryX = serializedObject.FindProperty("boundaryX");
        boundaryY = serializedObject.FindProperty("boundaryY");

        // Sliders part
        sliderSersicIndex = serializedObject.FindProperty("sliderSersicIndex");
        sliderAmplitude = serializedObject.FindProperty("sliderAmplitude");

        // Brightness part
        colorBrightnessMap = serializedObject.FindProperty("colorBrightnessMap");
        brightnessMap = serializedObject.FindProperty("brightnessMap");
        displayBrightnessMap = serializedObject.FindProperty("displayBrightnessMap");
        brightnessColorScale = serializedObject.FindProperty("brightnessColorScale");
        colorScaleOutline = serializedObject.FindProperty("colorScaleOutline");
        displayBrightnessColorScale = serializedObject.FindProperty("displayBrightnessColorScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(interactEllipseUI);
        EditorGUILayout.PropertyField(xCoordinateMax);
        EditorGUILayout.PropertyField(yCoordinateMax);
        EditorGUILayout.PropertyField(gridUI);
        EditorGUILayout.PropertyField(yAxis);
        EditorGUILayout.PropertyField(xAxis);
        EditorGUILayout.PropertyField(currentModeText);
        EditorGUILayout.PropertyField(boundaryX);
        EditorGUILayout.PropertyField(boundaryY);

        // Sliders part
        EditorGUILayout.PropertyField(sliderSersicIndex);
        EditorGUILayout.PropertyField(sliderAmplitude);

        // Convergence Kappa Part
        EditorGUILayout.PropertyField(colorBrightnessMap);
        EditorGUILayout.PropertyField(brightnessMap);
        EditorGUILayout.PropertyField(displayBrightnessMap);
        EditorGUILayout.PropertyField(brightnessColorScale);
        EditorGUILayout.PropertyField(colorScaleOutline);
        EditorGUILayout.PropertyField(displayBrightnessColorScale);

        sourcePlane = (SourcePlane) target;

        if (!sourcePlane.GetInteractEllipseUI() != ((InteractableEllipseUI) interactEllipseUI.objectReferenceValue))
        {
            sourcePlane.SetInteractEllipseUI((InteractableEllipseUI) interactEllipseUI.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetXCoordinateMax().Equals(xCoordinateMax.floatValue))
        {
            sourcePlane.SetXCoordinateMax(xCoordinateMax.floatValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetYCoordinateMax().Equals(yCoordinateMax.floatValue))
        {
            sourcePlane.SetYCoordinateMax(yCoordinateMax.floatValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetGridUI() != ((GridUI) gridUI.objectReferenceValue))
        {
            sourcePlane.SetGridUI((GridUI) gridUI.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetYAxis() != ((AxisUI) yAxis.objectReferenceValue))
        {
            sourcePlane.SetYAxis((AxisUI) yAxis.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetXAxis() != ((AxisUI) xAxis.objectReferenceValue))
        {
            sourcePlane.SetXAxis((AxisUI) xAxis.objectReferenceValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetCurrentModeText() != ((TextMeshProUGUI) currentModeText.objectReferenceValue))
        {
            sourcePlane.SetCurrentModeText((TextMeshProUGUI) currentModeText.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetBoundaryX().Equals(boundaryX.floatValue))
        {
            sourcePlane.SetBoundaryX(boundaryX.floatValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetBoundaryY().Equals(boundaryY.floatValue))
        {
            sourcePlane.SetBoundaryY(boundaryY.floatValue);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetSliderAmplitude() != ((SliderCurrentValue) sliderAmplitude.objectReferenceValue))
        {
            sourcePlane.SetSliderAmplitude((SliderCurrentValue) sliderAmplitude.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetSliderSersicIndex() != ((SliderCurrentValue) sliderSersicIndex.objectReferenceValue))
        {
            sourcePlane.SetSliderSersicIndex((SliderCurrentValue) sliderSersicIndex.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (sourcePlane.GetBrightnessMap() != ((Image) brightnessMap.objectReferenceValue))
        {
            sourcePlane.SetBrightnessMap((Image) brightnessMap.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetColorBrightnessMap().Equals(colorBrightnessMap.colorValue))
        {
            sourcePlane.SetColorBrightnessMap(colorBrightnessMap.colorValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetDisplayBrightnessMap().Equals(displayBrightnessMap.boolValue))
        {
            sourcePlane.SetDisplayBrightnessMap(displayBrightnessMap.boolValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (sourcePlane.GetBrightnessColorScale() != ((Image) brightnessColorScale.objectReferenceValue))
        {
            sourcePlane.SetBrightnessColorScale((Image) brightnessColorScale.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (sourcePlane.GetColorScaleOutline() != ((GameObject) colorScaleOutline.objectReferenceValue))
        {
            sourcePlane.SetColorScaleOutline((GameObject) colorScaleOutline.objectReferenceValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        if (!sourcePlane.GetDisplayBrightnessColorScale().Equals(displayBrightnessColorScale.boolValue))
        {
            sourcePlane.SetDisplayBrightnessColorScale(displayBrightnessColorScale.boolValue, true);
            // Mark the object as dirty
            EditorUtility.SetDirty(sourcePlane);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
