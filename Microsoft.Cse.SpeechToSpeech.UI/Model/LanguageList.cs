namespace Microsoft.Cse.SpeechToSpeech.UI.Model
{
    using System.Collections.Generic;

    public static class LanguageList
    {
        public static IEnumerable<Language> Languages { get => languages; }

        private static Language[] languages = {
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
    }
}
