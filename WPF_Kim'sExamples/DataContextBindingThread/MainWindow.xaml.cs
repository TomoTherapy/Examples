using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DataContextBindingThread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string _textBox1;
        public string TextBox1
        {
            get
            {
                return this._textBox1;
            }
            set
            {
                if (this._textBox1 == value) return;
                this._textBox1 = value;
                this.NotifyPropertyChanged("TextBox1");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.TextBox1 = "aaa";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(Jesus);

            thread.Start();
            thread.Join();
        }

        private void Jesus()
        {
            TextBox1 = "Jesus Christ!!";
        }
    }
}
