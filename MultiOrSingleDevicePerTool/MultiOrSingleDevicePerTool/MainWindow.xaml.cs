using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

namespace MultiOrSingleDevicePerTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViDi2.Runtime.Local.Control ViDiControl;
        public int AverageTime { get; set; }
        private string WSName;
        private int count;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Single_Click(object sender, RoutedEventArgs e)
        {
            if (ViDiControl != null) ViDiControl.Dispose();
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            int gpuCount = mos.Get().Count;

            switch (gpuCount)
            {
                case 0:
                    throw new Exception("GPU가 없습니다");
                case 1:
                    ViDiControl = new ViDi2.Runtime.Local.Control();
                    break;
                default:
                    try
                    {
                        List<int> gpuDevices = new List<int>();
                        for (int i = 0; i < gpuCount; i++) gpuDevices.Add(i);
                        ViDiControl = new ViDi2.Runtime.Local.Control(ViDi2.GpuMode.SingleDevicePerTool, gpuDevices);
                    }
                    catch
                    {
                        ViDiControl = new ViDi2.Runtime.Local.Control();
                    }
                    break;
            }
        }

        private void Multi_Click(object sender, RoutedEventArgs e)
        {
            if (ViDiControl != null) ViDiControl.Dispose();
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            int gpuCount = mos.Get().Count;

            switch (gpuCount)
            {
                case 0:
                    throw new Exception("GPU가 없습니다");
                case 1:
                    ViDiControl = new ViDi2.Runtime.Local.Control();
                    break;
                default:
                    try
                    {
                        List<int> gpuDevices = new List<int>();
                        for (int i = 0; i < gpuCount; i++) gpuDevices.Add(i);
                        ViDiControl = new ViDi2.Runtime.Local.Control(ViDi2.GpuMode.MultipleDevicesPerTool, gpuDevices);
                    }
                    catch
                    {
                        ViDiControl = new ViDi2.Runtime.Local.Control();
                    }
                    break;
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "vrws | *.vrws" };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            if (ViDiControl.Workspaces.Count != 0)
            {
                foreach (var ws in ViDiControl.Workspaces)
                {
                    ViDiControl.Workspaces.Remove(ws.DisplayName);
                }
            }

            using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open)) //귀찮으니 워크스페이스 두개로.
            {
                WSName = dialog.FileName.Split('\\').Last().Split('.').First();
                ViDiControl.Workspaces.Add(WSName, stream);
                ViDiControl.Workspaces.Add(WSName + "1", stream);
            }


        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            Thread thread = new Thread(new ThreadStart(delegate
            {
                string[] files = Directory.GetFiles(dialog.SelectedPath).ToList().FindAll(a => a.ToUpper().Contains("BMP")).ToArray();
                count = files.Length;

                Stopwatch watch = new Stopwatch();
                watch.Start();

                bool flag = false;

                foreach (string file in files)
                {
                    if (flag)
                        FirstQueue.Enqueue(file);
                    else
                        SecondQueue.Enqueue(file);

                    flag = !flag;
                }

                watch.Stop();
                AverageTime = (int)watch.ElapsedMilliseconds / files.Length;
            }));
        }

        private Queue<string> FirstQueue = new Queue<string>();
        private Queue<string> SecondQueue = new Queue<string>();

        private void FirstThreadMethod()
        {
            while (true)
            {
                if (FirstQueue.Count == 0)
                {
                    Thread.Sleep(5);
                    continue;
                }
                ViDiControl.Workspaces[WSName].Streams.First().Process(new ViDi2.FormsImage(new Bitmap(FirstQueue.Dequeue())));
                count--;
            }

        }

        private void SecondThreadMethod()
        {
            while (true)
            {
                if (SecondQueue.Count == 0)
                {
                    Thread.Sleep(5);
                    continue;
                }
                ViDiControl.Workspaces[WSName + "1"].Streams.First().Process(new ViDi2.FormsImage(new Bitmap(SecondQueue.Dequeue())));
                count--;
            }

        }

    }
}
