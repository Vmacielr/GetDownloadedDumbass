using System.IO;
using Newtonsoft.Json;
using GetDownloadedDumbass.Models;

namespace GetDownloadedDumbass.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath = "options.json";

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    var defaults = new AppSettings();
                    Save(defaults);
                    return defaults;
                }

                string json = File.ReadAllText(_settingsPath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            try
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}