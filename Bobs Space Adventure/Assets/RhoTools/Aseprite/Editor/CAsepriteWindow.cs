using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;

namespace RhoTools.Aseprite
{
    /// <summary>
    /// Editor window for the aseprite export/import tool
    /// </summary>
    public class CAsepriteWindow : EditorWindow
    {
        const string PREF_BORDER = "Aseprite_border";
        const string PREF_EXE_PATH = "Aseprite_exe_path";
        const string USE_TAGS = "Aseprite_use_tags";
        const string USE_CONFIGS = "Aseprite_use_configs";
        const string LOOP_ANIM = "Aseprite_loop_anim";
        const string USE_CHILD = "Aseprite_use_child";
        const string PIVOT = "Aseprite_pivot";
        const string ALIGNMENT = "Aseprite_alignment";
        const string ANIM_TYPE = "Aseprite_animType";
        const string PREF_SELECTED_TAB = "Aseprite_selected_tab";
        const string PREF_TARGET_DIR = "Aseprite_target_dir";
        const string PREF_TARGET_ANIM_DIR = "Aseprite_target_anim_dir";
        const string PREF_TARGET_PREFABS_DIR = "Aseprite_target_prefabs_dir";
        const string ASE_EXTENSION = "ase";

        /// <summary>
        /// Type of animation to be used
        /// </summary>
        public enum AnimationType
        {
            /// <summary>
            /// <see cref="UnityEngine.SpriteRenderer"/>
            /// </summary>
            SpriteRenderer,
            /// <summary>
            /// <see cref="UnityEngine.UI.Image"/>
            /// </summary>
            Image,
        }

        #region Window definition
        /// <summary>
        /// Open window
        /// </summary>
        [MenuItem(CConstants.ROOT_MENU + "Aseprite Importer...")]
        public static void ShowWindow()
        {
            EditorWindow tWindow = GetWindow(typeof(CAsepriteWindow));
            // Loads an icon from an image stored at the specified path
            MonoScript tScript = MonoScript.FromScriptableObject(tWindow);
            string tPath = AssetDatabase.GetAssetPath(tScript);
            tPath = Path.GetDirectoryName(tPath) + "/icon.png";
            Texture tIcon = AssetDatabase.LoadAssetAtPath<Texture>(tPath);
            // Create the instance of GUIContent to assign to the window. Gives the title "RBSettings" and the icon
            GUIContent titleContent = new GUIContent("AsepriteImp", tIcon);
            tWindow.titleContent = titleContent;
        }
        #endregion

        Texture2D _spritesheetTexture;
        TextAsset _asepriteAtlas;
        Object _fileObj;
        bool _autoImport;
        bool _importAnim;
        bool _createPrefab;
        System.Action[] _drawCallbacks;
        string[] _titles;

        #region Editor Prefs
        /// <summary>
        /// Set to true if tags should be used to separate animations.
        /// Otherwise all will be imported as one animation.
        /// </summary>
        public static bool UseTags
        {
            get
            {
                return EditorProjectPrefs.GetBool(USE_TAGS, false);
            }
            set
            {
                EditorProjectPrefs.SetBool(USE_TAGS, value);
            }
        }

        /// <summary>
        /// Set to true if resulting animations should loop (affects all)
        /// </summary>
        public static bool LoopAnim
        {
            get
            {
                return EditorProjectPrefs.GetBool(LOOP_ANIM, false);
            }
            set
            {
                EditorProjectPrefs.SetBool(LOOP_ANIM, value);
            }
        }

        /// <summary>
        /// Set to true if animation configuration should be taken from the tag names
        /// (disables LoopAnim)
        /// </summary>
        public static bool UseConfig
        {
            get
            {
                return EditorProjectPrefs.GetBool(USE_CONFIGS, false);
            }
            set
            {
                EditorProjectPrefs.SetBool(USE_CONFIGS, value);
            }
        }

