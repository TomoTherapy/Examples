using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace DataGridAndDataTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _resultDT = GetSampleData();
            this.DataContext = this;
        }

        private DataTable _resultDT;
        public DataTable ResultDT
        {
            get { return _resultDT; }
            set { _resultDT = value; }
        }

        private DataTable GetSampleData()
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Rank", typeof(string));
            table.Columns.Add("Status", typeof(int));

            table.Rows.Add("0", "Jesus", "Saved his people from their sin by sacrifying himself" ,"God's right hand" , -1);
            table.Rows.Add("1", "Christ", "Saved his people. he's actually Jesus' brother", "God's left hand", 0);
            table.Rows.Add("2", "Allah", "Helped a lot of suicide bomb terrorist", "Almighty", 1);

            return table;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataTemplate dt = null;

            if (e.PropertyName == "Status")
            {
                dt = Resources["StateTemplate"] as DataTemplate;
            }

            if (dt != null)
            {
                DataGridTemplateColumn c = new DataGridTemplateColumn()
                {
                    CellTemplate = dt,
                    Header = e.Column.Header,
                    HeaderTemplate = e.Column.HeaderTemplate,
                    HeaderStringFormat = e.Column.HeaderStringFormat,
                    SortMemberPath = e.PropertyName
                };
                e.Column = c;
            }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DataGridCell)
            {
                DataGridCell cell = e.OriginalSource as DataGridCell;

                if (cell.Content is TextBlock)
                {
                    TextBlock block = cell.Content as TextBlock;
                    MessageBox.Show(block.Text);
                }
                else if (cell.Content is Ellipse)
                {
                    Ellipse Ell = cell.Content as Ellipse;
                    MessageBox.Show(Ell.Fill.ToString());
                }
            }
        }
    }



    public class StateProp
    {
        public string Visibility { get; set; }
        public string Color { get; set; }
        public StateProp(object data)
        {
            if (data is int)
            {
                switch ((int)data)
                {
                    case -1:
                        Visibility = "Visibility";
                        Color = "Red";
                        break;
                    case 0:
                        Visibility = "Visibility";
                        Color = "Orange";
                        break;
                    case 1:
                        Visibility = "Visibility";
                        Color = "Green";
                        break;
                    default:
                        Visibility = "Hidden";
                        break;
                }
            }

        }
    }

    public class DataRowViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridCell cell = value as DataGridCell;
            if (cell == null) return null;

            DataRowView drv = cell.DataContext as DataRowView;
            if (drv == null) return null;

            return new StateProp(drv.Row[cell.Column.SortMemberPath]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
