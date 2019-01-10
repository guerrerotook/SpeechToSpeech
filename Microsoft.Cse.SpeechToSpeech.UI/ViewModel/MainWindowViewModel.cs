using GalaSoft.MvvmLight;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Cse.SpeechToSpeech.UI.Model;
using Microsoft.Cse.SpeechToSpeech.UI.Speech;
using Microsoft.Cse.SpeechToSpeech.UI.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            new Language() { Name = "English", Code = "en-US" },
            new Language() { Name = "Arabic", Code = "ar-EG" },
            new Language() { Name = "Chinese (Mandarin)", Code = "zh-CN" },
            new Language() { Name = "French", Code = "fr-FR" },
            new Language() { Name = "German", Code = "de-DE" },
            new Language() { Name = "Italian", Code = "It-IT" },
            new Language() { Name = "Japanese", Code = "ja-JA" },
            new Language() { Name = "Portuguese", Code = "pt-BR" },
            new Language() { Name = "Russian", Code = "ru-RU" },
            new Language() { Name = "Spanish", Code = "es-ES" }
        };

        private string subscriptionKey;
        private string customModelEndpointId;
        private Language language;
        private Language translationLanguage;
        private string partialOutput;
        private string debugOutput;
        private string lastOutput;

        public MainWindowViewModel()
        {
            PartialOutput = string.Empty;
            DebugOutput = string.Empty;
            LoadKeys();
            PropertyChanged += OnSpeechModelPropertyChanged;

        }

        public string SubscriptionKey { get => subscriptionKey; set => Set(nameof(SubscriptionKey), ref subscriptionKey, value); }
        public string CustomModelEndpointId { get => customModelEndpointId; set => Set(nameof(CustomModelEndpointId), ref customModelEndpointId, value); }
        public Language Language { get => language; set => Set(nameof(Language), ref language, value); }
        public string PartialOutput { get => partialOutput; set => Set(nameof(PartialOutput), ref partialOutput, value); }
        public string DebugOutput { get => debugOutput; set => Set(nameof(DebugOutput), ref debugOutput, value); }
        public Language TranslationLanguage { get => translationLanguage; set => Set(nameof(TranslationLanguage), ref translationLanguage, value); }

        public IEnumerable<Language> Languages { get => languages; }
        public IEnumerable<string> Regions { get => regions; }

        public bool IsRecoznizingRunning
        {
            get
            {
                if (azureSpeech != null)
                {
                    return azureSpeech.IsSessionStarted;
                }
                else
                {
                    return false;
                }
            }
        }
        public string LastOutput { get => lastOutput; set => Set(nameof(LastOutput), ref lastOutput, value); }

        private void LoadKeys()
        {
            CustomModelEndpointId = IsolatedStorageManager.GetValueFromIsolatedStorage(endpointIdFileName);
            SubscriptionKey = IsolatedStorageManager.GetValueFromIsolatedStorage(subscriptionKeyFileName);

            if (!string.IsNullOrEmpty(CustomModelEndpointId) && !string.IsNullOrEmpty(SubscriptionKey))
            {
                CreateAzureSpeechManager(SubscriptionKey, CustomModelEndpointId);

            }
        }

        private void OnSpeechModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionKey" || e.PropertyName == "CustomModelEndpointId")
            {
                SaveAzureSpeechKey();
            }
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

        private void CreateAzureSpeechManager(string subcription, string region)
        {
            azureSpeech = new AzureSpeechManager();
            azureSpeech.Recognizing += OnAzureSpeechRecognizing;
            azureSpeech.Notification += OnAzureSpeechNotification;
            azureSpeech.Initialize(subcription, region);
        }

        private void DestoyAzureSpeechManager()
        {
            azureSpeech.Recognizing -= OnAzureSpeechRecognizing;
            azureSpeech.Notification -= OnAzureSpeechNotification;
            azureSpeech.Dispose();
            azureSpeech = null;
        }

        private void OnAzureSpeechNotification(object sender, NotificationEventArgs e)
        {
            DebugOutput = string.Concat(DebugOutput, Environment.NewLine, e.Message);
        }

        private void OnAzureSpeechRecognizing(object sender, TranslationRecognitionEventArgs e)
        {
            LastOutput = e.ToString();
            PartialOutput = string.Concat(PartialOutput, e.ToString(), Environment.NewLine);
        }

        public async Task StartRecognizer()
        {
            if (azureSpeech == null)
            {
                if (!string.IsNullOrEmpty(CustomModelEndpointId) && !string.IsNullOrEmpty(SubscriptionKey))
                {
                    CreateAzureSpeechManager(SubscriptionKey, CustomModelEndpointId);
                }
            }

            if (azureSpeech != null)
            {
                azureSpeech.SetSpeechLanguage(Language.Code, TranslationLanguage.Code);
                await azureSpeech.CreateTranslationRecognizer();
                await azureSpeech.StartContinuousRecognitionAsync();
            }
        }

        public async Task StopRecognizer()
        {
            if (azureSpeech != null)
            {
                await azureSpeech.StopContinuousRecognitionAsync();
                DestoyAzureSpeechManager();
            }
        }
    }
}
