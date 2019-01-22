namespace Microsoft.Cse.SpeechToSpeech.UI.Translation
{
    using Newtonsoft.Json;

    public class TranslationItem
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string LanguageCode { get; set; }
    }
}
