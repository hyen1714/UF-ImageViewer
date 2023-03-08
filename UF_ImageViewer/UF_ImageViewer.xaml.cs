using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UF_ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    Bitmap? _imgOrg = null, _imgShow = null;

    public UF_ImageViewer()
    {
        InitializeComponent();
    }

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
    
    void ImageViewer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
    }

    void ImageViewer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
    }

    void ImageViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        ScaleTransform st = _scaleTransform;
        TranslateTransform tt = _translateTransform;

        double scaleX = st.ScaleX;
        double scaleY = st.ScaleY;
        if (e.Delta < 0)
        {
            scaleX *= 0.7;
            scaleY *= 0.7;
        }
        else
        {
            scaleX *= 1.6;
            scaleY *= 1.6;
        }
        if (scaleX < 1.0)
            scaleX = 1.0;
        if (scaleY < 1.0)
            scaleY = 1.0;

        System.Windows.Point mousePos = e.GetPosition(_imageViewer);
        double absoluteX = (mousePos.X * st.ScaleX) + tt.X;
        double absoluteY = (mousePos.Y * st.ScaleY) + tt.Y;
        double panX = absoluteX - (mousePos.X * scaleX);
        double panY = absoluteY - (mousePos.Y * scaleY);
        if (panX > 0.0 || scaleX == 1.0)
            panX = 0.0;
        if (panY > 0.0 || scaleY == 1.0)
            panY = 0.0;

        st.ScaleX = scaleX;
        st.ScaleY = scaleY;
        tt.X = panX;
        tt.Y = panY;
    }


    static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
    {
        using MemoryStream outStream = new();
        BitmapEncoder enc = new BmpBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        enc.Save(outStream);
        using Bitmap bitmap = new(outStream);

        return new Bitmap(bitmap);
    }

    static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
    {
        using var memory = new MemoryStream();
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }
}
