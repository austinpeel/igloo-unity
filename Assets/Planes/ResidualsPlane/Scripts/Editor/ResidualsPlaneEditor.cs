using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResidualsPlane))]
public class ResidualsPlaneEditor : Editor 
{
    private ResidualsPlane residualsPlane;

    private void OnEnable() 
    {
        residualsPlane = (ResidualsPlane) target;
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Export Residuals Image"))
        {
            residualsPlane.ExportResidualsImage("residuals_image.png");
        }
    }
}

