using System.Diagnostics;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RhoTools.Aseprite
{
    /// <summary>
    /// Aseprite export tool
    /// </summary>
    public class CAsepriteExporter
    {
        /// <summary>
        /// Exports a .ase file to a .json and .png file using Aseprite CLI
        /// </summary>
        /// <param name="aFile">.ase file path</param>
        /// <param name="aEnv">Environment data variable</param>
        /// <returns></returns>
        public static bool Export(string aFile, ref CAsepriteWindow.Environment aEnv)
        {
            string tExePath = CAsepriteWindow.asepriteExePath;
            if (tExePath != "" && File.Exists(tExePath))
            {
                // Create arguments
                string tName = Path.GetFileNameWithoutExtension(aFile);

                string tOutPath;
                if (aEnv.targetTexture == null)
                {
                    tOutPath = CAsepriteWindow.TargetDir;
                    if (tOutPath == "")
                        tOutPath = Path.GetDirectoryName(aFile);
                    tOutPath += "/" + tName + ".png";
                }
                else
                    tOutPath = AssetDatabase.GetAssetPath(aEnv.targetTexture);

                int tBorder = aEnv.border;
                string tFramename = "";
                if (aEnv.useTags)
                    tFramename = "{frame000}_{tag}";
                else
                    tFramename = "{frame000}_{title}";
                string args = "-b \"" + aFile + "\" --filename-format "
                    + tFramename + " --sheet-pack ";
                if (tBorder > 0)
                    args += "--inner-padding " + tBorder.ToString() + " ";
                args += "--sheet \"" + tOutPath + "\"";

                // Create process
                Process tProcess = new Process();
                tProcess.StartInfo.FileName = tExePath;
                tProcess.StartInfo.UseShellExecute = false;
                tProcess.StartInfo.Arguments = args;
                tProcess.StartInfo.CreateNoWindow = true;
                tProcess.StartInfo.RedirectStandardOutput = true;
                tProcess.StartInfo.RedirectStandardError = true;

                tProcess.EnableRaisingEvents = true;
                tProcess.Start();

                string tOutput = tProcess.StandardOutput.ReadToEnd();

                using (StreamReader s = tProcess.StandardError)
                {
                    //string error = s.ReadToEnd();
                    tProcess.WaitForExit(20000);
                }

                if (tOutput.Trim() == "")
                {
                    return false; 
                }

                // Apply border to atlas
                if (tBorder > 0)
                {
                    JSONNode tData = JSON.Parse(tOutput);
                    JSONObject tFrames = tData["frames"].AsObject;
                    List<string> tKeys = new List<string>();
                    tKeys.AddRange(tFrames.Keys);
                    foreach (JSONNode tFrame in tFrames.Children)
                    {
                        JSONNode tRect = tFrame["frame"];
                        tRect["x"].AsInt += tBorder;
                        tRect["w"].AsInt -= tBorder * 2;
                        tRect["y"].AsInt += tBorder;
                        tRect["h"].AsInt -= tBorder * 2;
                    }
                    tOutput = CPrettyJSON.FormatJson(tData.ToString());
                }

                string tOutJsonFilePath;
                if (aEnv.targetAtlas == null)
                {
                    tOutJsonFilePath = CAsepriteWindow.TargetDir;
                    if (tOutJsonFilePath == "")
                        tOutJsonFilePath = Path.GetDirectoryName(aFile);
                    tOutJsonFilePath += "/" + tName + ".json";
                }
                else
                    tOutJsonFilePath = AssetDatabase.GetAssetPath(aEnv.targetAtlas);
                CTxtManager.Write(tOutJsonFilePath, tOutput);


                if (tOutJsonFilePath.StartsWith(Application.dataPath))
                    tOutJsonFilePath = "Assets" + tOutJsonFilePath.Substring(Application.dataPath.Length);
                if (tOutPath.StartsWith(Application.dataPath))
                    tOutPath = "Assets" + tOutPath.Substring(Application.dataPath.Length);

                AssetDatabase.ImportAsset(tOutPath,
                    ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.ImportAsset(tOutJsonFilePath,
                    ImportAssetOptions.ForceSynchronousImport);

                aEnv.targetAtlas = AssetDatabase.LoadAssetAtPath<TextAsset>(tOutJsonFilePath);
                aEnv.targetTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(tOutPath);
                return true;
            }

            return false;
        }
    }
}
