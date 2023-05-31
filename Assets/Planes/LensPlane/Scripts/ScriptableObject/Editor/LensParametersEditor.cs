using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LensParameters))]
public class LensParametersEditor : Editor
{
    private LensParameters lensParameters;

    private void OnEnable() 
    {
        lensParameters = (LensParameters) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Apply"))
        {
            lensParameters.ApplyParameters();
        }
    }
}
