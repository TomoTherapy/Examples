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
using System.Windows.Shapes;

namespace ExampleInspector
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        CogRecordDisplay display;
        public SettingWindow()
        {
            InitializeComponent();

            List<string> list = new List<string>();

            list.Add("Model 1");
            list.Add("Model 2");

            listView.ItemsSource = list;

            display = new CogRecordDisplay();
            Display_WFH.Child = display;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            display.HorizontalScrollBar = false;
            display.VerticalScrollBar = false;

            display.Image = new CogImage24PlanarColor(new System.Drawing.Bitmap(@"C:\Users\jkhong\Desktop\이미지 009.bmp"));
            display.Fit();
        }
    }
}
