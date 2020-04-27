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

namespace BegindCodeBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*
             * 바인딩 소스 = textValue
             * 바인딩 소스 경로 = textValue.Text
             * 바인딩 대상 = bindValue
             * 바인딩 대상의 속성 = bindValue.Text
             */

            //Path=Text
            Binding binding = new Binding("Text");
            //ElementName=textValue
            binding.Source = textValue;

            //Text={Binding Path=Text, ElementName=textValue}
            bindValue.SetBinding(TextBox.TextProperty, binding);

            //??? 왜 안될까??
        }
    }
}
