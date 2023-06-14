using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LensedImagePlane))]
public class LensedImagePlaneEditor : Editor
{
    private LensedImagePlane lensedImagePlane;

    private void OnEnable()
    {
        lensedImagePlane = (LensedImagePlane)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Export Source Image"))
        {
            lensedImagePlane.ExportSourceImage(new Vector2Int(128, 128), "source_image.png");
        }

        if (GUILayout.Button("Export Lensing Image"))
        {
            lensedImagePlane.ExportLensingImage(new Vector2Int(128, 128), "lensing_image.png");
        }
    }
}
