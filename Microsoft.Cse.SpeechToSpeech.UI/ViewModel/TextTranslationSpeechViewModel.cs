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
        private string requestUri = "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1";

        private Language language;
        private Language translationLanguage;
        private VoiceLanguage selectedVoice;
        private List<string> audioFormat;
        private string selectedAudioOutputFormat;
        private string text;
        private string translatedText;
        private string debugOutput;
        private string accessToken = null;
        private Synthesize cortana;
        private AppSettings settings;

        public TextTranslationSpeechViewModel()
        {
            DebugOutput = string.Empty;
            settings = IsolatedStorageSettings.Settings;
            audioFormat = new List<string>();
            audioFormat.AddRange(Enum.GetNames(typeof(AudioOutputFormat)));
            SelectedAudioOutputFormat = Enum.GetName(typeof(AudioOutputFormat), AudioOutputFormat.Riff16Khz16BitMonoPcm);
            ShowTranslationApiConfigOnStartup = string.IsNullOrEmpty(settings.SpeechSubscriptionKey)
                                                || string.IsNullOrEmpty(settings.TextTranslationSubscriptionKey);
        }

        public AppSettings Settings { get => settings; }
        public Language Language { get => language; set => Set(nameof(Language), ref language, value); }
        public Language TranslationLanguage { get => translationLanguage; set => Set(nameof(TranslationLanguage), ref translationLanguage, value); }
        public VoiceLanguage SelectedVoice { get => selectedVoice; set => Set(nameof(SelectedVoice), ref selectedVoice, value); }
        public string Text { get => text; set => Set(nameof(Text), ref text, value); }
        public string TranslatedText { get => translatedText; set => Set(nameof(TranslatedText), ref translatedText, value); }
        public string DebugOutput { get => debugOutput; set => Set(nameof(DebugOutput), ref debugOutput, value); }
        public List<string> AudioFormat { get => audioFormat; set => Set(nameof(AudioFormat), ref audioFormat, value); }
        public string SelectedAudioOutputFormat { get => selectedAudioOutputFormat; set => Set(nameof(SelectedAudioOutputFormat), ref selectedAudioOutputFormat, value); }

        public IEnumerable<Language> Languages { get => LanguageList.Languages; }
        public IEnumerable<string> Regions { get => RegionList.Regions; }
        public IEnumerable<VoiceLanguage> Voices { get => VoiceList.Voices; }
        public bool ShowTranslationApiConfigOnStartup { get; }
        public string AudioOutputFolder { get; set; }

        public async Task Translate()
        {
            TranslationClient client = new TranslationClient(Settings.TextTranslationSubscriptionKey);

            List<TranslationResult> result = await client.Translate(language, translationLanguage, Text);
            if (result != null && result.Count > 0)
            {
                AppendDebug("Translated text is: ");
                foreach (var translationResult in result)
                {
                    if (translationResult.IsSuccess)
                    {
                        foreach (var item in translationResult.Translations)
                        {
                            AppendDebug($"Language: {item.LanguageCode} -> {item.Text}");
                            TranslatedText = item.Text;
                        }
                    }
                    else
                    {
                        AppendDebug($"Error {translationResult.Error}");
                        TranslatedText = null;
                    }
                }
            }


            if (TranslatedText != null)
            {
                Authentication auth = new Authentication(Settings.SpeechSubscriptionKey);
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
                OutputFormat = (AudioOutputFormat)Enum.Parse(typeof(AudioOutputFormat), SelectedAudioOutputFormat, false),
                AuthorizationToken = "Bearer " + accessToken,
            });

            cortana.Error += OnSynthesizeError;
            cortana.AudioAvailable += OnAudioAvailable;
            cortana.Speak(CancellationToken.None);
        }

        private void OnAudioAvailable(object sender, GenericEventArgs<byte[]> e)
        {
            string tempFilename = Path.GetTempFileName();

            if (!string.IsNullOrEmpty(AudioOutputFolder) && Directory.Exists(AudioOutputFolder))
            {
                tempFilename = Path.Combine(AudioOutputFolder, Path.GetFileName(tempFilename));
            }

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

        private void AppendDebug(string message)
        {
            DebugOutput = string.Concat(DebugOutput, Environment.NewLine, message);
        }
    }
}
