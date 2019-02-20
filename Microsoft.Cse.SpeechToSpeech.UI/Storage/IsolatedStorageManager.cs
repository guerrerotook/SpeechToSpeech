namespace Microsoft.Cse.SpeechToSpeech.UI.Storage
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;
    using System.Threading;

    public static class IsolatedStorageSettings
    {
        private const string SettingsFileName = "settings.json";
        private const IsolatedStorageScope Scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;

        private static Lazy<AppSettings> settingsLazy = new Lazy<AppSettings>(LoadSettings, LazyThreadSafetyMode.PublicationOnly);
        public static AppSettings Settings
        {
            get => settingsLazy.Value;
        }
        public static void Save()
        {
            if (settingsLazy.IsValueCreated)
            {
                SaveSettings(settingsLazy.Value);
            }
        }

        private static AppSettings LoadSettings()
        {
            AppSettings settings = null;
            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetStore(Scope, null, null))
            {
                if (isolatedStorage.FileExists(SettingsFileName))
                {
                    var serializer = JsonSerializer.CreateDefault();
                    using (var stream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Open, isolatedStorage))
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        settings = serializer.Deserialize<AppSettings>(jsonReader);
                    }
                }
            }
            if (settings == null)
            {
                settings = new AppSettings();
            }
            // wire up auto-saving settings :-)
            settings.PropertyChanged += (sender, e) => SaveSettings(settings);

            return settings;
        }


        private static void SaveSettings(AppSettings settings)
        {
            var serializer = JsonSerializer.CreateDefault();
            using (var isolatedStorage = IsolatedStorageFile.GetStore(Scope, null, null))
            using (var stream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Create, isolatedStorage))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}