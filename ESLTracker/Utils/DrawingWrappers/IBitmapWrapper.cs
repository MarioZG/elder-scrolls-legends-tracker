using System.Drawing;
using System.Drawing.Imaging;

namespace ESLTracker.Utils.DrawingWrappers
{
    public interface IBitmapWrapper
    {
        void Save(Bitmap bmp, string path, ImageFormat format);
    }
}