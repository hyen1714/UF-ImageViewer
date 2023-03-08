using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    Point panXY, clickStart;

    void UserControl_Drop(object sender, DragEventArgs e)
    {
        string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
        foreach(string path in data)
        {
            if(IsImageFile(path) == true)
            {
                ImagePath = path;
                return;
            }
        }
    }

    void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        TranslateTransform tt = _translateTransform;
        panXY = new Point(tt.X, tt.Y);
        clickStart = e.GetPosition(this);

        _imageViewer.CaptureMouse();
    }

    void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _imageViewer.ReleaseMouseCapture();
    }

    void ImageViewer_MouseMove(object sender, MouseEventArgs e)
    {
        if (_imageViewer.IsMouseCaptured)
            Pan(panXY, clickStart, e.GetPosition(this));
    }

    void ImageViewer_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        Zoom(e.Delta < 0, e.GetPosition(_imageViewer));
    }

}
