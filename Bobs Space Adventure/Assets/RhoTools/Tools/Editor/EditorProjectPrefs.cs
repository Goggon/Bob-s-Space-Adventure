using SimpleJSON;
using System.IO;

namespace RhoTools
{
    public static class EditorProjectPrefs
    {
        const string FILEPATH = "EditorProjectPrefs.json";
        static string FilePath { get { return CConstants.SETTINGS_PATH + FILEPATH; } }

        static JSONObject _prefs;
        static JSONObject Prefs
        {
            get
            {
                if (_prefs == null)
                {
                    if (File.Exists(FilePath))
                    {
                        try
                        {
                            JSONNode tNode = JSON.Parse(File.ReadAllText(FilePath));
                            if (tNode.IsObject)
                                _prefs = tNode.AsObject;
                            else
                                _prefs = new JSONObject();
                        }
                        catch
                        {
                            _prefs = new JSONObject();
                        }
                    }
                    else
                        _prefs = new JSONObject();
                }
                return _prefs;
            }
        }

        // Bool
        /// <summary>
        /// Sets the value of the preferences identified by the key.
        /// </summary>
        public static void SetBool(string key, bool value)
        {
            if (Prefs[key] == null)
                Prefs[key] = new JSONBool(value);
            else
                Prefs[key].AsBool = value;
            Save();
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static bool GetBool(string key, bool defaultValue)
        {
            if (Prefs[key] == null)
                return defaultValue;
            else
                return Prefs[key].AsBool;
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static bool GetBool(string key)
        {
            return GetBool(key, false);
        }

        // String
        /// <summary>
        /// Sets the value of the preferences identified by the key.
        /// </summary>
        public static void SetString(string key, string value)
        {
            if (Prefs[key] == null)
                Prefs[key] = new JSONString(value);
            else
                Prefs[key].Value = value;
            Save();
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static string GetString(string key, string defaultValue)
        {
            if (Prefs[key] == null)
                return defaultValue;
            else
                return Prefs[key].Value;
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static string GetString(string key)
        {
            return GetString(key, "");
        }

        // Int
        /// <summary>
        /// Sets the value of the preferences identified by the key.
        /// </summary>
        public static void SetInt(string key, int value)
        {
            if (Prefs[key] == null)
                Prefs[key] = new JSONNumber(value);
            else
                Prefs[key].AsInt = value;
            Save();
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static int GetInt(string key, int defaultValue)
        {
            if (Prefs[key] == null)
                return defaultValue;
            else
                return Prefs[key].AsInt;
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        // Float
        /// <summary>
        /// Sets the value of the preferences identified by the key.
        /// </summary>
        public static void SetFloat(string key, float value)
        {
            if (Prefs[key] == null)
                Prefs[key] = new JSONNumber(value);
            else
                Prefs[key].AsFloat = value;
            Save();
        }


        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static float GetFloat(string key, float defaultValue)
        {
            if (Prefs[key] == null)
                return defaultValue;
            else
                return Prefs[key].AsFloat;
        }


        /// <summary>
        /// Returns the value corresponding to the key in the preferences file if it exists.
        /// </summary>
        public static float GetFloat(string key)
        {
            return GetFloat(key, 0f);
        }

        public static void Save()
        {
            Directory.CreateDirectory(CConstants.SETTINGS_PATH);
            File.WriteAllText(FilePath, Prefs.ToString());
        }

        /// <summary>
        /// Removes key and its corresponding value from the preferences.
        /// </summary>
        public static void DeleteKey(string key)
        {
            Prefs.Remove(key);
            Save();
        }

        /// <summary>
        /// Removes all keys and values from the preferences. Use with caution.
        /// </summary>
        public static void DeleteAll()
        {
            _prefs = new JSONObject();
            Save();
        }

        /// <summary>
        /// Returns true if the key exists in the preferences file.
        /// </summary>
        public static bool HasKey(string key)
        {
            return Prefs[key] != null;
        }
    }
}
