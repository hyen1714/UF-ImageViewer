using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;
using Point = System.Windows.Point;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    void Pan(Point panXY, Point clickStart, Point clickCurrent)
    {
        ScaleTransform st = _scaleTransform;
        double scaleW = st.ScaleX;
        double scaleH = st.ScaleY;
        if (scaleW == 1.0 && scaleH == 1.0)
            return;

        TranslateTransform tt = _translateTransform;
        Vector vector = clickStart - clickCurrent;
        double panX = panXY.X - vector.X;
        double panY = panXY.Y - vector.Y;
        double imgDipW = _imageViewer.ActualWidth;
        double imgDipH = _imageViewer.ActualHeight;
        if (panX > 0.0)
            panX = 0.0;
        else
        {
            double scaleDipW = imgDipW * scaleW;
            if (panX + scaleDipW < imgDipW)
                panX = imgDipW - scaleDipW;
        }

        if (panY > 0.0)
            panY = 0.0;
        else
        {
            double scaleDipH = imgDipH * scaleH;
            if (panY + scaleDipH < imgDipH)
                panY = imgDipH - scaleDipH;
        }

        tt.X = panX;
        tt.Y = panY;
    }

    void Zoom(bool zoomIn, Point point)
    {
        ScaleTransform st = _scaleTransform;
        if (zoomIn == true)
        {
            double pixelCntX = _bitmapImage.PixelWidth / st.ScaleX;   
            double pixelCntY = _bitmapImage.PixelHeight / st.ScaleY;
            double dipPixelW = _imageViewer.ActualWidth / pixelCntX;  
            double dipPixelH = _imageViewer.ActualHeight / pixelCntY;
            double canvasPixelCntX = _canvas.ActualWidth / dipPixelW; 
            double canvasPixelCntY = _canvas.ActualHeight / dipPixelH;
            if (canvasPixelCntX < 10.0 && canvasPixelCntY < 10.0)
                return;
        }

        double scaleW = st.ScaleX;
        double scaleH = st.ScaleY;
        if (zoomIn == false)
        {
            scaleW *= 0.7;
            scaleH *= 0.7;
            if (scaleW < 1.0)
                scaleW = 1.0;
            if (scaleH < 1.0)
                scaleH = 1.0;
        }
        else
        {
            scaleW *= 1.6;
            scaleH *= 1.6;
        }

        TranslateTransform tt = _translateTransform;
        double absoluteX = (point.X * st.ScaleX) + tt.X;
        double absoluteY = (point.Y * st.ScaleY) + tt.Y;
        double panX = absoluteX - (point.X * scaleW);
        double panY = absoluteY - (point.Y * scaleH);
        double imgDipW = _imageViewer.ActualWidth;
        double imgDipH = _imageViewer.ActualHeight;
        if (panX > 0.0)
            panX = 0.0;
        else
        {
            double scaleDipW = imgDipW * scaleW;
            if (panX + scaleDipW < imgDipW)
                panX = imgDipW - scaleDipW;
        }
        if (panY > 0.0)
            panY = 0.0;
        else
        {
            double scaleDipH = imgDipH * scaleH;
            if (panY + scaleDipH < imgDipH)
                panY = imgDipH - scaleDipH;
        }

        st.ScaleX = scaleW;
        st.ScaleY = scaleH;
        tt.X = panX;
        tt.Y = panY;
    }

    void ResetZoom()
    {
        ScaleTransform st = _scaleTransform;
        TranslateTransform tt = _translateTransform;
        st.ScaleX = 1;
        st.ScaleY = 1;
        tt.X = 0;
        tt.Y = 0;
    }

    void ClearPixelLine()
    {
        var linesToRemove = _canvas.Children.OfType<Line>().Where(line => line.Name.StartsWith("PixelLine")).ToList();
        foreach (var line in linesToRemove)
            _canvas.Children.Remove(line);

        var textBlocksToRemove = _canvas.Children.OfType<TextBlock>().Where(line => line.Name.StartsWith("PixelValue")).ToList();
        foreach (var textBlock in textBlocksToRemove)
            _canvas.Children.Remove(textBlock);
    }

    void DrawPixelLine()
    {        
        ScaleTransform st = _scaleTransform;
        double pixelCntX = _bitmapImage.PixelWidth / st.ScaleX;   // 확대한 픽셀 개수
        double pixelCntY = _bitmapImage.PixelHeight / st.ScaleY;
        double dipPixelW = _imageViewer.ActualWidth / pixelCntX;  // 현재 픽셀 사이즈
        double dipPixelH = _imageViewer.ActualHeight / pixelCntY;
        double canvasPixelCntX = _canvas.ActualWidth / dipPixelW; // 화면상 보여지고 픽셀 개수
        double canvasPixelCntY = _canvas.ActualHeight / dipPixelH;

        if (canvasPixelCntX < 20.0 && canvasPixelCntY < 20.0)
        {
            TranslateTransform tt = _translateTransform;
            double canvasWGap = Math.Round(_canvas.ActualWidth - _imageViewer.ActualWidth, 3) / 2.0;
            double canvasHGap = Math.Round(_canvas.ActualHeight - _imageViewer.ActualHeight, 3) / 2.0;
            double calibrationX = canvasWGap / dipPixelW;
            double calibrationY = canvasHGap / dipPixelH;

            double ratioX = _bitmapImage.PixelWidth / (_imageViewer.ActualWidth * st.ScaleX);
            double ratioY = _bitmapImage.PixelHeight / (_imageViewer.ActualHeight * st.ScaleY);
            double pixelL = Math.Abs(tt.X * ratioX) - calibrationX;
            double pixelT = Math.Abs(tt.Y * ratioY) - calibrationY;

            double gapX = Math.Ceiling(pixelL) - pixelL;
            double gapY = Math.Ceiling(pixelT) - pixelT;
            double startLineX =  dipPixelW * gapX;
            double startLineY =  dipPixelH * gapY;

            string key;
            for (int i = 0; i < canvasPixelCntX; i++)
            {
                key = $"PixelLineV{i}";
                _canvas.Children.Add(new Line()
                {
                    X1 = startLineX + (dipPixelW * i),
                    X2 = startLineX + (dipPixelW * i),
                    Y2 = _canvas.ActualHeight,
                    Stroke = new SolidColorBrush(PixelLineColor),
                    Name = key
                });
            }

            for (int i = 0; i < canvasPixelCntY; i++)
            {
                key = $"PixelLineH{i}";
                _canvas.Children.Add(new Line()
                {
                    X2 = _canvas.ActualWidth,
                    Y1 = startLineY + (dipPixelH * i),
                    Y2 = startLineY + (dipPixelH * i),
                    Stroke = new SolidColorBrush(PixelLineColor),
                    Name = key
                });
            }

            if(_bitmapImage.Format.BitsPerPixel == 8)
            {
                double startValX = startLineX - dipPixelW;
                double startValY = startLineY - dipPixelH;
                int startPixelX = (int)Math.Truncate(pixelL);
                int startPixelY = (int)Math.Truncate(pixelT);
                for (int i = 0; i < canvasPixelCntX + 1; i++)
                {
                    for (int j = 0; j < canvasPixelCntY + 1; j++)
                    {
                        int idx = ((startPixelY + j) * _bitmapImage.PixelWidth) + (startPixelX + i);
                        key = $"PixelValue{i}{j}";
                        TextBlock clrVal = new()
                        {
                            Text = $"{_pixels[idx]}",
                            FontSize = 20,
                            Foreground = new SolidColorBrush(PixelValuColor),
                            Name = key
                        };
                        clrVal.MouseWheel += ImageViewer_MouseWheel;
                        clrVal.MouseLeftButtonDown += ImageViewer_MouseLeftButtonDown;
                        clrVal.MouseLeftButtonUp += ImageViewer_MouseLeftButtonUp;
                        clrVal.MouseMove += ImageViewer_MouseMove;

                        Canvas.SetLeft(clrVal, startValX + (dipPixelW * i));
                        Canvas.SetTop(clrVal, startValY + (dipPixelH * j));
                        _canvas.Children.Add(clrVal);
                    }
                }
            }
        }
    }

    static bool IsImageFile(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif" || extension == ".tif";
    }
}

