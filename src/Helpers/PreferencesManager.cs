using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CalculatriceMargeWPF.Helpers
{
    public class PreferencesManager
    {
        private static PreferencesManager _instance;
        private string _preferencesPath;
        private Dictionary<string, string> _preferences;

        public static PreferencesManager Instance => _instance ??= new PreferencesManager();

        public PreferencesManager()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CalculatriceMarge"
            );
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _preferencesPath = Path.Combine(appDataPath, "preferences.xml");
            _preferences = new Dictionary<string, string>();
            LoadPreferences();
        }

        public void LoadPreferences()
        {
            if (!File.Exists(_preferencesPath))
                return;

            try
            {
                var doc = XDocument.Load(_preferencesPath);
                var root = doc.Root;
                if (root != null)
                {
                    foreach (var elem in root.Elements("setting"))
                    {
                        var key = elem.Attribute("key")?.Value;
                        var value = elem.Attribute("value")?.Value;
                        if (key != null && value != null)
                            _preferences[key] = value;
                    }
                }
            }
            catch { }
        }

        public void SavePreferences()
        {
            try
            {
                var root = new XElement("preferences");
                foreach (var kvp in _preferences)
                {
                    root.Add(new XElement("setting",
                        new XAttribute("key", kvp.Key),
                        new XAttribute("value", kvp.Value)
                    ));
                }
                var doc = new XDocument(root);
                doc.Save(_preferencesPath);
            }
            catch { }
        }

        public string GetPreference(string key, string defaultValue = "")
        {
            return _preferences.ContainsKey(key) ? _preferences[key] : defaultValue;
        }

        public void SetPreference(string key, string value)
        {
            _preferences[key] = value;
            SavePreferences();
        }

        public void SetTheme(string theme)
        {
            SetPreference("theme", theme);
        }

        public string GetTheme()
        {
            return GetPreference("theme", "Light");
        }

        public void SetLanguage(string language)
        {
            SetPreference("language", language);
        }

        public string GetLanguage()
        {
            return GetPreference("language", "fr");
        }

        public void SetAutoSaveEnabled(bool enabled)
        {
            SetPreference("autosave", enabled.ToString());
        }

        public bool GetAutoSaveEnabled()
        {
            return bool.TryParse(GetPreference("autosave", "true"), out var result) && result;
        }

        public void SetAutoSaveInterval(int seconds)
        {
            SetPreference("autosave_interval", seconds.ToString());
        }

        public int GetAutoSaveInterval()
        {
            return int.TryParse(GetPreference("autosave_interval", "60"), out var result) ? result : 60;
        }
    }
}