        /// <summary>
        /// Path to Aseprite executable
        /// </summary>
        public static string asepriteExePath
        {
            get
            {
                return EditorPrefs.GetString(PREF_EXE_PATH, "");
            }
            set
            {
                EditorPrefs.SetString(PREF_EXE_PATH, value);
            }
        }

        /// <summary>
        /// Border to be left bewteen sprites in the spritesheet
        /// </summary>
        public static int Border
        {
            get
            {
                return EditorProjectPrefs.GetInt(PREF_BORDER, 0);
            }
            set
            {
                EditorProjectPrefs.SetInt(PREF_BORDER, value);
            }
        }

        /// <summary>
        /// Set to true if the resulting animation should be on an object child to the root
        /// of the animation
        /// </summary>
        public static bool UseChild
        {
            get
            {
                return EditorProjectPrefs.GetBool(USE_CHILD, true);
            }
            set
            {
                EditorProjectPrefs.SetBool(USE_CHILD, value);
            }
        }

        /// <summary>
        /// Alignment used to set the pivot of the sprites (will be converted to <see cref="UnityEngine.SpriteAlignment"/>)
        /// </summary>
        public static int Alignment
        {
            get
            {
                return EditorProjectPrefs.GetInt(ALIGNMENT, 0);
            }
            set
            {
                EditorProjectPrefs.SetInt(ALIGNMENT, value);
            }
        }

        /// <summary>
        /// Custom pivot to be set on the sprites (only considered if Alignment is set to "Custom")
        /// </summary>
        public static Vector2 Pivot
        {
            get
            {
                return Vector2.right * EditorProjectPrefs.GetFloat(PIVOT + "x", 0.5f)
                    + Vector2.up * EditorProjectPrefs.GetFloat(PIVOT + "y", .5f);
            }
            set
            {
                EditorProjectPrefs.SetFloat(PIVOT + "x", value.x);
                EditorProjectPrefs.SetFloat(PIVOT + "y", value.y);
            }
        }

        /// <summary>
        /// Sets the component to be used in the animation
        /// </summary>
        public static AnimationType AnimType
        {
            get
            {
                return (AnimationType)EditorProjectPrefs.GetInt(ANIM_TYPE, 0);
            }
            set
            {
                EditorProjectPrefs.SetInt(ANIM_TYPE, (int)value);
            }
        }

        /// <summary>
        /// Selected tab in the window
        /// </summary>
        public static int SelectedTab
        {
            get
            {
                return EditorProjectPrefs.GetInt(PREF_SELECTED_TAB, 0);
            }
            set
            {
                EditorProjectPrefs.SetInt(PREF_SELECTED_TAB, value);
            }
        }

        /// <summary>
        /// Target sprites and json directory
        /// </summary>
        public static string TargetDir
        {
            get
            {
                return EditorProjectPrefs.GetString(PREF_TARGET_DIR,
                    "Assets/RhoTools/Aseprite/Assets/Sprites");
            }
            set
            {
                string tVal = value.Replace("\\", "/").Trim();
                if (tVal.StartsWith(Application.dataPath))
                    tVal = "Assets" + tVal.Substring(Application.dataPath.Length);
                EditorProjectPrefs.SetString(PREF_TARGET_DIR, tVal);
            }
        }

        /// <summary>
        /// Target animations directory
        /// </summary>
        public static string TargetAnimDir
        {
            get
            {
                string tDir = EditorProjectPrefs.GetString(PREF_TARGET_ANIM_DIR,
                    "Assets/RhoTools/Aseprite/Assets/Animations");
                if (tDir != "" && Directory.Exists(tDir))
                    return tDir;
                else
                    return TargetDir;
            }
            set
            {
                string tVal = value.Replace("\\", "/").Trim();
                if (tVal.StartsWith(Application.dataPath))
                    tVal = "Assets" + tVal.Substring(Application.dataPath.Length);
                EditorProjectPrefs.SetString(PREF_TARGET_ANIM_DIR, tVal);
            }
        }

