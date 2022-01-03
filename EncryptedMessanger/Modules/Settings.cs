using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EncryptedMessanger.Modules
{
    public class Settings
    {
#nullable enable
        public int? ContactId { get; set; }
        public string? NickName { get; set; }
#nullable disable
    }

    public static class SettingsExtensions
    {
        public static byte[] Serialize(this Settings settings) {
            var strData = JsonSerializer.Serialize(settings);
            var bytes = Encoding.UTF8.GetBytes(strData);
            return bytes;
        }
        public static Task Deserialize(this Settings settings, byte[] bytes) {
            if (settings == null) { settings = new Settings(); }
            var strData = Encoding.UTF8.GetString(bytes);
            settings = JsonSerializer.Deserialize<Settings>(strData);
            return Task.CompletedTask;
        }

    }
}
