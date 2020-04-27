using System;
using System.Collections.Generic;
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
using System.Xml.Serialization;

namespace ViDiTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ViDi vidi;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vidi = new ViDi();
        }
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        object main = new Main();
        XmlSerializer xml;
        private void XmlTestSave()
        {
            xml = new XmlSerializer(main.GetType());
            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"ConInfo.xml");
            xml.Serialize(sw, main);
            sw.Dispose();
        }

        private void XmlTestLoad()
        {
            xml = new XmlSerializer(main.GetType());
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"ConInfo.xml");
            main = xml.Deserialize(sr) as Main;
            sr.Dispose();
        }

        private void XmlSave_button_Click(object sender, RoutedEventArgs e)
        {
            XmlTestSave();
        }

        private void XmlLoad_button_Click(object sender, RoutedEventArgs e)
        {
            XmlTestLoad();
        }
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        
        private void CreateWorkspace_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.CreateWorkspace();
            WriteOuput("Workspace Created");
        }

        private void CreateStream_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.CreateStream();
            WriteOuput("Stream Created");
        }

        private void CreateRedTool_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.CreateTool();
            WriteOuput("Red Tool Created");
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.Save();
            WriteOuput("Saved");
        }

        private void GetImage_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.GetImages();
            WriteOuput("Images Added");
        }

        private void SetRoi_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.SetRoi();
            WriteOuput("ROI Set");
        }

        private void SetMask_button_Click(object sender, RoutedEventArgs e)
        {
            vidi.SetMask();
            WriteOuput("Mask Set");
        }

        private void WriteOuput(string text)
        {
            Output_textBlock.Text += "\n" + text.Trim();
            Scroll.ScrollToEnd();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            vidi.SettingTools();
        }
    }

    public class Main
    {
        //반드시 public을 먹여줘야한다.
        public int Jesus { get; set; } = 1;
        public int Christ { get; set; } = 2;
        public int Allah { get; set; } = 3;
        public int Buddga { get; set; } = 4;

        //클래스의 경우 new를 통해 반드시 생성을 해줘야한다.
        public PlusLeft PlusL1 { get; set; } = new PlusLeft();
        public PlusLeft PlusL2 { get; set; } = new PlusLeft();
        public PlusRight PlusR { get; set; } = new PlusRight();
    }

    public class PlusLeft
    {
        public bool[] Bypass { get; set; } = { true, true, true, true };
        public double[] Threshold { get; set; } = { 11.1, 22.2, 33.3, 44.4 };
    }

    public class PlusRight
    {
        public bool[] Bypass { get; set; } = { true, true, true, true };
        public double[] Threshold { get; set; } = { 11.1, 22.2, 33.3, 44.4 };
        public int Jesus { get; set; } = 321312454;
    }

}
