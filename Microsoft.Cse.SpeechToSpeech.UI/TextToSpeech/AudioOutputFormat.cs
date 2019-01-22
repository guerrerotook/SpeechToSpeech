namespace Microsoft.Cse.SpeechToSpeech.UI.TextToSpeech
{
    /// <summary>
    /// Voice output formats.
    /// </summary>
    public enum AudioOutputFormat
    {
        /// <summary>
        /// raw-8khz-8bit-mono-mulaw request output audio format type.
        /// </summary>
        Raw8Khz8BitMonoMULaw,
        /// <summary>
        /// raw-16khz-16bit-mono-pcm request output audio format type.
        /// </summary>
        Raw16Khz16BitMonoPcm,
        /// <summary>
        /// riff-8khz-8bit-mono-mulaw request output audio format type.
        /// </summary>
        Riff8Khz8BitMonoMULaw,
        /// <summary>
        /// riff-16khz-16bit-mono-pcm request output audio format type.
        /// </summary>
        Riff16Khz16BitMonoPcm,
        /// <summary>
        /// riff-24khz-16bit-mono-pcm request output audio format type.
        /// </summary>
        Riff24Khz16BitMonoPcm,
    }
}
