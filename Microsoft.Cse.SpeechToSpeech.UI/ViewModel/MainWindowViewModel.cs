using GalaSoft.MvvmLight;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Cse.SpeechToSpeech.UI.Model;
using Microsoft.Cse.SpeechToSpeech.UI.Speech;
using Microsoft.Cse.SpeechToSpeech.UI.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Cse.SpeechToSpeech.UI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AzureSpeechManager azureSpeech;

        private const string endpointIdFileName = "CustomModelEndpointId.txt";
        private const string subscriptionKeyFileName = "SubscriptionKey.txt";

        private string[] regions = { "westus", "westeurope", "eastasia", "northeurope" };
        private Language[] languages = {
            new Language() { Name = "Arabic (Egypt), modern standard", Code = "ar-EG" },
            new Language() { Name = "Catalan (Spain)", Code = "ca-ES" },
            new Language() { Name = "Danish (Denmark)", Code = "da-DK" },
            new Language() { Name = "German (Germany)", Code = "de-DE" },
            new Language() { Name = "English (Australia)", Code = "en-AU" },
            new Language() { Name = "English (Canada)", Code = "en-CA" },
            new Language() { Name = "English (United Kingdom)", Code = "en-GB" },
            new Language() { Name = "English (India)", Code = "en-IN" },
            new Language() { Name = "English (New Zealand)", Code = "en-NZ" },
            new Language() { Name = "English (United States)", Code = "en-US" },
            new Language() { Name = "Spanish (Spain)", Code = "es-ES" },
            new Language() { Name = "Spanish (Mexico)", Code = "ed-MX" },
            new Language() { Name = "Finnish (Finland)", Code = "fi-FI" },
            new Language() { Name = "French (Canada)", Code = "fr-CA" },
            new Language() { Name = "French (France)", Code = "fr-FR" },
            new Language() { Name = "Hindi (India)", Code = "hi-IN" },
            new Language() { Name = "Italian (Italy)", Code = "it-IT" },
            new Language() { Name = "Japanese (Japan)", Code = "ja-JP" },
            new Language() { Name = "Korean (Korea)", Code = "ko-KR" },
            new Language() { Name = "Norwegian (Bokmål) (Norway)", Code = "en-NZ" },
            new Language() { Name = "Dutch (Netherlands)", Code = "nl-NL" },
            new Language() { Name = "Polish (Poland)", Code = "pl-PL" },
            new Language() { Name = "Portuguese (Brazil)", Code = "pt-BR" },
            new Language() { Name = "Portuguese (Portugal)", Code = "pt-PT" },
            new Language() { Name = "Russian (Russia)", Code = "ru-RU" },
            new Language() { Name = "Swedish (Sweden)", Code = "sv-SE" },
            new Language() { Name = "Chinese (Mandarin, simplified)", Code = "zh-CN" },
            new Language() { Name = "Chinese (Mandarin, Traditional)", Code = "zh-HK" },
            new Language() { Name = "Chinese (Taiwanese Mandarin)", Code = "zh-TW" },
            new Language() { Name = "Thai (Thailand)", Code = "th-TH" },
        };

        private VoiceLanguage[] voices = {
            new VoiceLanguage() { Locale = "ar-EG*", Language="Arabic (Egypt)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (ar-EG, Hoda)" },
            new VoiceLanguage() { Locale = "ar-SA", Language="Arabic (Saudi Arabia)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (ar-SA, Naayf)" },
            new VoiceLanguage() { Locale = "bg-BG", Language="Bulgarian", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (bg-BG, Ivan)" },
            new VoiceLanguage() { Locale = "ca-ES", Language="Catalan (Spain)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (ca-ES, HerenaRUS)" },
            new VoiceLanguage() { Locale = "cs-CZ", Language="Czech", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (cs-CZ, Jakub)" },
            new VoiceLanguage() { Locale = "da-DK", Language="Danish", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (da-DK, HelleRUS)"},
            new VoiceLanguage() { Locale = "de-AT", Language="German (Austria)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (de-AT, Michael)"},
            new VoiceLanguage() { Locale = "de-CH", Language="German (Switzerland)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (de-CH, Karsten)" },
            new VoiceLanguage() { Locale = "de-DE", Language="German (Germany)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (de-DE, Hedda)" },
            new VoiceLanguage() { Locale = "de-DE", Language="German (Germany)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (de-DE, HeddaRUS)" },
            new VoiceLanguage() { Locale = "de-DE", Language="German (Germany)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (de-DE, Stefan, Apollo)" },
            new VoiceLanguage() { Locale = "el-GR", Language="Greek", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (el-GR, Stefanos)" },
            new VoiceLanguage() { Locale = "en-AU", Language="English (Australia)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-AU, Catherine)" },
            new VoiceLanguage() { Locale = "en-AU", Language="English (Australia)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-AU, HayleyRUS)" },
            new VoiceLanguage() { Locale = "en-CA", Language="English (Canada)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-CA, Linda)" },
            new VoiceLanguage() { Locale = "en-CA", Language="English (Canada)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-CA, HeatherRUS)" },
            new VoiceLanguage() { Locale = "en-GB", Language="English (UK)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-GB, Susan, Apollo)" },
            new VoiceLanguage() { Locale = "en-GB", Language="English (UK)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-GB, Susan, HazelRUS)" },
            new VoiceLanguage() { Locale = "en-GB", Language="English (UK)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-GB, George, Apollo)" },
            new VoiceLanguage() { Locale = "en-IE", Language="English (Ireland)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-IE, Sean)"},
            new VoiceLanguage() { Locale = "en-IN", Language="English (India)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-IN, Heera, Apollo)" },
            new VoiceLanguage() { Locale = "en-IN", Language="English (India)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-IN, Heera, PriyaRUS)" },
            new VoiceLanguage() { Locale = "en-IN", Language="English (India)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-IN, Ravi, Apollo)" },
            new VoiceLanguage() { Locale = "en-US", Language="English (US)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)" },
            new VoiceLanguage() { Locale = "en-US", Language="English (US)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)" },
            new VoiceLanguage() { Locale = "en-US", Language="English (US)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-US, BenjaminRUS)" },
            new VoiceLanguage() { Locale = "en-US", Language="English (US)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-US, Jessa24kRUS)" },
            new VoiceLanguage() { Locale = "en-US", Language="English (US)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (en-US, Guy24kRUS)" },
            new VoiceLanguage() { Locale = "es-ES", Language="Spanish (Spain)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (es-ES, Laura, Apollo)" },
            new VoiceLanguage() { Locale = "es-ES", Language="Spanish (Spain)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (es-ES, HelenaRUS)" },
            new VoiceLanguage() { Locale = "es-ES", Language="Spanish (Spain)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (es-ES, Pablo, Apollo)" },
            new VoiceLanguage() { Locale = "es-MX", Language="Spanish (Mexico)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (es-MX, HildaRUS)" },
            new VoiceLanguage() { Locale = "es-MX", Language="Spanish (Mexico)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (es-MX, Raul, Apollo)" },
            new VoiceLanguage() { Locale = "fi-FI", Language="Finnish", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (fi-FI, HeidiRUS)" },
            new VoiceLanguage() { Locale = "fr-CA", Language="French (Canada)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (fr-CA, Caroline)" },
            new VoiceLanguage() { Locale = "fr-CA", Language="French (Canada)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (fr-CA, HarmonieRUS)" },
            new VoiceLanguage() { Locale = "fr-CH", Language="French (Switzerland)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (fr-CH, Guillaume)" },
            new VoiceLanguage() { Locale = "fr-FR", Language="French (France)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (fr-FR, Julie, Apollo)" },
            new VoiceLanguage() { Locale = "fr-FR", Language="French (France)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (fr-FR, Julie, HortenseRUS)" },
            new VoiceLanguage() { Locale = "fr-FR", Language="French (France)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (fr-FR, Paul, Apollo)" },
            new VoiceLanguage() { Locale = "he-IL", Language="Hebrew (Israel)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (he-IL, Asaf)" },
            new VoiceLanguage() { Locale = "hi-IN", Language="Hindi (India)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (hi-IN, Kalpana, Apollo)" },
            new VoiceLanguage() { Locale = "hi-IN", Language="Hindi (India)", Gender = "Female", VoiceName = "Microsoft Server Speech Text to Speech Voice (hi-IN, Kalpana)" },
            new VoiceLanguage() { Locale = "hi-IN", Language="Hindi (India)", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (hi-IN, Hemant)" },
            new VoiceLanguage() { Locale = "hr-HR", Language="Croatian", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (hr-HR, Matej)" },
            new VoiceLanguage() { Locale = "hu-HU", Language="Hungarian", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (hu-HU, Szabolcs)" },
            new VoiceLanguage() { Locale = "id-ID", Language="Indonesian", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (id-ID, Andika)" },
            new VoiceLanguage() { Locale = "it-IT", Language="Italian", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (it-IT, Cosimo, Apollo)" },
            new VoiceLanguage() { Locale = "it-IT", Language="Italian", Gender = "Female", VoiceName = "Microsoft Server Speech Text to Speech Voice (it-IT, LuciaRUS)" },
            new VoiceLanguage() { Locale = "ja-JP", Language="Japanese", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (ja-JP, Ayumi, Apollo)"},
            new VoiceLanguage() { Locale = "ja-JP", Language="Japanese", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (ja-JP, Ichiro, Apollo)"},
            new VoiceLanguage() { Locale = "ja-JP", Language="Japanese", Gender = "Female", VoiceName = "Microsoft Server Speech Text to Speech Voice (ja-JP, HarukaRUS)"},
            new VoiceLanguage() { Locale = "ko-KR", Language="Korean", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (ko-KR, HeamiRUS)"},
            new VoiceLanguage() { Locale = "ms-MY", Language="Malay", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (ms-MY, Rizwan)" },
            new VoiceLanguage() { Locale = "nb-NO", Language="Norwegian", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (nb-NO, HuldaRUS)"},
            new VoiceLanguage() { Locale = "nl-NL", Language="Dutch", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (nl-NL, HannaRUS)" },
            new VoiceLanguage() { Locale = "pl-PL", Language="Polish", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (pl-PL, PaulinaRUS)" },
            new VoiceLanguage() { Locale = "pt-BR", Language="Portuguese (Brazil)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (pt-BR, HeloisaRUS)" },
            new VoiceLanguage() { Locale = "pt-BR", Language="Portuguese (Brazil)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (pt-BR, HeloisaRUS)" },
            new VoiceLanguage() { Locale = "pt-PT", Language="Portuguese (Portugal)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (pt-PT, HeliaRUS)" },
            new VoiceLanguage() { Locale = "ro-RO", Language="Romanian", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (ro-RO, Andrei)" },
            new VoiceLanguage() { Locale = "ru-RU", Language="Russian", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (ru-RU, Irina, Apollo)" },
            new VoiceLanguage() { Locale = "ru-RU", Language="Russian", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (ru-RU, Pavel, Apollo)" },
            new VoiceLanguage() { Locale = "ru-RU", Language="Russian", Gender = "Female", VoiceName = "Microsoft Server Speech Text to Speech Voice (ru-RU, EkaterinaRUS)" },
            new VoiceLanguage() { Locale = "sk-SK", Language="Slovak", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (sk-SK, Filip)" },
            new VoiceLanguage() { Locale = "sl-SI", Language="Slovenian", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (sl-SI, Lado)" },
            new VoiceLanguage() { Locale = "sv-SE", Language="Swedish", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (sv-SE, HedvigRUS)" },
            new VoiceLanguage() { Locale = "ta-IN", Language="Tamil (India)", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (ta-IN, Valluvar)" },
            new VoiceLanguage() { Locale = "te-IN", Language="Telugu (India)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (te-IN, Chitra)" },
            new VoiceLanguage() { Locale = "th-TH", Language="Thai", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (th-TH, Pattara)" },
            new VoiceLanguage() { Locale = "tr-TR", Language="Turkish", Gender = "Female", VoiceName = "Microsoft Server Speech Text to Speech Voice (tr-TR, SedaRUS)" },
            new VoiceLanguage() { Locale = "vi-VN", Language="Vietnamese", Gender = "Male", VoiceName = "Microsoft Server Speech Text to Speech Voice (vi-VN, An)" },
            new VoiceLanguage() { Locale = "zh-CN", Language="Chinese (Mainland)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-CN, HuihuiRUS)" },
            new VoiceLanguage() { Locale = "zh-CN", Language="Chinese (Mainland)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-CN, Yaoyao, Apollo)"},
            new VoiceLanguage() { Locale = "zh-CN", Language="Chinese (Mainland)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-CN, Kangkang, Apollo)" },
            new VoiceLanguage() { Locale = "zh-HK", Language="Chinese (Hong Kong)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-HK, Tracy, Apollo)" },
            new VoiceLanguage() { Locale = "zh-HK", Language="Chinese (Hong Kong)", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-HK, TracyRUS)" },
            new VoiceLanguage() { Locale = "zh-HK", Language="Chinese (Hong Kong)", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-HK, Danny, Apollo)" },
            new VoiceLanguage() { Locale = "zh-TW", Language="Arabic", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-TW, Yating, Apollo)" },
            new VoiceLanguage() { Locale = "zh-TW", Language="Arabic", Gender = "Female", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-TW, HanHanRUS)" },
            new VoiceLanguage() { Locale = "zh-TW", Language="Arabic", Gender = "Male", VoiceName ="Microsoft Server Speech Text to Speech Voice (zh-TW, Zhiwei, Apollo)" }
        };

        private string subscriptionKey;
        private string customModelEndpointId;
        private Language language;
        private Language translationLanguage;
        private VoiceLanguage selectedVoice;
        private string partialOutput;
        private string debugOutput;
        private string lastOutput;
        private string lastOutputFilename;
        private InputSourceType inputSource = InputSourceType.Microphone;
        private string wavInputFilename;

        public MainWindowViewModel()
        {
            PartialOutput = string.Empty;
            DebugOutput = string.Empty;
            LoadKeys();
            PropertyChanged += OnSpeechModelPropertyChanged;

            ShowSpeechApiConfigOnStartup = string.IsNullOrEmpty(SubscriptionKey);
        }

        public string SubscriptionKey { get => subscriptionKey; set => Set(nameof(SubscriptionKey), ref subscriptionKey, value); }
        public string CustomModelEndpointId { get => customModelEndpointId; set => Set(nameof(CustomModelEndpointId), ref customModelEndpointId, value); }
        public Language Language { get => language; set => Set(nameof(Language), ref language, value); }
        public string PartialOutput { get => partialOutput; set => Set(nameof(PartialOutput), ref partialOutput, value); }
        public string DebugOutput { get => debugOutput; set => Set(nameof(DebugOutput), ref debugOutput, value); }
        public Language TranslationLanguage { get => translationLanguage; set => Set(nameof(TranslationLanguage), ref translationLanguage, value); }
        public string LastOutput { get => lastOutput; set => Set(nameof(LastOutput), ref lastOutput, value); }
        public string LastOutputFilename { get => lastOutputFilename; set => Set(nameof(LastOutputFilename), ref lastOutputFilename, value); }
        public VoiceLanguage SelectedVoice { get => selectedVoice; set => Set(nameof(SelectedVoice), ref selectedVoice, value); }
        public InputSourceType InputSource { get => inputSource; set => Set(nameof(InputSource), ref inputSource, value); }
        public string WavInputFilename { get => wavInputFilename; set => Set(nameof(WavInputFilename), ref wavInputFilename, value); }

        public bool ShowSpeechApiConfigOnStartup { get; }


        public IEnumerable<Language> Languages { get => languages; }
        public IEnumerable<string> Regions { get => regions; }
        public IEnumerable<VoiceLanguage> Voices { get => voices; }

        public bool IsRecognizerRunning => azureSpeech != null;

        private void LoadKeys()
        {
            CustomModelEndpointId = IsolatedStorageManager.GetValueFromIsolatedStorage(endpointIdFileName);
            SubscriptionKey = IsolatedStorageManager.GetValueFromIsolatedStorage(subscriptionKeyFileName);
        }

        private void OnSpeechModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionKey" || e.PropertyName == "CustomModelEndpointId")
            {
                SaveAzureSpeechKey();
            }
        }

        private void AppendDebug(string message)
        {
            DebugOutput = string.Concat(DebugOutput, Environment.NewLine, message);
        }

        private void SaveAzureSpeechKey()
        {
            if (!string.IsNullOrEmpty(SubscriptionKey))
            {
                IsolatedStorageManager.SaveKeyToIsolatedStorage(subscriptionKeyFileName, SubscriptionKey);
            }

            if (!string.IsNullOrEmpty(CustomModelEndpointId))
            {
                IsolatedStorageManager.SaveKeyToIsolatedStorage(endpointIdFileName, CustomModelEndpointId);
            }
        }

        private void OnAzureSpeechTranslationSynthesizing(object sender, TranslationSynthesisEventArgs e)
        {
            byte[] buffer = e.Result.GetAudio();
            if (buffer.Length == 0)
            {
                return;
            }

            string tempFilename = Path.GetTempFileName();

            string outputFileName = Path.ChangeExtension(tempFilename, ".wav");
            File.Delete(tempFilename); // clean up .tmp file
            using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buffer, 0, buffer.Length);
            }

            AppendDebug($"Saved output wave file in {outputFileName}");

            SetOutputFilename(outputFileName);
        }

        private void SetOutputFilename(string outputFileName)
        {
            var previousFile = LastOutputFilename;
            LastOutputFilename = outputFileName;

            if (!string.IsNullOrEmpty(previousFile))
            {
                File.Delete(previousFile); // clean up old wav file
            }
        }

        private void OnAzureSpeechNotification(object sender, NotificationEventArgs e)
        {
            AppendDebug(e.Message);
        }

        private void OnAzureSpeechRecognizing(object sender, TranslationRecognitionEventArgs e)
        {
            LastOutput = e.ToString();
            PartialOutput = string.Concat(PartialOutput, e.ToString(), Environment.NewLine);
        }

        public async Task StartRecognizer()
        {
            try
            {
                if (azureSpeech == null)
                {
                    if (string.IsNullOrEmpty(CustomModelEndpointId))
                    {
                        AppendDebug("CustomModelEndpointId must be specified");
                        return;
                    }
                    if (string.IsNullOrEmpty(SubscriptionKey))
                    {
                        AppendDebug("SubscriptionKey must be specified");
                        return;
                    }
                    azureSpeech = new AzureSpeechManager();
                    azureSpeech.Recognizing += OnAzureSpeechRecognizing;
                    azureSpeech.Notification += OnAzureSpeechNotification;
                    azureSpeech.TranslationSynthesizing += OnAzureSpeechTranslationSynthesizing;
                    azureSpeech.Initialize(SubscriptionKey, CustomModelEndpointId, inputSource, wavInputFilename);

                }
                if (Language == null)
                {
                    AppendDebug("Language must be specified");
                    return;
                }
                if (TranslationLanguage == null)
                {
                    AppendDebug("Translation language must be specified");
                    return;
                }
                if (SelectedVoice == null)
                {
                    AppendDebug("Voice must be specified");
                    return;
                }

                azureSpeech.SetSpeechLanguage(Language.Code, TranslationLanguage.Code, SelectedVoice.VoiceName);
                await azureSpeech.CreateTranslationRecognizer();
                await azureSpeech.StartContinuousRecognitionAsync();
            }
            catch (Exception ex)
            {
                AppendDebug($"Exception: {ex.Message}");
            }
        }
        public async Task StopRecognizer()
        {
            try
            {
                if (azureSpeech != null)
                {
                    await azureSpeech.StopContinuousRecognitionAsync();
                    azureSpeech.Recognizing -= OnAzureSpeechRecognizing;
                    azureSpeech.Notification -= OnAzureSpeechNotification;
                    azureSpeech.TranslationSynthesizing -= OnAzureSpeechTranslationSynthesizing;
                    azureSpeech.Dispose();
                    azureSpeech = null;
                }
                SetOutputFilename(null); // clean up file
            }
            catch (Exception ex)
            {
                AppendDebug($"Exception: {ex.Message}");
            }
        }
    }
}
