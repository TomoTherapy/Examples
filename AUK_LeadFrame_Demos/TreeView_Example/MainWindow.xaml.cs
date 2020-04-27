using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TreeView_Example
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TreeItem> TreeItems { get; set; }
        public ObservableCollection<string> TypeCollection { get; set; }
        public string ModelName { get; set; }
        public string SelectedItem { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            TreeItems = new ObservableCollection<TreeItem>();
            TypeCollection = new ObservableCollection<string>();
            TypeCollection.Add("PS");
            TypeCollection.Add("삼단자");
            TypeCollection.Add("Other");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TreeItems.Any(item => item.Name == SelectedItem))
            {
                var child = TreeItems.Single(item => item.Name == SelectedItem);
                if (!child.Children.Any(item => item.Name == ModelName))
                {
                    child.Children.Add(new TreeItem { Name = ModelName });
                }
            }
            else
            {
                var tmpItem = new TreeItem
                {
                    Name = SelectedItem
                };
                tmpItem.Children.Add(new TreeItem { Name = ModelName });
                TreeItems.Add(tmpItem);
            }
        }
    }

    public class TreeItem : INotifyPropertyChanged
    {
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string pName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ObservableCollection<TreeItem> Children { get; set; }

        public TreeItem()
        {
            Children = new ObservableCollection<TreeItem>();
        }
    }
}
