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

namespace StackPanelBind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string TxtName { get; set; }

        //리스트뷰 모델 생성
        MyListViewModel listViewModel = new MyListViewModel();

        public MainWindow()
        {
            InitializeComponent();

            listView.DataContext = listViewModel;
            listView.ItemsSource = listViewModel.MyList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listViewModel.MyList.Add(new MyItem(TxtName));

            listViewModel.MyList[0].TextName = "지져쓰";
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}
