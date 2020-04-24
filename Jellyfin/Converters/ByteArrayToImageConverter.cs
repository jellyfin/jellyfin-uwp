using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is byte[]))
                return null;

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                // Writes the image byte array in an InMemoryRandomAccessStream
                // that is needed to set the source of BitmapImage.
                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes((byte[])value);

                    // The GetResults here forces to wait until the operation completes
                    // (i.e., it is executed synchronously), so this call can block the UI.
                    writer.StoreAsync().GetResults();
                }

                var image = new BitmapImage();
                image.SetSource(ms);
                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
