using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace PM2E2GRUPO3.Config
{
    public class Base64toImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource image = null;

            if (value != null)
            {
                String Base64Image =  (string)value;
                byte[] fotobyte = System.Convert.FromBase64String(Base64Image);
                var stream = new MemoryStream(fotobyte);
                image = ImageSource.FromStream(() => stream);
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
