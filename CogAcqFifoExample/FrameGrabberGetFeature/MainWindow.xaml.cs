using Cognex.VisionPro;
using Cognex.VisionPro.Display;
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

namespace FrameGrabberGetFeature
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CogDisplay display;
        CogFrameGrabbers grabbers;
        List<GigECamInfo> camList;
        ICogAcqFifo fifo;

        public MainWindow()
        {
            InitializeComponent();

            camList = new List<GigECamInfo>();
            grabbers = new CogFrameGrabbers();

            if (grabbers.Count != 0)
            {
                for (int i = 0; i < grabbers.Count; i++)
                {
                    ICogFrameGrabber grabber = grabbers[i];
                    ICogGigEAccess gigEAccess = grabber.OwnedGigEAccess;
                    string vendor = gigEAccess.GetFeature("DeviceVendorName");

                    camList.Add(new GigECamInfo() { Name = grabber.Name, SerialNumber = grabber.SerialNumber, VendorName = vendor });
                }
            }

            dataGrid.ItemsSource = camList;

            display = new CogDisplay();
            WFH.Child = display;

            fifo = grabbers[0].CreateAcqFifo("Generic GigEVision (Mono)", CogAcqFifoPixelFormatConstants.Format8Grey, 0, false);
            textBlock.Text = grabbers[0].Name;
            dataGrid.SelectedIndex = 0;
        }

        private void Single_button_Click(object sender, RoutedEventArgs e)
        {
            ICogImage image = fifo.Acquire(out int a);

            display.Image = image;
            display.Fit();
        }

        private void Live_button_Click(object sender, RoutedEventArgs e)
        {
            if (display.LiveDisplayRunning)
            {
                display.StopLiveDisplay();
            }
            else
            {
                display.StartLiveDisplay(fifo, true);
                display.Fit();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fifo = grabbers[dataGrid.SelectedIndex].CreateAcqFifo("Generic GigEVision (Mono)", CogAcqFifoPixelFormatConstants.Format8Grey, 0, false);
            textBlock.Text = grabbers[dataGrid.SelectedIndex].Name;
        }
    }

    public class GigECamInfo
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string VendorName { get; set; }
    }
}
