// Talis.XIVPlugin.Twintania
// TimerConverter.cs
// 
// 	

using System;
using System.Globalization;
using System.Windows.Data;

namespace Talis.XIVPlugin.Twintania.Converters
{
    [ValueConversion(typeof (object), typeof (string))]
    public class TimerConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = System.Convert.ToDouble(value);
            var ss = System.Convert.ToInt32(Math.Abs(d));

            if (ss > 60)
            {
                var mm = ss / 60;
                ss = ss % 60;
                return string.Format(@"{0:D2}:{1:D2}", mm, ss);
            }

            return string.Format(@"{0:F2}", d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}
