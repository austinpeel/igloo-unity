using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableEllipseUI))]

public class InteractableEllipseUIEditor : Editor
{
    // EllipseUI part
    private SerializedProperty thickness;
    private SerializedProperty q;
    private SerializedProperty einsteinRadius;
    private SerializedProperty angle;
    private SerializedProperty centerPosition;
    private EllipseUI ellipseUI;

    // LensEllipseUI part
    private SerializedProperty distanceMagnetCenter;
    private SerializedProperty distanceMagnetQ;
    private SerializedProperty distanceMagnetAngle;
    private SerializedProperty qPointParameter;
    private SerializedProperty qPointParameterDisplay;
    private SerializedProperty centerPointParameter;
    private SerializedProperty centerPointParameterDisplay;
    private SerializedProperty einsteinPointParameter;
    private SerializedProperty einsteinPointParameterDisplay;
    private SerializedProperty anglePointParameter;
    private SerializedProperty anglePointParameterDisplay;
    private SerializedProperty semiMajorAxisLine;
    private SerializedProperty anglePointParameterLine;
    private SerializedProperty anglePointParameterLineLength;
    private SerializedProperty axisYRotation;
    private SerializedProperty arcAngleRotation;
    private SerializedProperty ellipseParameters;
    private InteractableEllipseUI lensEllipseUI;

