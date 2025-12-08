using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Preset pour sauvegarder les configurations fréquentes
    /// </summary>
    public class Preset
    {
        public string Name { get; set; }
        public double DebourseSec { get; set; }
        public double PrixVenteHT { get; set; }
        public double TVA { get; set; }
        public double FraisGeneraux { get; set; }
        public bool FraisEnPourcent { get; set; }
        public DateTime CreatedAt { get; set; }

        public Preset()
        {
            CreatedAt = DateTime.Now;
            FraisEnPourcent = true;
            TVA = 20;
            FraisGeneraux = 10;
        }

        public override string ToString() => $"{Name} (TVA: {TVA}%, FG: {FraisGeneraux}{(FraisEnPourcent ? "%" : "€")})";
    }

    /// <summary>
    /// Gestionnaire de presets
    /// </summary>
    public class PresetManager
    {
        private string _presetsPath;
        public ObservableCollection<Preset> Presets { get; }

        public PresetManager()
        {
            Presets = new ObservableCollection<Preset>();
            InitPresetsFolder();
            LoadPresets();
        }

        private void InitPresetsFolder()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appDataFolder, "CalculatriceMarge", "Presets");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            _presetsPath = Path.Combine(folder, "presets.json");
        }

        public void SavePreset(Preset preset)
        {
            // Vérifier si le preset existe déjà
            var existing = Presets.FirstOrDefault(p => p.Name == preset.Name);
            if (existing != null)
                Presets.Remove(existing);

            Presets.Add(preset);
            PersistPresets();
        }

        public void DeletePreset(Preset preset)
        {
            Presets.Remove(preset);
            PersistPresets();
        }

        private void PersistPresets()
        {
            try
            {
                var json = JsonSerializer.Serialize(Presets, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_presetsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur sauvegarde presets: {ex.Message}");
            }
        }

        private void LoadPresets()
        {
            try
            {
                if (File.Exists(_presetsPath))
                {
                    var json = File.ReadAllText(_presetsPath);
                    var loaded = JsonSerializer.Deserialize<List<Preset>>(json);
                    if (loaded != null)
                    {
                        foreach (var preset in loaded)
                            Presets.Add(preset);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur chargement presets: {ex.Message}");
            }
        }
    }
}
