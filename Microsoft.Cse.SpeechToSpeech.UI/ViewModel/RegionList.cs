namespace Microsoft.Cse.SpeechToSpeech.UI.ViewModel
{
    using System.Collections.Generic;

    public static class RegionList
    {
        public static IEnumerable<string> Regions { get => regions; }

        private static string[] regions = { "westus", "westeurope", "eastasia", "northeurope" };
    }
}
