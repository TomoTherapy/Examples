using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DataContextXmlInitialize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Jesus_button_Click(object sender, RoutedEventArgs e)
        {
            DataContextExample e1 = FindResource("DataContextClass1") as DataContextExample;
            DataContextExample e2 = FindResource("DataContextClass2") as DataContextExample;

            e1.DataContext_InnerText = "Jesus Christ";
            e2.DataContext_InnerText = "Allahu Akbar!!";
        }
    }

    public class DataContextExample : INotifyPropertyChanged
    {
        private string m_InnerText;
        public string DataContext_InnerText
        {
            get { return m_InnerText; }
            set
            {
                m_InnerText = value;
                RaisePropertyChanged("DataContext_InnerText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
