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

namespace SeparateCamExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Crevis.Devices.CrevisCamera camera;
        public MainWindow()
        {
            InitializeComponent();

            camera = new Crevis.Devices.CrevisCamera();
            camera.Open(1);

            int i = camera.CameraList.Count;

            textBox.Text = i.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            camera.Close();
        }
    }
}
