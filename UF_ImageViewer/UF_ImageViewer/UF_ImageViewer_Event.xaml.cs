using System;
using System.Diagnostics;
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
        if(_imageViewer.IsFocused == false)
            _imageViewer.Focus();

        ScaleTransform st = _scaleTransform;
        if (st.ScaleX == 1.0 && st.ScaleY == 1.0)
            return;

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
        Zoom(e.Delta > 0, e.GetPosition(_imageViewer));
    }

    void ImageViewer_KeyDown(object sender, KeyEventArgs e)
    {
        bool zoom = true;
        switch(e.Key)
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
