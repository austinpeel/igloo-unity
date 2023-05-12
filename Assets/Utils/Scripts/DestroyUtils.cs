using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

public class DestroyUtils : MonoBehaviour
{
    public static T SafeDestroy<T>(T obj) where T : Object
    {
        if (Application.isEditor)
        {
            Object.DestroyImmediate(obj);
        }
        else
        {
            Object.Destroy(obj);
        }
     
        return null;
    }
    public static T SafeDestroyGameObject<T>(T component) where T : Component
    {
        if (component != null)
        {
            SafeDestroy(component.gameObject);
        }
        
        return null;
    }

#if UNITY_EDITOR
    public static T SafeDestroyGameObjectNextFrame<T>(T component) where T : Component
    {
        if (component != null)
        {
            EditorCoroutine state = EditorCoroutineUtility.StartCoroutine(IEDestroyNextFrame(component.gameObject), component.gameObject);
        }
        
        return null;
    }

    private static IEnumerator IEDestroyNextFrame(Object obj)
    {
        yield return null;

        SafeDestroy(obj);
    }
#endif

}
