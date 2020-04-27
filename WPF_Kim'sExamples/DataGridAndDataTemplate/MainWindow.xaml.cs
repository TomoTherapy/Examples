using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataGridAndDataTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataTable dt = new DataTable();
            dt.Columns.Add("StringColumn", typeof(string));
            dt.Columns.Add("IntColumn", typeof(int));
            dt.Columns.Add("AColumn1", typeof(A));
            dt.Columns.Add("AColumn2", typeof(A));
            dt.Columns.Add("BColumn1", typeof(B));
            dt.Columns.Add("BColumn2", typeof(B));

            dt.Rows.Add(
                "TestString",
                123,
                new A() { Name = "A10", GroupName = "GroupName", IsSelected = true },
                new A() { Name = "A20", GroupName = "GroupName", IsSelected = false },
                new B() { FullName = "B10", IsChecked = true },
                new B() { FullName = "B20", IsChecked = false }
            );

            dt.Rows.Add(
                "TestString2",
                1232,
                new A() { Name = "A1", GroupName = "GroupName2", IsSelected = true },
                new A() { Name = "A2", GroupName = "GroupName2", IsSelected = false },
                new B() { FullName = "B1", IsChecked = true },
                new B() { FullName = "B2", IsChecked = false }
            );

            Items = dt;
            this.DataContext = this;
        }

        public DataTable Items { get; set; }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataTemplate dt = null;
            if (e.PropertyType == typeof(A))
                dt = (DataTemplate)Resources["ATemplate"];
            else if (e.PropertyType == typeof(B))
                dt = (DataTemplate)Resources["BTemplate"];

            if (dt != null)
            {
                DataGridTemplateColumn c = new DataGridTemplateColumn()
                {
                    CellTemplate = dt,
                    Header = e.Column.Header,
                    HeaderTemplate = e.Column.HeaderTemplate,
                    HeaderStringFormat = e.Column.HeaderStringFormat,
                    SortMemberPath = e.PropertyName // this is used to index into the DataRowView so it MUST be the property's name (for this implementation anyways)
                };
                e.Column = c;
            }
        }
    }

    public class A
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class B
    {
        public string FullName { get; set; }
        public bool IsChecked { get; set; }
    }

    public class DataRowViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DataGridCell cell))
                return null;

            if (!(cell.DataContext is DataRowView drv))
                return null;

            return drv.Row[cell.Column.SortMemberPath];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
