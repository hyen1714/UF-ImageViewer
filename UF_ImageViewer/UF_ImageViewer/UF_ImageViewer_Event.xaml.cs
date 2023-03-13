using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    Point panXY, clickStart;

    void UserControl_Drop(object sender, DragEventArgs e)
    {
        string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
        foreach (string path in data)
        {
            if (IsImageFile(path) == true)
            {
                ImagePath = path;
                return;
            }
        }
    }

    void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged == true || e.HeightChanged == true)
        {
            _imageViewer.Width = e.NewSize.Width;
            _imageViewer.Height = e.NewSize.Height;

            ClearPixelLine();
        }
    }

    void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_imageViewer.IsFocused == false)
            _imageViewer.Focus();

        ScaleTransform st = _scaleTransform;
        if (st.ScaleX == 1.0 && st.ScaleY == 1.0)
            return;

        TranslateTransform tt = _translateTransform;
        panXY = new Point(tt.X, tt.Y);
        clickStart = e.GetPosition(_canvas);
        _imageViewer.CaptureMouse();
    }

    void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _imageViewer.ReleaseMouseCapture();
    }

    void ImageViewer_MouseMove(object sender, MouseEventArgs e)
    {
        if (_imageViewer.IsMouseCaptured)
        {
            Pan(panXY, clickStart, e.GetPosition(_canvas));
            ClearPixelLine();
            DrawPixelLine();
        }

        // 현재 마우스 위치 값을 이미지 픽셀 값으로
        double ratioW = _bitmapImage.PixelWidth / _imageViewer.ActualWidth;
        double ratioH = _bitmapImage.PixelHeight / _imageViewer.ActualHeight;
        double x = e.GetPosition(_imageViewer).X * ratioW;
        double y = e.GetPosition(_imageViewer).Y * ratioH;
        CurrentPixelPosition = new Point(x, y);
    }

    void ImageViewer_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        Zoom(e.Delta > 0, e.GetPosition(_imageViewer));

        ClearPixelLine();
        DrawPixelLine();
    }

    void ImageViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 타원 그리기
        Ellipse ellipse = new Ellipse();
        ellipse.Width = 100;
        ellipse.Height = 50;
        ellipse.Fill = Brushes.Green;
        Canvas.SetLeft(ellipse, 120);
        Canvas.SetTop(ellipse, 150);
        _canvas.Children.Add(ellipse);
        _canvas.Children.Remove(ellipse);
    }

    void ImageViewer_KeyDown(object sender, KeyEventArgs e)
    {
        bool zoom = true;
        switch (e.Key)
        {
            case Key.Add:
                break;
            case Key.Subtract:
                zoom = false;
                break;
            default:
                return;
        }
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            Zoom(zoom, Mouse.GetPosition(_imageViewer));
    }
}

