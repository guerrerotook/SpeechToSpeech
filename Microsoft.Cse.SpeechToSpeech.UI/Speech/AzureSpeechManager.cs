namespace Microsoft.Cse.SpeechToSpeech.UI.Speech
{
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
    using Microsoft.CognitiveServices.Speech.Translation;
    using Microsoft.Cse.SpeechToSpeech.UI;
    using Microsoft.Cse.SpeechToSpeech.UI.Model;
    using System;
    using System.Threading.Tasks;

    public class AzureSpeechManager : IDisposable
    {
        private SpeechTranslationConfig speechConfiguration;
        private AudioConfig audioConfig;
        private TranslationRecognizer recognizer;
        public bool IsSessionStarted { get; set; }
        public bool IsSessionStopped { get; set; }
        public string SessionId { get; set; }

        public event EventHandler<TranslationRecognitionEventArgs> Recognizing;
        public event EventHandler<NotificationEventArgs> Notification;
        public event EventHandler<TranslationSynthesisEventArgs> TranslationSynthesizing;

        public AzureSpeechManager()
        {

        }

        public void Initialize(string subscriptionKey, string region, InputSourceType inputSource, string wavFilename)
        {
            subscriptionKey.EnsureIsNotNull(nameof(subscriptionKey));
            subscriptionKey.EnsureIsNotNull(nameof(region));

            speechConfiguration = SpeechTranslationConfig.FromSubscription(subscriptionKey, region);
            speechConfiguration.OutputFormat = OutputFormat.Detailed;
            SendMessage($"Created the SpeechConfiguration with {subscriptionKey} | {region}");

            audioConfig = GetAudioConfig(inputSource, wavFilename);
        }

        public void SetSpeechLanguage(string language, string translationLanguage, string voice)
        {
            speechConfiguration.SpeechRecognitionLanguage = language;
            speechConfiguration.AddTargetLanguage(translationLanguage);
            speechConfiguration.VoiceName = voice;
        }

        public async Task CreateTranslationRecognizer()
        {
            if (recognizer != null)
            {
                await recognizer.StopContinuousRecognitionAsync();
                await recognizer.StopKeywordRecognitionAsync();
            }

            recognizer = new TranslationRecognizer(speechConfiguration, audioConfig);
            recognizer.Recognizing += OnSpeechRecognizing;
            recognizer.SessionStarted += OnSessionStarted;
            recognizer.SessionStopped += OnSessionStopped;
            recognizer.Recognized += OnRecognized;
            recognizer.Synthesizing += OnSynthesizing;
            recognizer.Canceled += OnCanceled;
        }

        private AudioConfig GetAudioConfig(InputSourceType inputSource, string wavFilename)
        {
            switch (inputSource)
            {
                case InputSourceType.Microphone:
                    return AudioConfig.FromDefaultMicrophoneInput();
                case InputSourceType.WavFile:
                    return AudioConfig.FromWavFileInput(wavFilename);
                default:
                    throw new ArgumentException($"Unhandled InputSourceType: {inputSource}");
            }

        }

        private void SendMessage(string value)
        {
            Notification?.Invoke(this, new NotificationEventArgs(value));
        }

        private void OnCanceled(object sender, TranslationRecognitionCanceledEventArgs e)
        {
            SendMessage(e.ErrorDetails);
        }

        private void OnSynthesizing(object sender, TranslationSynthesisEventArgs e)
        {
            TranslationSynthesizing?.Invoke(sender, e);
        }

        private void OnRecognized(object sender, TranslationRecognitionEventArgs e)
        {
            SendMessage(e.ToString());
        }

        public Task StartContinuousRecognitionAsync()
        {
            return recognizer.StartContinuousRecognitionAsync();
        }

        public Task StopContinuousRecognitionAsync()
        {
            return recognizer.StopContinuousRecognitionAsync();
        }

        private void OnSessionStopped(object sender, SessionEventArgs e)
        {
            IsSessionStarted = false;
            IsSessionStopped = true;
            SendMessage($"Session {SessionId} stopped");
        }

        private void OnSessionStarted(object sender, SessionEventArgs e)
        {
            IsSessionStarted = true;
            IsSessionStopped = false;
            SessionId = e.SessionId;
            SendMessage($"Session {SessionId} started");
        }

        private void OnSpeechRecognizing(object sender, TranslationRecognitionEventArgs e)
        {
            Recognizing?.Invoke(sender, e);
        }

        public void Dispose()
        {
            if (recognizer != null)
            {
                recognizer.Recognizing -= OnSpeechRecognizing;
                recognizer.SessionStarted -= OnSessionStarted;
                recognizer.SessionStopped -= OnSessionStopped;
                recognizer.Dispose();
                recognizer = null;
            }
            if (audioConfig != null)
            {
                audioConfig.Dispose();
                audioConfig = null;
            }
        }
    }
}
