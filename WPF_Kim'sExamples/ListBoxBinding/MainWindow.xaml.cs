using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ListBoxBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<User> Users { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Users = new ObservableCollection<User>();

            Users.Add(new User() { Name1 = "Jesus" });
            Users.Add(new User() { Name1 = "Christ" });
            Users.Add(new User() { Name1 = "Penis" });
            Users.Add(new User() { Name1 = "Vagina" });
            Users.Add(new User() { Name1 = "Reproduction Activity" });

            lbUsers.ItemsSource = Users;
            
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            Users.Add(new User() { Name1 = "Jesus Christ" });
        }

        private void BtnChangeUser_Click(object sender, RoutedEventArgs e)
        {
            if (lbUsers.SelectedItem != null)
            {
                User a = lbUsers.SelectedItem as User;
                a.Name1 = "AAAAAAAAAAAAAAAA";
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (lbUsers.SelectedItem != null)
            {
                Users.Remove(lbUsers.SelectedItem as User);
            }
        }
    }

    public class User : INotifyPropertyChanged
    {
        private string _name;
        public string Name1
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                if (_name is null) return;

                _name = value;
                NotifyPropertyChanged("Name1");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (!(PropertyChanged is null))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
