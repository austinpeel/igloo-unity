using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SourceParameters))]
public class SourceParametersEditor : Editor
{
    private SourceParameters sourceParameters;

    private void OnEnable() 
    {
        sourceParameters = (SourceParameters) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Apply"))
        {
            sourceParameters.ApplyParameters();
        }
    }
}
