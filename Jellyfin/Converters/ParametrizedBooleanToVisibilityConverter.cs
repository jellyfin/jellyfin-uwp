using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Jellyfin.Converters
{
    public sealed class ParametrizedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = false;

            if (value is bool)
            {
                flag = (bool) value;
            }

            //If false is passed as a converter parameter then reverse the value of input value
            if (parameter != null)
            {
                bool par = true;
                if ((bool.TryParse(parameter.ToString(), out par)) && (!par)) flag = !flag;
            }

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility?) value == Visibility.Visible;
        }
    }
}
