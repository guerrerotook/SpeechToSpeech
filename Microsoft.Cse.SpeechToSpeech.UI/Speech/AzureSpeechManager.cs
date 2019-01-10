namespace Microsoft.Cse.SpeechToSpeech.UI.Speech
{
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Translation;
    using Microsoft.Cse.SpeechToSpeech.UI;
    using System;
    using System.Threading.Tasks;

    public class AzureSpeechManager : IDisposable
    {
        private SpeechTranslationConfig speechConfiguration;
        private TranslationRecognizer recognized;
        public bool IsSessionStarted { get; set; }
        public bool IsSessionStopped { get; set; }
        public string SessionId { get; set; }

        public event EventHandler<TranslationRecognitionEventArgs> Recognizing;
        public event EventHandler<NotificationEventArgs> Notification;

        public AzureSpeechManager()
        {

        }

        public void Initialize(string subscriptionKey, string region)
        {
            subscriptionKey.EnsureIsNotNull(nameof(subscriptionKey));
            subscriptionKey.EnsureIsNotNull(nameof(region));

            speechConfiguration = SpeechTranslationConfig.FromSubscription(subscriptionKey, region);
            speechConfiguration.OutputFormat = OutputFormat.Detailed;
            SendMessage($"Created the SpeechConfiguration with {subscriptionKey} | {region}");
        }

        public void SetSpeechLanguage(string language, string translationLanguage)
        {
            speechConfiguration.SpeechRecognitionLanguage = language;
            speechConfiguration.AddTargetLanguage(translationLanguage);
        }

        public async Task CreateTranslationRecognizer()
        {
            if (recognized != null)
            {
                await recognized.StopContinuousRecognitionAsync();
                await recognized.StopKeywordRecognitionAsync();

            }

            recognized = new TranslationRecognizer(speechConfiguration);
            recognized.Recognizing += OnSpeechRecognizing;
            recognized.SessionStarted += OnSessionStarted;
            recognized.SessionStopped += OnSessionStopped;
            recognized.Recognized += OnRecognized;
            recognized.Synthesizing += OnSynthesizing;
            recognized.Canceled += OnCanceled;
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
            SendMessage(e.Result.ToString());
        }

        private void OnRecognized(object sender, TranslationRecognitionEventArgs e)
        {
            SendMessage(e.ToString());
        }

        public Task StartContinuousRecognitionAsync()
        {
            return recognized.StartContinuousRecognitionAsync();
        }

        public Task StopContinuousRecognitionAsync()
        {
            return recognized.StopContinuousRecognitionAsync();
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
            if (recognized != null)
            {
                recognized.Recognizing -= OnSpeechRecognizing;
                recognized.SessionStarted -= OnSessionStarted;
                recognized.SessionStopped -= OnSessionStopped;
                recognized.Dispose();
                recognized = null;
            }
        }
    }
}
