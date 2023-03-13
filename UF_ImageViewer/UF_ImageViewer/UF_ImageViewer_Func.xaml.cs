using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    /// <summary>
    /// 이미지를 이동 시킵니다.
    /// </summary>
    /// <param name="panXY"> 이전 출력 위치 </param>
    /// <param name="clickStart"> 시작 클릭 위치 </param>
    /// <param name="clickCurrent"> 현재 클릭 위치 </param>
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

    /// <summary>
    /// 이미지를 확대 축소한다.
    /// </summary>
    /// <param name="zoomIn"> 확대 인 경우 true </param>
    /// <param name="point"> 확대, 축소의 기준이 될 이미지의 위치 </param>
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

    void ClearPixelLine()
    {
        var linesToRemove = _canvas.Children.OfType<Line>().Where(line => line.Name.StartsWith("PixelLine")).ToList();
        foreach (var line in linesToRemove)
            _canvas.Children.Remove(line);
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
            double startX =  dipPixelW * gapX;
            double startY =  dipPixelH * gapY;

            for (int i = 0; i < canvasPixelCntX; i++)
            {
                string keyV = $"PixelLineV{i}";
                Line? shape = _canvas.Children.OfType<Line>().FirstOrDefault(s => s.Name == keyV);
                if (shape == null)
                {
                    Line line = new()
                    {
                        X1 = startX + (dipPixelW * i),
                        X2 = startX + (dipPixelW * i),
                        Y2 = _canvas.ActualHeight,
                        Stroke = new SolidColorBrush(PixelLineColor),
                        Name = keyV
                    };
                    _canvas.Children.Add(line);
                }
            }
            
            for (int i = 0; i < canvasPixelCntY; i++)
            {
                string keyH = $"PixelLineH{i}";
                Line? shape = _canvas.Children.OfType<Line>().FirstOrDefault(s => s.Name == keyH);
                if (shape == null)
                {
                    Line line = new()
                    {
                        X2 = _canvas.ActualWidth,
                        Y1 = startY + (dipPixelH * i),
                        Y2 = startY + (dipPixelH * i),
                        Stroke = new SolidColorBrush(PixelLineColor),
                        Name = keyH
                    };
                    _canvas.Children.Add(line);
                }
            }
        }
    }

    /// <summary>
    /// 파일의 확장자확인하여 이미지 파일인지 판단.
    /// </summary>
    /// <param name="filePath"> 풀경로 </param>
    /// <returns> 이미지 확장자 인 경우 true </returns>
    static bool IsImageFile(string filePath)
    {
        string extension = System.IO.Path.GetExtension(filePath).ToLower();
        return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif";
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

