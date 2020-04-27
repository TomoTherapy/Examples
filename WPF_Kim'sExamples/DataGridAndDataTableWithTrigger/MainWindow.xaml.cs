using System;
using System.Collections.Generic;
using System.Data;
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

namespace DataGridAndDataTableWithTrigger
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

        DataTable _resultDT;
        public DataTable ResultDT
        {
            get { return _resultDT; }
            set { _resultDT = value; /*RaisePropertyChanged("ResultDT");*/ }
        }

        private DataTable GetSampleData()
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Rank", typeof(string));
            table.Columns.Add("Status", typeof(int));

            table.Rows.Add("0", "Jesus", "Saved his people from their sin by sacrifying himself", "God's right hand", -1);
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
    }
}
