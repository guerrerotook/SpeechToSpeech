﻿namespace Microsoft.Cse.SpeechToSpeech.UI.ViewModel
{
    using GalaSoft.MvvmLight;
    using Microsoft.CognitiveServices.Speech.Translation;
    using Microsoft.Cse.SpeechToSpeech.UI.Model;
    using Microsoft.Cse.SpeechToSpeech.UI.Speech;
    using Microsoft.Cse.SpeechToSpeech.UI.Storage;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    public class TranslationSpeechViewModel : ViewModelBase
    {
        private const string endpointIdFileName = "CustomModelEndpointId.txt";
        private const string subscriptionKeyFileName = "SubscriptionKey.txt";

        private AzureSpeechManager azureSpeech;
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

        public TranslationSpeechViewModel()
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
        public string AudioOutputFolder { get; set; }

        public IEnumerable<Language> Languages { get => LanguageList.Languages; }
        public IEnumerable<string> Regions { get => RegionList.Regions; }
        public IEnumerable<VoiceLanguage> Voices { get => VoiceList.Voices; }

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

            if (!string.IsNullOrEmpty(AudioOutputFolder) && Directory.Exists(AudioOutputFolder))
            {
                tempFilename = Path.Combine(AudioOutputFolder, Path.GetFileName(tempFilename));
            }

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
                AppendDebug($"Exception: {ex.ToString()}");
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
