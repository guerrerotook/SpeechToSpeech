namespace Microsoft.Cse.SpeechToSpeech.UI.Model.WavValidation
{
    public class WavError
    {
        public WavError(WavFormatErrorType format, string message)
        {
            Format = format;
            Message = message;
        }

        public WavFormatErrorType Format { get; private set; }

        public string Message { get; private set; }
    }
}
