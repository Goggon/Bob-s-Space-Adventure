using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(DefaultAsset))]
public class CustomFiletypeInspector : Editor
{
    Editor _customInspector;

    private void OnEnable()
    {

        CreateEditor();
    }

    void CreateEditor()
    {
        string path = AssetDatabase.GetAssetPath(target);

        Assembly[] AS = AppDomain.CurrentDomain.GetAssemblies();
        Type tEditorType = typeof(Editor);
        foreach (var A in AS)
        {
            try
            {
                Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(tEditorType))
                    {
                        CustomFieltypeEditorAttribute[] tExtensions = 
                            (CustomFieltypeEditorAttribute[])T.GetCustomAttributes(typeof(CustomFieltypeEditorAttribute), true);
                        if (tExtensions.Length > 0)
                        {
                            for (int i = 0; i < tExtensions.Length; i++)
                            {
                                if (HasExtension(path, tExtensions[i]))
                                {
                                    _customInspector = CreateEditor(target, T);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {

            }
        }
    }

    static bool HasExtension(string aPath, CustomFieltypeEditorAttribute aAttr)
    {
        for (int i = 0; i < aAttr.extensions.Length; i++)
        {
            if (aPath.EndsWith(aAttr.extensions[i]))
                return true;
        }
        return false;
    }

    #region Unity events
    public override void OnInspectorGUI()
    {
        if (_customInspector != null)
            _customInspector.OnInspectorGUI();
        else
            base.OnInspectorGUI();
    }
    #endregion
}
