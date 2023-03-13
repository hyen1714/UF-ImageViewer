using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    BitmapImage _bitmapImage = new();

    public UF_ImageViewer()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 마우스 포인터의 이미지 픽셀 위치
    /// </summary>
    public Point CurrentPixelPosition { get; private set; }

    /// <summary>
    /// 픽셀라인의 색상을 변경합니다.
    /// </summary>
    public Color PixelLineColor { get; set; } = Color.FromArgb(255, 221, 81, 69);
}