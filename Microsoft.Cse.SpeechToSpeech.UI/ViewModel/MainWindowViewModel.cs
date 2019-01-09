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



        private string subscriptionKey;
        private string customModelEndpointId;
        private string language;
        private string partialOutput;
        private string debugOutput;

        public string SubscriptionKey { get => subscriptionKey; set => Set(nameof(SubscriptionKey), ref subscriptionKey, value); }
        public string CustomModelEndpointId { get => customModelEndpointId; set => Set(nameof(CustomModelEndpointId), ref customModelEndpointId, value); }
        public string Language { get => language; set => Set(nameof(Language), ref language, value); }
        public string PartialOutput { get => partialOutput; set => Set(nameof(PartialOutput), ref partialOutput, value); }
        public string DebugOutput { get => debugOutput; set => Set(nameof(DebugOutput), ref debugOutput, value); }

        public MainWindowViewModel()
        {
            PartialOutput = string.Empty;
            DebugOutput = string.Empty;
            LoadKeys();
            PropertyChanged += OnSpeechModelPropertyChanged;
            Language = "en-US";

        }

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

        private void OnAzureSpeechNotification(object sender, NotificationEventArgs e)
        {
            DebugOutput = string.Concat(DebugOutput, Environment.NewLine, e.Message);
        }

        private void OnAzureSpeechRecognizing(object sender, TranslationRecognitionEventArgs e)
        {
            PartialOutput = string.Concat(PartialOutput, Environment.NewLine, e.ToString());
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
                azureSpeech.SetSpeechLanguage(Language);
                await azureSpeech.CreateTranslationRecognizer();
                await azureSpeech.StartContinuousRecognitionAsync();
            }
        }

        public async Task StopRecognizer()
        {
            if (azureSpeech != null)
            {
                await azureSpeech.StopContinuousRecognitionAsync();
            }
        }
    }
}
