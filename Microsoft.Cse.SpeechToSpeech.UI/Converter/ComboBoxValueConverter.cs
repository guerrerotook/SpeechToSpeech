namespace Microsoft.Cse.SpeechToSpeech.UI.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Data;


    public class ComboBoxValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ComboBoxItem item = value as ComboBoxItem;
            if (item != null)
            {
                return item.Content as string;
            }
            else if(value is string)
            {
                return value;
            }
            else 
            {
                return null;
            }
        }
    }
}
