namespace Microsoft.Cse.SpeechToSpeech.UI.ViewModel
{
    using GalaSoft.MvvmLight;
    using Microsoft.Cse.SpeechToSpeech.UI.Model;
    using Microsoft.Cse.SpeechToSpeech.UI.Storage;
    using Microsoft.Cse.SpeechToSpeech.UI.TextToSpeech;
    using Microsoft.Cse.SpeechToSpeech.UI.Translation;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
       
    public class TextTranslationSpeechViewModel : ViewModelBase
    {
        private const string subscriptionKeyFileName = "TranslationSubscriptionKey.txt";
        private const string speechSubscriptionKeyFileName = "TranslationSpeechSubscriptionKey.txt";
        private string requestUri = "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1";

        private Language language;
        private Language translationLanguage;
        private VoiceLanguage selectedVoice;
        private string subscriptionKey;
        private string speechSubscriptionKey;
        private string text;
        private string translatedText;
        private string debugOutput;
        private string accessToken = null;
        private Synthesize cortana;

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
        public string SpeechSubscriptionKey { get => speechSubscriptionKey; set => Set(nameof(SpeechSubscriptionKey), ref speechSubscriptionKey, value); }
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
                            TranslatedText = item.Text;
                        }
                    }
                    else
                    {
                        AppendDebug($"Error {tranlationResult.Error}");
                    }
                }
            }



            Authentication auth = new Authentication(SpeechSubscriptionKey);
            await auth.RenewAuthenticationToken();
            try
            {
                accessToken = auth.GetAccessToken();
                AppendDebug($"Access Token {accessToken}");
            }
            catch (Exception ex)
            {
                AppendDebug($"Error {ex.ToString()}");
            }
           
            ExecuteTextToSpeech(TranslatedText, SelectedVoice);
        }

        private void ExecuteTextToSpeech(string text, VoiceLanguage voice)
        {
            if (cortana != null)
            {
                cortana.Error -= OnSynthesizeError;
                cortana.AudioAvailable -= OnAudioAvailable;

            }

            cortana = new Synthesize(new Synthesize.InputOptions()
            {
                RequestUri = new Uri(requestUri),
                Text = text,
                VoiceType = voice.Gender,
                Locale = voice.Locale,
                VoiceName = voice.VoiceName,
                OutputFormat = AudioOutputFormat.Raw8Khz8BitMonoMULaw,
                AuthorizationToken = "Bearer " + accessToken,
            });

            cortana.Error += OnSynthesizeError;
            cortana.AudioAvailable += OnAudioAvailable;
            cortana.Speak(CancellationToken.None);
        }

        private void OnAudioAvailable(object sender, GenericEventArgs<byte[]> e)
        {
            string tempFilename = Path.GetTempFileName();

            string outputFileName = Path.ChangeExtension(tempFilename, ".wav");
            File.Delete(tempFilename); // clean up .tmp file
            using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(e.EventData, 0, e.EventData.Length);
            }

            AppendDebug($"Saved output wave file in {outputFileName}");
        }

        private void OnSynthesizeError(object sender, GenericEventArgs<Exception> e)
        {
            AppendDebug(e.EventData.ToString());
        }

        private void OnTextTranslationSpeechViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionKey" || e.PropertyName == "SpeechSubscriptionKey")
            {
                SaveAzureSpeechKey();
            }
        }

        private void LoadKeys()
        {
            SubscriptionKey = IsolatedStorageManager.GetValueFromIsolatedStorage(subscriptionKeyFileName);
            SpeechSubscriptionKey = IsolatedStorageManager.GetValueFromIsolatedStorage(speechSubscriptionKeyFileName);
        }

        private void SaveAzureSpeechKey()
        {
            if (!string.IsNullOrEmpty(SubscriptionKey))
            {
                IsolatedStorageManager.SaveKeyToIsolatedStorage(subscriptionKeyFileName, SubscriptionKey);
            }

            if (!string.IsNullOrEmpty(SpeechSubscriptionKey))
            {
                IsolatedStorageManager.SaveKeyToIsolatedStorage(speechSubscriptionKeyFileName, SpeechSubscriptionKey);
            }
        }

        private void AppendDebug(string message)
        {
            DebugOutput = string.Concat(DebugOutput, Environment.NewLine, message);
        }
    }
}
