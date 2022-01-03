using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using EncryptedMessanger.Modules;
using EncryptedMessanger.Data;
using Microsoft.Maui.Essentials;
using System.IO;

namespace EncryptedMessanger.Handlers
{
    public class SettingsHandler {
        public Settings Settings { get; private set; }
        private const string settingsFileName = "SettingsData.emm";
        private readonly string path = FileSystem.AppDataDirectory;
        private readonly string path_name;

        public SettingsHandler() {
            path_name = Path.Combine(path, settingsFileName);
            ReadSettingsFromFile();
        }


        // Read
        private async void ReadSettingsFromFile() {

            if (!File.Exists(path_name)) Create();

            var bytes = await File.ReadAllBytesAsync(path_name);
            await Settings.Deserialize(bytes);
        }

        // Write
        private async void WriteSettingsToFile() {
            var bytes = Settings.Serialize();
            await File.WriteAllBytesAsync(path_name, bytes);
        }
        // Serialize
        private static void Serialize(Settings settings) {
            var strData = JsonSerializer.Serialize(settings);
            var bytes = Encoding.UTF8.GetBytes(strData);
        }
        // Deserialize
        private static Settings Deserialize(byte[] bytes) { 
            var strData = Encoding.UTF8.GetString(bytes);
            Settings settings = JsonSerializer.Deserialize<Settings>(strData);
            return settings;
        }
        // Create
        private void Create() {
            Settings = new Settings();
            WriteSettingsToFile();
        }
    }
}
