using UnityEngine;
using UnityEditor;
using SimpleJSON;

namespace RhoTools.Aseprite
{
    public class CAsepritePostProcesor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                string tPath = importedAssets[i];
                if (tPath.EndsWith(".ase"))
                {
                    AssetImporter tImporter = AssetImporter.GetAtPath(tPath);
                    JSONNode tData = JSON.Parse(tImporter.userData);
                    if (tData == null)
                        tData = new JSONObject();

                    CAsepriteWindow.Environment tEnv = CAsepriteInspector.CreateEnvironment(tData);
                    if (tEnv.autoImport)
                    {
                        bool tDone = CAsepriteExporter.Export(
                            Application.dataPath + tPath.Substring("Assets".Length), ref tEnv);
                        if (tDone && tEnv.importSpritesheet)
                        {
                            TextureImporter tTexture = AssetImporter.GetAtPath(
                                    AssetDatabase.GetAssetPath(tEnv.targetTexture)) as TextureImporter;

                            CAsepriteImporter.ImportSheet(tTexture, tEnv);
                            if (tEnv.importAnimations)
                            {
                                CAsepriteImporter.ImportAnimation(tTexture, ref tEnv);
                                if (tEnv.createAnimator)
                                    CAsepriteWindow.CreateAnimator(ref tEnv);
                            }
                        }

                        if (tDone)
                        {
                            CAsepriteInspector.ApplyData(tEnv, ref tData);
                            tImporter.userData = tData.ToString();
                            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<Object>(tPath));
                            AssetDatabase.WriteImportSettingsIfDirty(tPath);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }
        }
    }
}
