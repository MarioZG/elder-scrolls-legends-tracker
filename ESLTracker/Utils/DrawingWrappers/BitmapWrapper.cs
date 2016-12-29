using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.DrawingWrappers
{
    public class BitmapWrapper : IBitmapWrapper
    {
        public BitmapWrapper()
        {
        }

        public void Save(Bitmap bmp, string path, ImageFormat format)
        {
            bmp.Save(path, format);
        }
    }
}
