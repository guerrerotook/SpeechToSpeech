namespace Microsoft.Cse.SpeechToSpeech.UI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Returns a boolean indicating whether the parameter matches the value enum
    /// </summary>
    public class EnumValueMatchesValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type valueType = value.GetType();
            if (!valueType.IsEnum)
            {
                throw new ArgumentException("Must bind to an enum property");
            }
            if (targetType != typeof(bool?) && targetType != typeof(bool))
            {
                throw new ArgumentException($"{nameof(EnumValueMatchesValueConverter)} can only convert to bool or Nullable<bool>");
            }

            if (parameter is string parameterString)
            {
                object result = null;
                try
                {
                    result = Enum.Parse(valueType, (string)parameter);
                }
                catch (ArgumentException) { } // Argument exception thrown if parameter is not a valid enum member
                return value.Equals(result);
            }
            else if (parameter.GetType() == valueType)
            {
                return value.Equals(parameter);
            }
            else
            {
                throw new ArgumentException("parameter must either be a string or the value enum type");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool b && b))
            {
                throw new ArgumentException("ConvertBack only handles boolean true value to convert");
            }
            if (!targetType.IsEnum)
            {
                throw new ArgumentException("Must bind to an enum property");
            }
            if (parameter is string parameterString)
            {
                return Enum.Parse(targetType, (string)parameter);
            }
            else if (parameter.GetType() == targetType)
            {
                return parameter;
            }
            else
            {
                throw new ArgumentException("Type to convert must either be a string or the value enum type");
            }
        }
    }
}
