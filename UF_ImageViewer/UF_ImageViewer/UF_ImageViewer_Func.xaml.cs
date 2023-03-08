using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace UF_ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    void Pan(Point panXY, Point clickStart, Point clickCurrent)
    {
        ScaleTransform st = _scaleTransform;
        TranslateTransform tt = _translateTransform;
        double width = _imageViewer.ActualWidth;
        double height = _imageViewer.ActualHeight;

        double scaleX = st.ScaleX;
        double scaleY = st.ScaleY;
        if (scaleX == 1.0 && scaleY == 1.0)
            return;

        Vector v = clickStart - clickCurrent;
        double panX = panXY.X - v.X;
        double panY = panXY.Y - v.Y;

        if (panX > 0.0)
            panX = 0.0;
        else
        {
            double widthScale = width * scaleX;
            if (panX + widthScale < width)
                panX = width - widthScale;
        }

        if (panY > 0.0)
            panY = 0.0;
        else
        {
            double heightScale = height * scaleY;
            if (panY + heightScale < height)
                panY = height - heightScale;
        }

        tt.X = panX;
        tt.Y = panY;
    }

    /// <summary>
    /// 이미지를 확대 축소한다.
    /// </summary>
    /// <param name="zoomIn"> 확대 인 경우 true </param>
    /// <param name="point"> 확대, 축소의 기준이 될 이미지의 위치 </param>
    void Zoom(bool zoomIn, Point point)
    {
        ScaleTransform st = _scaleTransform;
        TranslateTransform tt = _translateTransform;
        double width = _imageViewer.ActualWidth;
        double height = _imageViewer.ActualHeight;

        double scaleX = st.ScaleX;
        double scaleY = st.ScaleY;
        if (zoomIn == true)
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

        double absoluteX = (point.X * st.ScaleX) + tt.X;
        double absoluteY = (point.Y * st.ScaleY) + tt.Y;
        double panX = absoluteX - (point.X * scaleX);
        double panY = absoluteY - (point.Y * scaleY);
        if (panX > 0.0)
            panX = 0.0;
        else
        {   
            double widthScale = width * scaleX;
            if (panX + widthScale < width)
                panX = width - widthScale;
        }
        if (panY > 0.0)
            panY = 0.0;
        else
        {   
            double heightScale = height * scaleY;
            if (panY + heightScale < height)
                panY = height - heightScale;
        }

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