        /// <summary>
        /// Target prefabs directory
        /// </summary>
        public static string TargetPrefabsDir
        {
            get
            {
                string tDir = EditorProjectPrefs.GetString(PREF_TARGET_PREFABS_DIR,
                    "Assets/RhoTools/Aseprite/Assets/Prefabs");
                if (tDir != "" && Directory.Exists(tDir))
                    return tDir;
                else
                    return TargetDir;
            }
            set
            {
                string tVal = value.Replace("\\", "/").Trim();
                if (tVal.StartsWith(Application.dataPath))
                    tVal = "Assets" + tVal.Substring(Application.dataPath.Length);
                EditorProjectPrefs.SetString(PREF_TARGET_PREFABS_DIR, tVal);
            }
        }
        #endregion

        /// <summary>
        /// Environment data for import configuration
        /// </summary>
        public struct Environment
        {
            public Texture2D targetTexture;
            public TextAsset targetAtlas;
            public AnimatorController targetAnimator;
            public GameObject targetPrefab;
            public bool useTags;
            public bool loopAnim;
            public bool useConfig;
            public int border;
            public bool useChild;
            public int alignment;
            public Vector2 pivot;
            public AnimationType animType;
            public bool autoImport;
            public bool importSpritesheet;
            public bool importAnimations;
            public bool createAnimator;
            public AnimationClip[] clips;
        }

        static GUIStyle _boxStyle;
        static GUIStyle boxStyle
        {
            get
            {
                if (_boxStyle == null)
                {
                    _boxStyle = new GUIStyle(GUI.skin.box);
                    Texture2D tTex = new Texture2D(1, 1);
                    tTex.SetPixel(0, 0, Color.gray);
                    tTex.Apply();
                    _boxStyle.normal.background = tTex;
                }
                return _boxStyle;
            }
        }

        void OnEnable()
        {
            _drawCallbacks = new System.Action[2];
            _titles = new string[_drawCallbacks.Length];
            _drawCallbacks[0] = OnAsepriteFile;
            _titles[0] = "Use aseprite file";
            _drawCallbacks[1] = OnSpriteSheetAndAtlas;
            _titles[1] = "Use spritesheet and atlas";
        }

