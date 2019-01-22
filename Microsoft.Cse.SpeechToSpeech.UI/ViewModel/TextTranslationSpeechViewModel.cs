namespace Microsoft.Cse.SpeechToSpeech.UI.ViewModel
{
    using GalaSoft.MvvmLight;
    using Microsoft.Cse.SpeechToSpeech.UI.Model;
    using Microsoft.Cse.SpeechToSpeech.UI.Storage;
    using Microsoft.Cse.SpeechToSpeech.UI.Translation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class TextTranslationSpeechViewModel : ViewModelBase
    {
        private const string subscriptionKeyFileName = "TranslationSubscriptionKey.txt";

        private Language language;
        private Language translationLanguage;
        private VoiceLanguage selectedVoice;
        private string subscriptionKey;
        private string text;
        private string translatedText;
        private string debugOutput;

        public TextTranslationSpeechViewModel()
        {
            DebugOutput = string.Empty;
            LoadKeys();
            PropertyChanged += OnTextTranslationSpeechViewModelPropertyChanged;
        }

        public string SubscriptionKey { get => subscriptionKey; set => Set(nameof(SubscriptionKey), ref subscriptionKey, value); }
        public Language Language { get => language; set => Set(nameof(Language), ref language, value); }
        public Language TranslationLanguage { get => translationLanguage; set => Set(nameof(TranslationLanguage), ref translationLanguage, value); }
        public VoiceLanguage SelectedVoice { get => selectedVoice; set => Set(nameof(SelectedVoice), ref selectedVoice, value); }
        public string Text { get => text; set => Set(nameof(Text), ref text, value); }
        public string TranslatedText { get => translatedText; set => Set(nameof(TranslatedText), ref translatedText, value); }
        public string DebugOutput { get => debugOutput; set => Set(nameof(DebugOutput), ref debugOutput, value); }
        public string CustomModelEndpointId { get; private set; }

        public IEnumerable<Language> Languages { get => LanguageList.Languages; }
        public IEnumerable<string> Regions { get => RegionList.Regions; }
        public IEnumerable<VoiceLanguage> Voices { get => VoiceList.Voices; }

        public async Task Translate()
        {
            TranslationClient client = new TranslationClient(subscriptionKey);

            List<TranslationResult> result = await client.Translate(language, translationLanguage, Text);
            if (result != null && result.Count > 0)
            {
                AppendDebug("Translated text is: ");
                foreach (var tranlationResult in result)
                {
                    if (tranlationResult.IsSuccess)
                    {
                        foreach (var item in tranlationResult.Translations)
                        {
                            AppendDebug($"Language: {item.LanguageCode} -> {item.Text}");
                        }
                    }
                    else
                    {
                        AppendDebug($"Error {tranlationResult.Error}");
                    }
                }
            }
        }

        private void OnTextTranslationSpeechViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionKey")
            {
                SaveAzureSpeechKey();
            }
        }

        private void LoadKeys()
        {
            SubscriptionKey = IsolatedStorageManager.GetValueFromIsolatedStorage(subscriptionKeyFileName);
        }

        private void SaveAzureSpeechKey()
        {
            if (!string.IsNullOrEmpty(SubscriptionKey))
            {
                IsolatedStorageManager.SaveKeyToIsolatedStorage(subscriptionKeyFileName, SubscriptionKey);
            }
        }

        private void AppendDebug(string message)
        {
            DebugOutput = string.Concat(DebugOutput, Environment.NewLine, message);
        }
    }
}
