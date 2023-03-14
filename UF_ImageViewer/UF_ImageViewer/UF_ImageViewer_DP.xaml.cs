using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(UF_ImageViewer), new PropertyMetadata("", ImagePathPropertyChanged));
    public string ImagePath
    {
        get { return (string)GetValue(ImagePathProperty); }
        set { SetValue(ImagePathProperty, value); }
    }
    static void ImagePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UF_ImageViewer control)
        {
            string path = (string)e.NewValue;
            if (IsImageFile(path) == false)
                return;

            BitmapImage image = new(new(path));
            int pixelWidth = image.PixelWidth;
            int pixelHeight = image.PixelHeight;
            int bitCountPerPixel = image.Format.BitsPerPixel;
            int stride = (pixelWidth * bitCountPerPixel / 8 + 3) & ~3;
            int size = stride * pixelHeight;
            if (control._pixels.Length != size)
                control._pixels = new byte[size];
            image.CopyPixels(control._pixels, stride, 0);

            control.ClearPixelLine();
            control.ResetZoom();
            control._bitmapImage = image;
            control._imageViewer.Source = image;
        }
    }
}
