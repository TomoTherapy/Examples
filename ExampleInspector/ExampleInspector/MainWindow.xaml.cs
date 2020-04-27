using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExampleInspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CogRecordDisplay cogDisplay;

        public MainWindow()
        {
            InitializeComponent();

            cogDisplay = new CogRecordDisplay();
            CogDisplay_WFH.Child = cogDisplay;
        }

        private void Exit_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Setting_button_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow();
            window.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cogDisplay.HorizontalScrollBar = false;
            cogDisplay.VerticalScrollBar = false;

            cogDisplay.Image = new CogImage24PlanarColor(new System.Drawing.Bitmap(@"C:\Users\jkhong\Desktop\이미지 009.bmp"));
            cogDisplay.Fit();
        }
    }
}
