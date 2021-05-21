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

namespace SomeSeriousComboBoxBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        private void ComboBoxX_DropDownClosed(object sender, EventArgs e)
        {
            (DataContext as ViewModel).ComboBoxX_DropDownClosed();
        }

        private void ComboBoxY_DropDownClosed(object sender, EventArgs e)
        {
            (DataContext as ViewModel).ComboBoxY_DropDownClosed();
        }

        private void ComboBoxMode_DropDownClosed(object sender, EventArgs e)
        {
            (DataContext as ViewModel).ComboBoxMode_DropDownClosed();
        }
    }
}
