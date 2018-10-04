using SimpleJSON;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using RhoTools.ReorderableList;

namespace RhoTools.Aseprite
{
    /// <summary>
    /// Inspector for .ase assets
    /// </summary>
    [CustomFieltypeEditor("ase")]
    public class CAsepriteInspector : Editor
    {
        AssetImporter _importer;
        JSONNode _data;
        bool _save;
        float _lastSave;
        string _savedUserData;
        ClipListAdaptor _clipsAdaptor;

        // Import data
        CAsepriteWindow.Environment _aseEnv;
        #region Constants
        const float SAVE_INTERVAL = 3f;
        const string USE_TAGS = "useTags";
        const string LOOP_ANIM = "loopAnim";
        const string USE_CONFIG = "useConfig";
        const string BORDER = "border";
        const string USE_CHILD = "useChild";
        const string ALIGNMENT = "alignment";
        const string PIVOT_X = "pivotX";
        const string PIVOT_Y = "pivotY";
        const string ANIM_TYPE = "animType";
        const string TARGET_TEXTURE = "targetTexture";
        const string TARGET_TEXT = "targetText";
        const string TARGET_ANIMATOR = "targetAnimator";
        const string CLIPS = "clips";
        const string AUTO_IMPORT = "autoImport";
        const string IMPORT_SPRITESHEET = "importSpritesheet";
        const string IMPORT_ANIMS = "importAnimations";
        const string CREATE_ANIMATOR = "createAnimator";
        const string TARGET_PREFAB = "targetPrefab";
        #endregion

        private void OnEnable()
        {
            var path = AssetDatabase.GetAssetPath(target);
            // Get importer and userData
            if (path.Trim() == "")
                return;
            _importer = AssetImporter.GetAtPath(path);
            _savedUserData = _importer.userData;

            LoadUserData();
        }

        void LoadUserData()
        {
            _data = JSON.Parse(_importer.userData);
            if (_data == null)
                _data = new JSONObject();

            _aseEnv = CreateEnvironment(_data);

            _clipsAdaptor = new ClipListAdaptor(_aseEnv.clips);
        }

        public static CAsepriteWindow.Environment CreateEnvironment(JSONNode aData)
        {
            return new CAsepriteWindow.Environment
            {
                targetTexture = GetAssetByID<Texture2D>(aData[TARGET_TEXTURE].Value),
                targetAtlas = GetAssetByID<TextAsset>(aData[TARGET_TEXT].Value),
                targetAnimator = GetAssetByID<AnimatorController>(aData[TARGET_ANIMATOR].Value),
                targetPrefab = GetAssetByID<GameObject>(aData[TARGET_PREFAB].Value),
                useTags = aData[USE_TAGS].AsBool,
                loopAnim = aData[LOOP_ANIM].AsBool,
                useConfig = aData[USE_CONFIG].AsBool,
                border = aData[BORDER].AsInt,
                useChild = aData[USE_CHILD].AsBool,
                alignment = aData[ALIGNMENT].AsInt,
                animType = (CAsepriteWindow.AnimationType)aData[ANIM_TYPE].AsInt,
                clips = GetAssetListByID<AnimationClip>(aData[CLIPS].AsArray),
                autoImport = aData[AUTO_IMPORT].AsBool,
                importAnimations = aData[IMPORT_ANIMS].AsBool,
                importSpritesheet = aData[IMPORT_SPRITESHEET].AsBool,
                createAnimator = aData[CREATE_ANIMATOR].AsBool,
                pivot = new Vector2(aData[PIVOT_X].AsFloat, aData[PIVOT_Y].AsFloat),
            };
        }

        static T[] GetAssetListByID<T>(JSONArray aArray) where T : Object
        {
            T[] tList = new T[aArray.Count];
            for (int i = 0; i < tList.Length; i++)
            {
                tList[i] = GetAssetByID<T>(aArray[i].Value);
            }

            return tList;
        }

        static T GetAssetByID<T>(string aID) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(
                AssetDatabase.GUIDToAssetPath(aID));
        }

        static string GetIDByAsset(Object aAsset)
        {
            return AssetDatabase.AssetPathToGUID(
                AssetDatabase.GetAssetPath(aAsset));
        }

        public override void OnInspectorGUI()
        {
            ASEInspectorGUI();
        }

