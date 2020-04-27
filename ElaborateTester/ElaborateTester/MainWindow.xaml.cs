using System;
using System.Windows;
using System.Threading;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro;
using System.IO;
using Cognex.VisionPro.Implementation;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;

namespace ElaborateTester
{
    public partial class MainWindow : Window
    {
        CogRecordDisplay display = new CogRecordDisplay();
        CogBlobTool blob = new CogBlobTool();
        CameraControl camControl = new CameraControl();
        Thread mainTask;
        CogBlobEditV2 blobEdit = new CogBlobEditV2();

        public MainWindow()
        {
            InitializeComponent();

            CogDisplayWPF.Child = display;
            blobEdit.Subject = blob;
            BlobEditorWPF.Child = blobEdit;

            camControl.InitSystem();
        }

        bool flag = false;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!flag)
            {
                camControl.OpenDeviceAquisitionStart();

                flag = !flag;
                mainTask = new Thread(new ThreadStart(MainTaskThread));
                mainTask.Start();

            }
            else
            {
                flag = !flag;
            }
        }

        public void MainTaskThread()
        {
            int count = 0;

            double constX = 0;
            double constY = 0;

            while (flag)
            {
                CogImage8Grey image = new CogImage8Grey();
                image = camControl.CamGrab();

                if (image == null) continue;

                blob.InputImage = image;
                blob.Run();
                double xxx = 0;
                double yyy = 0;
                if (blob.Results.GetBlobs().Count != 0)
                {
                    xxx = blob.Results.GetBlobs()[0].CenterOfMassX;
                    yyy = blob.Results.GetBlobs()[0].CenterOfMassY;
                }

                CogRecord record = new CogRecord();
                record.SubRecords.Add(blob.CreateLastRunRecord().SubRecords[0]);

                if (count < 10)
                {
                    constX += xxx;
                    constY += yyy;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        MinX_textBox.Text = xxx.ToString();
                        MaxX_textBox.Text = xxx.ToString();
                        MinY_textBox.Text = yyy.ToString();
                        MaxY_textBox.Text = yyy.ToString();
                    }));
                }
                else if (count == 10)
                {
                    constX = constX / 10;
                    constY = constY / 10;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        StdX_textBox.Text = constX.ToString();
                        StdY_textBox.Text = constY.ToString();
                        MinX_textBox.Text = xxx.ToString();
                        MaxX_textBox.Text = xxx.ToString();
                        MinY_textBox.Text = yyy.ToString();
                        MaxY_textBox.Text = yyy.ToString();
                    }));
                }
                else if (count % 60 == 0)
                {
                    var bmp8bpp = Grayscale.CommonAlgorithms.BT709.Apply(image.ToBitmap());
                    bmp8bpp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SavedImages\Routine_SavedImage-" + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".bmp", ImageFormat.Bmp);

                    Dispatcher.Invoke(new Action(() => {

                        double minx = double.Parse(MinX_textBox.Text);
                        double miny = double.Parse(MinY_textBox.Text);
                        double maxx = double.Parse(MaxX_textBox.Text);
                        double maxy = double.Parse(MaxY_textBox.Text);

                        if (xxx < minx) MinX_textBox.Text = xxx.ToString();
                        if (yyy < miny) MinY_textBox.Text = yyy.ToString();
                        if (xxx > maxx) MaxX_textBox.Text = xxx.ToString();
                        if (yyy > maxy) MaxY_textBox.Text = yyy.ToString();

                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => {
                        if (Math.Abs(constX - xxx) >= 0.5 || Math.Abs(constY - yyy) >= 0.5)
                        {
                            int a = (int.Parse(AbnormalCount_textBox.Text) + 1);
                            AbnormalCount_textBox.Text = a.ToString();
                        }
                        var bmp8bpp = Grayscale.CommonAlgorithms.BT709.Apply(image.ToBitmap());
                        bmp8bpp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SavedImages\NG_SavedImage-" + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".bmp", ImageFormat.Bmp);

                        double minx = double.Parse(MinX_textBox.Text);
                        double miny = double.Parse(MinY_textBox.Text);
                        double maxx = double.Parse(MaxX_textBox.Text);
                        double maxy = double.Parse(MaxY_textBox.Text);

                        if (xxx < minx) MinX_textBox.Text = xxx.ToString();
                        if (yyy < miny) MinY_textBox.Text = yyy.ToString();
                        if (xxx > maxx) MaxX_textBox.Text = xxx.ToString();
                        if (yyy > maxy) MaxY_textBox.Text = yyy.ToString();

                    }));
                }

                count++;
                Dispatcher.Invoke(new Action(() => {
                    TotalCount_textBox.Text = count.ToString();
                    display.Image = image;
                    display.Record = record;
                    display.Fit();
                }));

                TextWriter tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\SavedLog.csv", true);
                tw.WriteLine(count + "," + xxx + "," + yyy + "," + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
                tw.Close();
                Thread.Sleep(900);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CogSerializer.SaveObjectToFile(blob, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\tool.vpp");
            camControl.CloseDeviceAcquisitionStop();
            camControl.CamFree();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\tool.vpp"))
            {
                blob = CogSerializer.LoadObjectFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\tool.vpp") as CogBlobTool;
            }
        }

        private void TestImage_button_Click(object sender, RoutedEventArgs e)
        {
            CogImage8Grey image = new CogImage8Grey();
            image = camControl.CamGrab();

            if (image == null) MessageBox.Show("이미지 촬영 실패!");

            blob.InputImage = image;
            blob.Run();
        }
    }

    class XY
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
