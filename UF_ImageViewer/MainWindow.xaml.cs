using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace UF.ImageViewer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files|*.png;*.jpg;*.bmp|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
            myViewer.ImagePath = openFileDialog.FileName;
    }
}
