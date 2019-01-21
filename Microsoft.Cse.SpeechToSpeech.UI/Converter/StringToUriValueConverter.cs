namespace Microsoft.Cse.SpeechToSpeech.UI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class StringToUriValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string valueString)
            {
                if (targetType != typeof(Uri))
                {
                    throw new ArgumentException("Can only convert to System.Uri");
                }
                return new Uri(valueString, UriKind.RelativeOrAbsolute);
            }
            else
            {
                throw new ArgumentException("Can only convert from string");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