        private void ASEInspectorGUI()
        {
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            _aseEnv.targetTexture = (Texture2D)EditorGUILayout.ObjectField
                ("Target texture",
                _aseEnv.targetTexture,
                typeof(Texture2D),
                false
            );
            _aseEnv.targetAtlas = (TextAsset)EditorGUILayout.ObjectField(
                "Target atlas",
                _aseEnv.targetAtlas,
                typeof(TextAsset),
                false
            );
            _aseEnv.targetAnimator = (AnimatorController)EditorGUILayout.ObjectField(
                "Target animator controller",
                _aseEnv.targetAnimator,
                typeof(AnimatorController),
                false
            );
            _aseEnv.targetPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Target prefab",
                _aseEnv.targetPrefab,
                typeof(GameObject),
                false
            );
            _aseEnv.useTags = EditorGUILayout.Toggle("Use tags",
                _aseEnv.useTags);
            _aseEnv.useConfig = EditorGUILayout.Toggle("Use custom configuration",
                _aseEnv.useConfig);
            EditorGUI.BeginDisabledGroup(_aseEnv.useConfig);
            _aseEnv.loopAnim = EditorGUILayout.Toggle("Loop animation",
                _aseEnv.loopAnim);
            EditorGUI.EndDisabledGroup();
            _aseEnv.border = EditorGUILayout.IntField("Border", _aseEnv.border);
            _aseEnv.useChild = EditorGUILayout.Toggle("Use child object for animation",
                _aseEnv.useChild);
            SpriteAlignment tAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot:",
                (SpriteAlignment)_aseEnv.alignment);
            EditorGUI.BeginDisabledGroup(tAlignment != SpriteAlignment.Custom);
            _aseEnv.pivot = EditorGUILayout.Vector2Field("Custom pivot",
                _aseEnv.pivot);
            EditorGUI.EndDisabledGroup();
            _aseEnv.animType = (CAsepriteWindow.AnimationType)EditorGUILayout.EnumPopup(
                "Animation type", _aseEnv.animType);

            ReorderableListGUI.Title("Clips");
            ReorderableListGUI.ListField(_clipsAdaptor);

            _aseEnv.importSpritesheet = EditorGUILayout.Toggle("Import spritesheet",
                _aseEnv.importSpritesheet);
            EditorGUI.BeginDisabledGroup(!_aseEnv.importSpritesheet);
            _aseEnv.importAnimations = EditorGUILayout.Toggle("Import animations",
                _aseEnv.importAnimations);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!(_aseEnv.importAnimations && _aseEnv.importSpritesheet));
            _aseEnv.createAnimator = EditorGUILayout.Toggle("Create animator",
                _aseEnv.createAnimator);
            EditorGUI.EndDisabledGroup();

            _aseEnv.autoImport = EditorGUILayout.Toggle(
                "Auto import", _aseEnv.autoImport);

            // Save
            if (EditorGUI.EndChangeCheck())
            {
                _aseEnv.alignment = (int)tAlignment;
                _aseEnv.clips = _clipsAdaptor.list;
                ApplyData(_aseEnv, ref _data);
                _save = true;
            }
 
            if(GUILayout.Button("Import"))
            {
                string tPath = AssetDatabase.GetAssetPath(target);

                tPath = Application.dataPath + tPath.Substring("Assets".Length);
                bool tDone = CAsepriteExporter.Export(tPath, ref _aseEnv);
                if (_aseEnv.importSpritesheet && tDone)
                {
                    TextureImporter tTexture = AssetImporter.GetAtPath(
                            AssetDatabase.GetAssetPath(_aseEnv.targetTexture)) as TextureImporter;

                    CAsepriteImporter.ImportSheet(tTexture, _aseEnv);
                    if (_aseEnv.importAnimations)
                    {
                        CAsepriteImporter.ImportAnimation(tTexture, ref _aseEnv);
                        if (_aseEnv.createAnimator)
                            CAsepriteWindow.CreateAnimator(ref _aseEnv);

                        _clipsAdaptor.list = _aseEnv.clips;
                    }
                }

                if (tDone)
                {
                    ApplyData(_aseEnv, ref _data);
                    ForceSave();
                }
            }
            EditorGUI.BeginDisabledGroup(_aseEnv.targetAnimator == null);
            if (GUILayout.Button("Create prefab"))
            {
                CAsepriteWindow.CreatePrefab(ref _aseEnv);

                ApplyData(_aseEnv, ref _data);
                ForceSave();
            }
            EditorGUI.EndDisabledGroup();

            if (Time.realtimeSinceStartup - _lastSave >= SAVE_INTERVAL)
            {
                if (_save)
                {
                    ForceSave();
                    _save = false;
                    _lastSave = Time.realtimeSinceStartup;
                }
            }

            if (_importer.userData != _savedUserData)
            {
                LoadUserData();
            }
        }

        AnimationClip DrawClip(Rect position, AnimationClip aClip)
        {
            return (AnimationClip)EditorGUI.ObjectField(position, aClip,
                typeof(AnimationClip), false);
        }

        private void OnDestroy()
        {
            ForceSave();
        }

        void ForceSave()
        {
            if (_importer == null || _data == null)
                return;

            _importer.userData = _data.ToString();
            _savedUserData = _importer.userData;

            //_importer.SaveAndReimport();
            EditorUtility.SetDirty(target);
            AssetDatabase.WriteImportSettingsIfDirty(AssetDatabase.GetAssetPath(target));
            AssetDatabase.SaveAssets();
            _save = false;
        }

        /// <summary>
        /// Save all environment data to json node
        /// </summary>
        /// <param name="aEnv">Environment data</param>
        /// <param name="aData">JSON node</param>
        public static void ApplyData(CAsepriteWindow.Environment aEnv, ref JSONNode aData)
        {
            aData[TARGET_TEXTURE] = GetIDByAsset(aEnv.targetTexture);
            aData[TARGET_TEXT] = GetIDByAsset(aEnv.targetAtlas);
            aData[TARGET_ANIMATOR] = GetIDByAsset(aEnv.targetAnimator);
            aData[TARGET_PREFAB] = GetIDByAsset(aEnv.targetPrefab);
            aData[USE_TAGS].AsBool = aEnv.useTags;
            aData[LOOP_ANIM].AsBool = aEnv.loopAnim;
            aData[USE_CONFIG].AsBool = aEnv.useConfig;
            aData[BORDER].AsInt = aEnv.border;
            aData[USE_CHILD].AsBool = aEnv.useChild;
            aData[ALIGNMENT].AsInt = aEnv.alignment;
            aData[PIVOT_X].AsFloat = aEnv.pivot.x;
            aData[PIVOT_Y].AsFloat = aEnv.pivot.y;
            aData[ANIM_TYPE].AsInt = (int)aEnv.animType;
            JSONArray tIDS = new JSONArray();
            for (int i = 0; i < aEnv.clips.Length; i++)
                tIDS.Add(GetIDByAsset(aEnv.clips[i]));
            aData[CLIPS] = tIDS;
            aData[AUTO_IMPORT].AsBool = aEnv.autoImport;
            aData[IMPORT_ANIMS].AsBool = aEnv.importAnimations;
            aData[IMPORT_SPRITESHEET].AsBool = aEnv.importSpritesheet;
            aData[CREATE_ANIMATOR].AsBool = aEnv.createAnimator;
        }

        class ClipListAdaptor : IReorderableListAdaptor
        {
            public AnimationClip[] list;

            public ClipListAdaptor(AnimationClip[] aList)
            {
                list = aList;
            }

            public void Add()
            {
                ArrayUtility.Add(ref list, null);
            }

            public int Count
            {
                get
                {
                    return list.Length;
                }
            }

            public void BeginGUI()
            {

            }

            public bool CanDrag(int index)
            {
                return true;
            }

            public bool CanRemove(int index)
            {
                return true;
            }

            public void Clear()
            {
                list = new AnimationClip[0];
            }

            public void DrawItem(Rect position, int index)
            {
                list[index] = (AnimationClip)EditorGUI.ObjectField(position, list[index],
                    typeof(AnimationClip), false);
            }

            public void DrawItemBackground(Rect position, int index)
            {

            }

            public void Duplicate(int index)
            {
                ArrayUtility.Add(ref list, list[index]);
            }

            public void EndGUI()
            {

            }

            public float GetItemHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public void Insert(int index)
            {
                ArrayUtility.Insert(ref list, index, null);
            }

            public void Move(int sourceIndex, int destIndex)
            {
                AnimationClip tClip = list[sourceIndex];
                ArrayUtility.RemoveAt(ref list, sourceIndex);
                if (destIndex >= Count)
                    ArrayUtility.Add(ref list, tClip);
                else
                    ArrayUtility.Insert(ref list, destIndex, tClip);
            }

            public void Remove(int index)
            {
                ArrayUtility.RemoveAt(ref list, index);
            }
        }
    }
}
