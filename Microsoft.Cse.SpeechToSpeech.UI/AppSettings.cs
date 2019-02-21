using GalaSoft.MvvmLight;

namespace Microsoft.Cse.SpeechToSpeech.UI
{
    /// <summary>
    /// ViewModel for app settings
    /// </summary>
    public class AppSettings : ViewModelBase
    {
        private string speechSubscriptionKey;
        public string SpeechSubscriptionKey { get => speechSubscriptionKey; set => Set(nameof(SpeechSubscriptionKey), ref speechSubscriptionKey, value); }

        private string customModelEndpointId;
        public string CustomModelEndpointId { get => customModelEndpointId; set => Set(nameof(CustomModelEndpointId), ref customModelEndpointId, value); }

        private string textTranslationSubscriptionKey;
        public string TextTranslationSubscriptionKey { get => textTranslationSubscriptionKey; set => Set(nameof(TextTranslationSubscriptionKey), ref textTranslationSubscriptionKey, value); }
    }
}
