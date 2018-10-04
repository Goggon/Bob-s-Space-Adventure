using UnityEditor;
using SimpleJSON;
using UnityEngine;

public class FiletypeEditor : Editor
{
    const float SAVE_INTERVAL = 3f;
    AssetImporter _importer;
    bool _save;
    float _lastSave;
    protected JSONNode data;

    protected virtual void OnEnable()
    {
        string tPath = AssetDatabase.GetAssetPath(target);
        _importer = AssetImporter.GetAtPath(tPath);
        data = JSON.Parse(_importer.userData);
        _lastSave = Time.realtimeSinceStartup;
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = true;
        if (Time.realtimeSinceStartup - _lastSave > SAVE_INTERVAL && _save)
            ForceSave();
    }

    protected void Save()
    {
        _save = true;
    }

    protected virtual void OnDestroy()
    {
        ForceSave();
    }

    protected void ForceSave()
    {
        if (_importer == null || data == null)
            return;

        _importer.userData = data.ToString();
        EditorUtility.SetDirty(target);
        AssetDatabase.WriteImportSettingsIfDirty(AssetDatabase.GetAssetPath(target));
        AssetDatabase.SaveAssets();
        _save = false;
    }
}
