using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using ViDi2;

namespace ViDIRuntimeExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViDi2.Runtime.Local.Control m_Control;
        public ViDi2.Runtime.IWorkspace m_Workspace;
        public ViDi2.Runtime.IStream m_Stream;

        public MainWindow()
        {
            InitializeComponent();

            m_Control = new ViDi2.Runtime.Local.Control();

            m_Control.Workspaces.Add("jesus", @"C:\Users\jkhong\Desktop\IDontKnow3.vrws");
            m_Workspace = m_Control.Workspaces.First();
            m_Stream = m_Workspace.Streams.First();

            using (IImage image = new ViDi2.VisionPro.Image(new CogImage24PlanarColor(new Bitmap(@"C:\Users\jkhong\Desktop\라벨.bmp"))))
            {
                ISample sample = m_Stream.Process(image);

                IRedMarking red = null;
                IImage result = null;
                foreach (var a in sample.Markings)
                {
                    red = a.Value as IRedMarking;

                    result = red.OverlayImage(0, "");

                    result.Bitmap.Save(@"C:\Users\jkhong\Desktop\babo.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                }

                IImage resultOriginal = red.ViewImage(0);

                //var aa = red.Views[0] as IRedView;

                //aa.

                //result = aa.HeatMap;

                result.Bitmap.Save(@"C:\Users\jkhong\Desktop\babo.bmp",System.Drawing.Imaging.ImageFormat.Bmp);

                imageControl.Source = Convert(resultOriginal.Bitmap);
                imageControl_Copy.Source = Convert(result.Bitmap);
            }

        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();

            return image;
        }
    }
}