        void OnGUI()
        {
            // Export
            GUILayout.Label("Export Aseprite Spritesheet", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            string tExePath = CEditorGUILayout.FilePathField(
                "Aseprite executable path:",
                asepriteExePath,
                "Aseprite executable",
                "C:\\Program Files\\",
                ""
            );
            if (EditorGUI.EndChangeCheck())
            {
                asepriteExePath = tExePath;
            }
            GUILayout.Label("Defaults", EditorStyles.boldLabel);
            // Target dir
            GUIContent tContent = new GUIContent("Target sprites directory");
            tContent.tooltip = "Export output will be saved here";
            EditorGUI.BeginChangeCheck();
            string tTargetDir = CEditorGUILayout.DirPathField(
                tContent, TargetDir, "Select target directory", "");
            if (EditorGUI.EndChangeCheck())
            {
                TargetDir = tTargetDir;
            }

            // Target anim dir
            tContent = new GUIContent("Target animations directory");
            tContent.tooltip = "Output animations and animators will be saved here";
            EditorGUI.BeginChangeCheck();
            tTargetDir = CEditorGUILayout.DirPathField(
                tContent, TargetAnimDir, "Select target directory", "");
            if (EditorGUI.EndChangeCheck())
            {
                TargetAnimDir = tTargetDir;
            }

            // Target anim dir
            tContent = new GUIContent("Target prefabs directory");
            tContent.tooltip = "Output prefabs will be saved here";
            EditorGUI.BeginChangeCheck();
            tTargetDir = CEditorGUILayout.DirPathField(
                tContent, TargetPrefabsDir, "Select target directory", "");
            if (EditorGUI.EndChangeCheck())
            {
                TargetPrefabsDir = tTargetDir;
            }

            if (asepriteExePath == "")
            {
                CEditorGUILayout.Box(50, "You must select the Aseprite executable file above.");
                return;
            }

            int tSelected = CEditorGUILayout.DrawTabs(_titles, _drawCallbacks, SelectedTab);
            if (tSelected != SelectedTab)
                SelectedTab = tSelected;

            // Options
            GUILayout.Label("Options", EditorStyles.boldLabel);
            GUILayout.BeginVertical(boxStyle);

            // Line 1
            GUILayout.BeginHorizontal();
            _autoImport = GUILayout.Toggle(_autoImport, "Import spritesheet");
            _importAnim = GUILayout.Toggle(_importAnim, "Import animation");
            EditorGUI.BeginDisabledGroup(!_importAnim);
            _createPrefab = GUILayout.Toggle(_createPrefab, "Create prefab");
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            // Line 2
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            bool tUseTags = GUILayout.Toggle(UseTags, "Use tags");
            bool tUseConfigs = GUILayout.Toggle(UseConfig, "Use custom configuration");
            EditorGUI.BeginDisabledGroup(UseConfig);
            bool tLoopAnim = GUILayout.Toggle(LoopAnim, "Loop animation");
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                UseTags = tUseTags;
                LoopAnim = tLoopAnim;
                UseConfig = tUseConfigs;
            }
            GUILayout.EndHorizontal();

            // Line 3
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            bool tUseChild = GUILayout.Toggle(UseChild, "Use child sprite for animation");
            if (EditorGUI.EndChangeCheck())
            {
                UseChild = tUseChild;
            }
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            int tBorder = EditorGUILayout.IntField("Border", Border);
            if (EditorGUI.EndChangeCheck())
            {
                Border = tBorder;
            }

            // Line 4
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            SpriteAlignment tAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot:",
                (SpriteAlignment)Alignment);
            EditorGUI.BeginDisabledGroup(tAlignment != SpriteAlignment.Custom);
            Vector2 tPivot = EditorGUILayout.Vector2Field("Custom pivot", Pivot);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                Pivot = tPivot;
                Alignment = (int)tAlignment;
            }
            GUILayout.EndHorizontal();

            // Line 5
            EditorGUI.BeginChangeCheck();
            AnimationType tType = (AnimationType)EditorGUILayout.EnumPopup("Animation type:",
                AnimType);
            if (EditorGUI.EndChangeCheck())
            {
                AnimType = tType;
            }


