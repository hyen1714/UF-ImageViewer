using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace UF.ImageViewer;

public partial class UF_ImageViewer : UserControl
{
    BitmapImage _bitmapImage = new();
    byte[] _pixels = new byte[1];

    public UF_ImageViewer()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 마우스 포인터의 이미지 픽셀 위치
    /// </summary>
    public Point CurrentPixelPosition { get; private set; }

    /// <summary>
    /// 마우스 위치의 이미지 색상 값
    /// </summary>
    public Color CurrentPixelColor { get; private set; }

    /// <summary>
    /// 픽셀라인의 색상을 변경합니다.
    /// </summary>
    public Color PixelLineColor { get; set; } = Color.FromRgb(221, 81, 69);

    /// <summary>
    /// 픽셀값을 표시할 색상을 변경합니다.
    /// </summary>
    public Color PixelValuColor { get; set; } = Color.FromRgb(28, 255, 97);


}