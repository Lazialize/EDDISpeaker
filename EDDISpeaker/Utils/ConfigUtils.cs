using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EDDISpeaker.Utils
{
    public static class ConfigUtils
    {
        private const string ConfigPath = @".\config";
        private const string ConfigExtension = ".json";

        public static T Load<T>(string fileName)
        {
            var filePath = Path.Combine(ConfigPath, fileName + ConfigExtension);

            if (!File.Exists(fileName))
            {
                Save(Activator.CreateInstance<T>(), fileName);
            }

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var buffer = new byte[stream.Length];
            stream.Read(buffer);

            var reader = new Utf8JsonReader(new ReadOnlySequence<byte>(buffer));
            var result = JsonSerializer.Deserialize<T>(ref reader);
            return result;
        }

        public static void Save<T>(T config, string fileName)
        {
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
            }

            var filePath = Path.Combine(ConfigPath, config.GetType().Name + ConfigExtension);
            using var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            var options = new JsonWriterOptions() { Indented = true };
            using var writer = new Utf8JsonWriter(stream, options);
            JsonSerializer.Serialize(writer, config);
        }
    }
}
