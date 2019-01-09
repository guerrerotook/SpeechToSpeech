namespace Microsoft.Cse.SpeechToSpeech.UI.Speech
{
    using System;

    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(string message)
        {
            message.EnsureIsNotNull(nameof(message));
            Message = message;
        }

        public string Message { get; private set; }
    }
}
