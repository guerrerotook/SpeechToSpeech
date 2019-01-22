namespace Microsoft.Cse.SpeechToSpeech.UI.Translation
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class TranslationResult
    {
        public TranslationResult()
        {
            IsSuccess = true;
        }

        [JsonProperty(PropertyName = "translations")]
        public List<TranslationItem> Translations { get; set; }

        public bool IsSuccess { get; set; }

        public string Error { get; set; }
    }
}