    // LensPlane part
    private PlaneInteractableEllipse plane;
    private void OnEnable() 
    {
        // EllipseUI part
        thickness = serializedObject.FindProperty("thickness");
        q = serializedObject.FindProperty("q");
        einsteinRadius = serializedObject.FindProperty("einsteinRadius");
        angle = serializedObject.FindProperty("angle");
        centerPosition = serializedObject.FindProperty("centerPosition");

        // LensEllipseUI part
        distanceMagnetCenter = serializedObject.FindProperty("distanceMagnetCenter");
        distanceMagnetQ = serializedObject.FindProperty("distanceMagnetQ");
        distanceMagnetAngle = serializedObject.FindProperty("distanceMagnetAngle");
        qPointParameter = serializedObject.FindProperty("qPointParameter");
        qPointParameterDisplay = serializedObject.FindProperty("qPointParameterDisplay");
        centerPointParameter = serializedObject.FindProperty("centerPointParameter");
        centerPointParameterDisplay = serializedObject.FindProperty("centerPointParameterDisplay");
        einsteinPointParameter = serializedObject.FindProperty("einsteinPointParameter");
        einsteinPointParameterDisplay = serializedObject.FindProperty("einsteinPointParameterDisplay");
        anglePointParameter = serializedObject.FindProperty("anglePointParameter");
        anglePointParameterDisplay = serializedObject.FindProperty("anglePointParameterDisplay");
        semiMajorAxisLine = serializedObject.FindProperty("semiMajorAxisLine");
        anglePointParameterLine = serializedObject.FindProperty("anglePointParameterLine");
        anglePointParameterLineLength = serializedObject.FindProperty("anglePointParameterLineLength");
        axisYRotation = serializedObject.FindProperty("axisYRotation");
        arcAngleRotation = serializedObject.FindProperty("arcAngleRotation");
        ellipseParameters = serializedObject.FindProperty("ellipseParameters");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // EllipseUI part
        EditorGUILayout.PropertyField(thickness);
        EditorGUILayout.PropertyField(q);
        EditorGUILayout.PropertyField(einsteinRadius);
        EditorGUILayout.PropertyField(angle);
        EditorGUILayout.PropertyField(centerPosition);

        ellipseUI = (EllipseUI) target;

        // LensEllipseUI part
        EditorGUILayout.PropertyField(distanceMagnetCenter);
        EditorGUILayout.PropertyField(distanceMagnetQ);
        EditorGUILayout.PropertyField(distanceMagnetAngle);
        EditorGUILayout.PropertyField(qPointParameter);
        EditorGUILayout.PropertyField(qPointParameterDisplay);
        EditorGUILayout.PropertyField(centerPointParameter);
        EditorGUILayout.PropertyField(centerPointParameterDisplay);
        EditorGUILayout.PropertyField(einsteinPointParameter);
        EditorGUILayout.PropertyField(einsteinPointParameterDisplay);
        EditorGUILayout.PropertyField(anglePointParameter);
        EditorGUILayout.PropertyField(anglePointParameterDisplay);
        EditorGUILayout.PropertyField(semiMajorAxisLine);
        EditorGUILayout.PropertyField(anglePointParameterLine);
        EditorGUILayout.PropertyField(anglePointParameterLineLength);
        EditorGUILayout.PropertyField(axisYRotation);
        EditorGUILayout.PropertyField(arcAngleRotation);
        EditorGUILayout.PropertyField(ellipseParameters);

        lensEllipseUI = (InteractableEllipseUI) target;
        lensEllipseUI.InitializeCoordinateConverter();

        // Plane part
        plane = lensEllipseUI.GetComponentInParent<PlaneInteractableEllipse>();

        // EllipseUI part
        // Check if a value has changed and update accordingly
        if (!ellipseUI.GetThickness().Equals(thickness.floatValue))
        {
            ellipseUI.SetThickness(thickness.floatValue, true);
        }

        if (!ellipseUI.GetQParameter().Equals(q.floatValue))
        {
            plane.SetEllipseQParameter(q.floatValue);
        }

        if (!ellipseUI.GetEinsteinRadiusParameter().Equals(einsteinRadius.floatValue))
        {
            plane.SetEllipseEinsteinRadiusParameter(einsteinRadius.floatValue);
        }

        if (!ellipseUI.GetAngleParameter().Equals(angle.floatValue))
        {
            plane.SetEllipsePhiAngleParameter(angle.floatValue);
        }

        if (!ellipseUI.GetCenterPositionParameter().Equals(centerPosition.vector2Value))
        {
            plane.SetEllipseCenterPositionParameter(centerPosition.vector2Value);
        }

        // LensEllipseUI part
        if (!lensEllipseUI.GetDistanceMagnetCenter().Equals(distanceMagnetCenter.floatValue))
        {
            lensEllipseUI.SetDistanceMagnetCenter(distanceMagnetCenter.floatValue);
        }

        if (!lensEllipseUI.GetDistanceMagnetQ().Equals(distanceMagnetQ.floatValue))
        {
            lensEllipseUI.SetDistanceMagnetQ(distanceMagnetQ.floatValue);
        }

        if (!lensEllipseUI.GetDistanceMagnetAngle().Equals(distanceMagnetAngle.floatValue))
        {
            lensEllipseUI.SetDistanceMagnetAngle(distanceMagnetAngle.floatValue);
        }

        if (lensEllipseUI.GetQPointParameter() != ((QPointUI) qPointParameter.objectReferenceValue))
        {
            lensEllipseUI.SetQPointParameter((QPointUI) qPointParameter.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetQPointParameterDisplay() != ((ParameterImageValueDisplay) qPointParameterDisplay.objectReferenceValue))
        {
            lensEllipseUI.SetQPointParameterDisplay((ParameterImageValueDisplay) qPointParameterDisplay.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetCenterPointParameter() != ((CenterPointUI) centerPointParameter.objectReferenceValue))
        {
            lensEllipseUI.SetCenterPointParameter((CenterPointUI) centerPointParameter.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetCenterPointParameterDisplay() != ((ParameterImageValueDisplay) centerPointParameterDisplay.objectReferenceValue))
        {
            lensEllipseUI.SetCenterPointParameterDisplay((ParameterImageValueDisplay) centerPointParameterDisplay.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetEinsteinPointParameter() != ((EinsteinPointUI) einsteinPointParameter.objectReferenceValue))
        {
            lensEllipseUI.SetEinsteinPointParameter((EinsteinPointUI) einsteinPointParameter.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetEinsteinPointParameterDisplay() != ((ParameterImageValueDisplay) einsteinPointParameterDisplay.objectReferenceValue))
        {
            lensEllipseUI.SetEinsteinPointParameterDisplay((ParameterImageValueDisplay) einsteinPointParameterDisplay.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetAnglePointParameter() != ((AnglePointUI) anglePointParameter.objectReferenceValue))
        {
            lensEllipseUI.SetAnglePointParameter((AnglePointUI) anglePointParameter.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetAnglePointParameterDisplay() != ((ParameterImageValueDisplay) anglePointParameterDisplay.objectReferenceValue))
        {
            lensEllipseUI.SetAnglePointParameterDisplay((ParameterImageValueDisplay) anglePointParameterDisplay.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetSemiMajorAxisLine() != ((LineUI) semiMajorAxisLine.objectReferenceValue))
        {
            lensEllipseUI.SetSemiMajorAxisLine((LineUI) semiMajorAxisLine.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetAnglePointParameterLine() != ((LineUI) anglePointParameterLine.objectReferenceValue))
        {
            lensEllipseUI.SetAnglePointParameterLine((LineUI) anglePointParameterLine.objectReferenceValue, true);
        }

        if (!lensEllipseUI.GetAnglePointParameterLineLength().Equals(anglePointParameterLineLength.floatValue))
        {
            lensEllipseUI.SetAnglePointParameterLineLength(anglePointParameterLineLength.floatValue, true);
        }

        if (lensEllipseUI.GetAxisYRotation() != ((LineUI) axisYRotation.objectReferenceValue))
        {
            lensEllipseUI.SetAxisYRotation((LineUI) axisYRotation.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetArcAngleRotation() != ((CircularArcUI) arcAngleRotation.objectReferenceValue))
        {
            lensEllipseUI.SetArcAngleRotation((CircularArcUI) arcAngleRotation.objectReferenceValue, true);
        }

        if (lensEllipseUI.GetEllipseParameters() != ((EllipseParameters) ellipseParameters.objectReferenceValue))
        {
            lensEllipseUI.SetEllipseParameters((EllipseParameters) ellipseParameters.objectReferenceValue);
        }

        serializedObject.ApplyModifiedProperties();

        if(GUILayout.Button("Save Default Parameters"))
        {
            lensEllipseUI.SaveParameters();
        }

        if(GUILayout.Button("Reset to Default Parameters"))
        {
            plane.ResetEllipseParameters();
        }
    }
}
