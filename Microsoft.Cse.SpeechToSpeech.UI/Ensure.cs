namespace Microsoft.Cse.SpeechToSpeech.UI
{
    using System;

    public static class Ensure
    {
        public static void EnsureIsNotNull(this string value, string paramName = null, string message = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
        }
    }
}
