using System.Windows;
using System.Windows.Controls;

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

            control._bitmapImage = new(new(path));
            control._imageViewer.Source = control._bitmapImage;
        }
    }

}
