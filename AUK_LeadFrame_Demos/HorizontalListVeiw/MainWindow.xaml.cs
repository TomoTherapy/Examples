using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HorizontalListVeiw
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public string TxtName { get; set; }
       
        //리스트 뷰 모델 생성
        //MyListViewModel listViewModel = new MyListViewModel();

        public MainWindow()
        {
            InitializeComponent();

            //리스트 항목 추가
            MyListViewModel listViewModel = FindResource("MyListViewModel") as MyListViewModel;
            listViewModel.MyList.Add(new MyItem("송병호"));
            listViewModel.MyList.Add(new MyItem("김태균"));
            listViewModel.MyList.Add(new MyItem("김진영"));

            //리스트뷰 바인딩
            //lvList.DataContext = listViewModel;         //모델 바인딩
            //lvList.ItemsSource = listViewModel.MyList;  //아이템 소스 바인딩
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //리스트 항목 추가
            MyListViewModel listViewModel = FindResource("MyListViewModel") as MyListViewModel;
            listViewModel.MyList.Add(new MyItem(TxtName));
        }

    }

    
}