            GUILayout.EndVertical();
        }

        public static void CreateAnimator(ref Environment aEnv)
        {
            // Check if animator already exists
            if (aEnv.targetAnimator == null)
            {
                string tControllerPath = AssetDatabase.GetAssetPath(aEnv.targetTexture);
                tControllerPath = TargetAnimDir + "/" +
                    Path.GetFileNameWithoutExtension(tControllerPath) + ".controller";
                aEnv.targetAnimator = AnimatorController.CreateAnimatorControllerAtPath(
                    tControllerPath);
            }

            AnimatorStateMachine tRootStateMachine = aEnv.targetAnimator.layers[0].stateMachine;
            for (int i = 0; i < aEnv.clips.Length; i++)
            {
                bool tFound = false;
                AnimationClip tClip = aEnv.clips[i];
                for (int j = 0; j < tRootStateMachine.states.Length; j++)
                {
                    if (tRootStateMachine.states[j].state.motion == tClip)
                    {
                        tFound = true;
                        break;
                    }
                }
                if (!tFound)
                {
                    AnimatorState tState = tRootStateMachine.AddState(tClip.name);
                    tState.motion = tClip;
                }
            }
        }
        
        /// <summary>
        /// Creates a prefab using the environment data
        /// </summary>
        /// <param name="aEnv">Environment data</param>
        public static void CreatePrefab(ref Environment aEnv)
        {
            string tPrefabPath = AssetDatabase.GetAssetPath(aEnv.targetPrefab);
            if (tPrefabPath.Trim() == "")
            {
                string tPath = AssetDatabase.GetAssetPath(aEnv.targetTexture);
                tPrefabPath = TargetPrefabsDir + "/" +
                    Path.GetFileNameWithoutExtension(tPath);
            }
            tPrefabPath += ".prefab";
            if (tPrefabPath.StartsWith(Application.dataPath))
                tPrefabPath = "Assets" + tPrefabPath.Substring(Application.dataPath.Length);

            AssetDatabase.DeleteAsset(tPrefabPath);
            GameObject tAnimGameObject = new GameObject();
            Animator tAnimator = tAnimGameObject.AddComponent<Animator>();

            // Create or edit the animator
            CreateAnimator(ref aEnv);

            tAnimator.runtimeAnimatorController = null;
            tAnimator.runtimeAnimatorController = aEnv.targetAnimator;

            bool tUseChild = aEnv.useChild;
            SpriteRenderer tRenderer;
            Image tImage;
            EditorCurveBinding spriteBinding = new EditorCurveBinding();

            if (aEnv.animType == AnimationType.SpriteRenderer)
                spriteBinding.type = typeof(SpriteRenderer);
            else if (aEnv.animType == AnimationType.Image)
                spriteBinding.type = typeof(Image);

            spriteBinding.propertyName = "m_Sprite";
            if (tUseChild)
                spriteBinding.path = CAsepriteImporter.BINDING_PATH;
            else
                spriteBinding.path = "";

            ObjectReferenceKeyframe[] tRef = AnimationUtility.GetObjectReferenceCurve(
                aEnv.clips[0], spriteBinding);
            if (aEnv.animType == AnimationType.SpriteRenderer)
            {
                if (tUseChild)
                {
                    tRenderer = new GameObject(CAsepriteImporter.BINDING_PATH).AddComponent<SpriteRenderer>();
                    tRenderer.transform.SetParent(tAnimGameObject.transform);
                }
                else
                    tRenderer = tAnimGameObject.AddComponent<SpriteRenderer>();
                if (tRef != null && tRef.Length > 0)
                    tRenderer.sprite = tRef[0].value as Sprite;
                else
                    Debug.LogWarning("The asset is marked as \"" + (aEnv.useChild ? "don't " : "")
                        + "use child object for animation\" but the animation seems to be configured differently."
                        + "\nTry changing this configuration.");
            }
            else if (aEnv.animType == AnimationType.Image)
            {
                tAnimGameObject.AddComponent<RectTransform>();
                if (tUseChild)
                {
                    tImage = new GameObject(CAsepriteImporter.BINDING_PATH).AddComponent<Image>();
                    tImage.transform.SetParent(tAnimGameObject.transform);
                }
                else
                    tImage = tAnimGameObject.AddComponent<Image>();

                if (tRef != null && tRef.Length > 0)
                    tImage.sprite = tRef[0].value as Sprite;
                else
                    Debug.LogWarning("The asset is marked as \"" + (aEnv.useChild ? "don't " : "")
                        + "use child object for animation\" but the animation seems to be configured differently."
                        + "\nTry changing this configuration.");
            }

            aEnv.targetPrefab = PrefabUtility.CreatePrefab(tPrefabPath, tAnimGameObject);
            DestroyImmediate(tAnimGameObject);
        }

        void OnAsepriteFile()
        {
            EditorGUI.BeginChangeCheck();
            Object tAsepriteFile = EditorGUILayout.ObjectField("Aseprite file", _fileObj,
                typeof(Object), false);
            if (EditorGUI.EndChangeCheck())
            {
                string tPath = AssetDatabase.GetAssetPath(tAsepriteFile);
                if (tPath.EndsWith("." + ASE_EXTENSION))
                {
                    _fileObj = tAsepriteFile;
                }
                else
                    Debug.LogWarning("The file extension must be '." + ASE_EXTENSION + "'");
            }

            string tButtonText = "Export";
            if (_autoImport)
                tButtonText += " and import";

            if (GUILayout.Button(tButtonText) && _fileObj != null)
            {
                string tPath = AssetDatabase.GetAssetPath(_fileObj);

                tPath = Application.dataPath + tPath.Substring("Assets".Length);
                // Create environment
                Environment tEnv = CreateEnvironment();
                bool tDone = CAsepriteExporter.Export(tPath, ref tEnv);
                if (_autoImport && tDone)
                {
                    /*
                    string tRelPath;
                    if (tOut.StartsWith(Application.dataPath))
                        tRelPath = "Assets" + tOut.Substring(Application.dataPath.Length);
                    else
                        tRelPath = tOut;

                    string tTexturePath = tRelPath + ".png";
                    string tAtlasPath = tRelPath + ".json";
                    */
                    TextureImporter tTexture = AssetImporter.GetAtPath(
                        AssetDatabase.GetAssetPath(tEnv.targetTexture)) as TextureImporter;
                    CAsepriteImporter.ImportSheet(tTexture, tEnv);
                    if (_importAnim)
                    {
                        CAsepriteImporter.ImportAnimation(tTexture, ref tEnv);
                        if (_createPrefab)
                        {
                            //string tPrefPath = AssetDatabase.GetAssetPath(tEnv.targetTexture);
                            CreatePrefab(ref tEnv);
                        }
                    }

                    JSONNode tData = new JSONObject();
                    CAsepriteInspector.ApplyData(tEnv, ref tData);
                    AssetImporter tImporter = AssetImporter.GetAtPath(
                        AssetDatabase.GetAssetPath(_fileObj));
                    tImporter.userData = tData.ToString();
                    tImporter.SaveAndReimport();
                }
                else if (!tDone)
                    EditorUtility.DisplayDialog("Error", "There was an error running the aseprite executable."
                        + "\nCheck if the path is correct.", "OK");
            }
        }

        void OnSpriteSheetAndAtlas()
        {
            _spritesheetTexture = (Texture2D)EditorGUILayout.ObjectField("Spritesheet",
                _spritesheetTexture, typeof(Texture2D), false);
            EditorGUI.BeginChangeCheck();
            _asepriteAtlas = (TextAsset)EditorGUILayout.ObjectField("Atlas",
                _asepriteAtlas, typeof(TextAsset), false);

            if (GUILayout.Button("Import"))
            {
                if (_asepriteAtlas != null && _spritesheetTexture != null)
                {
                    string tTexturePath = AssetDatabase.GetAssetPath(_spritesheetTexture);

                    // Load texture
                    TextureImporter tTexture = AssetImporter.GetAtPath(tTexturePath)
                        as TextureImporter;
                    if (tTexture != null)
                    {
                        Environment tEnv = CreateEnvironment();
                        tEnv.targetAtlas = _asepriteAtlas;
                        tEnv.targetTexture = _spritesheetTexture;
                        if (_autoImport)
                            CAsepriteImporter.ImportSheet(tTexture, tEnv);
                        if (_importAnim)
                        {
                            CAsepriteImporter.ImportAnimation(tTexture, ref tEnv);
                            if (_createPrefab)
                            {
                                //string tPrefPath = AssetDatabase.GetAssetPath(tEnv.targetTexture);
                                CreatePrefab(ref tEnv);
                            }
                        }
                    }
                }
            }
        }

        Environment CreateEnvironment()
        {
            return new Environment
            {
                targetAnimator = null,
                targetAtlas = null,
                targetTexture = null,
                targetPrefab = null,
                alignment = Alignment,
                animType = AnimType,
                border = Border,
                loopAnim = LoopAnim,
                pivot = Pivot,
                useChild = UseChild,
                useConfig = UseConfig,
                useTags = UseTags,
                autoImport = true,
                importAnimations = _importAnim,
                importSpritesheet = _autoImport,
                createAnimator = _createPrefab,
                clips = new AnimationClip[0],
            };
        }
    }
}