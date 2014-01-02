using System;
using System.Windows.Data;

namespace talis.xivplugin.twintania.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class TimerConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d = System.Convert.ToDouble(value);
            Int32 ss = System.Convert.ToInt32(Math.Abs(d));

            if (ss > 60)
            {
                Int32 mm = ss / 60;
                ss = ss % 60;
                return string.Format(@"{0:D2}:{1:D2}", mm, ss);
            }

            return string.Format(@"{0:F2}", d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}
