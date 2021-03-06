﻿using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters
{
    public class PackScreenshotSettingPreview : MarkupConverter<PackScreenshotSettingPreview>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try {
                return MasserContainer.Container.GetInstance<ScreenshotNameProvider>().GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack, value.ToString());
            }
            catch(Exception ex)
            {
                return "ERROR:"+ex.Message;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
