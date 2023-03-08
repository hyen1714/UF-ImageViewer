using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace UF_ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    Bitmap? _imgOrg = null, _imgShow = null;

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

            using Bitmap source = new(path);
            control._imgOrg = new(source);
            control._imgShow = new(source);
            control._imageViewer.Source = Bitmap2BitmapImage(control._imgShow);
        }
    }
}
