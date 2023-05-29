using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TextureUtils
{
    // Code taken from "https://forum.unity.com/threads/how-to-resize-scale-down-texture-without-losing-quality.976965/"
    public static Texture2D RenderMaterial(ref Material material, Vector2Int resolution, string filename = "")
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(resolution.x, resolution.y);
        Graphics.Blit(null, renderTexture, material);
        
        Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector2.zero, resolution), 0, 0);
        #if UNITY_EDITOR
        // optional, if you want to save it:
        if (filename.Length != 0)
        {
            byte[] png = texture.EncodeToPNG();
            File.WriteAllBytes(filename, png);
            AssetDatabase.Refresh();
        }
        #endif
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        
        return texture;
    }
}
