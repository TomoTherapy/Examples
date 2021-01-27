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

namespace CogAcqFifoExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CogAcqFifoTool fifo;
        CogDisplay display;
        bool IsFreeRun;

        public MainWindow()
        {
            InitializeComponent();

            fifo = CogSerializer.LoadObjectFromFile(@"C:\Users\jkhong\Desktop\fifo.vpp") as CogAcqFifoTool;
            IsFreeRun = true;
            textBox.Text = "FreeRun";

            display = new CogDisplay();
            WFH.Child = display;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            display.StartLiveDisplay(fifo.Operator, true);
            display.Fit();
        }

        private void Stopbutton_Click(object sender, RoutedEventArgs e)
        {
            display.StopLiveDisplay();
        }

        private void TriggerModelbutton_Click(object sender, RoutedEventArgs e)
        {
            if (IsFreeRun)
            {
                ICogAcqTrigger triggerParam;

                triggerParam = fifo.Operator.OwnedTriggerParams;
                if (triggerParam != null)
                {
                    triggerParam.TriggerEnabled = true;
                    triggerParam.TriggerModel = CogAcqTriggerModelConstants.Manual;
                }
                textBox.Text = "Manual";
            }
            else
            {
                ICogAcqTrigger triggerParam;

                triggerParam = fifo.Operator.OwnedTriggerParams;
                if (triggerParam != null)
                {
                    triggerParam.TriggerEnabled = true;
                    triggerParam.TriggerModel = CogAcqTriggerModelConstants.FreeRun;
                }
                textBox.Text = "FreeRun";
            }

            IsFreeRun = !IsFreeRun;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            display.StopLiveDisplay();
        }
    }
}
