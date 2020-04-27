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

namespace TreeViewExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class Employee
    {
        public string Name { get; set; }
        public List<Employee> ManagedEmployees { get; set; }

        public Employee(string name)
        {
            Name = name;
            ManagedEmployees = new List<Employee>();
        }
    }

    public class EmployeeType
    {
        public string Description { get; set; }
        public List<Employee> Employees { get; set; }

        public EmployeeType(string description)
        {
            Description = description;
            Employees = new List<Employee>();
        }
    }

    public class EmployeeTypes : List<EmployeeType>
    {
        public EmployeeTypes()
        {
            EmployeeType type;
            Employee emp;
            Employee managed;

            type = new EmployeeType("Manager");
            emp = new Employee("Michael");
            managed = new Employee("John");
            emp.ManagedEmployees.Add(managed);
            managed = new Employee("Tim");
            emp.ManagedEmployees.Add(managed);
            type.Employees.Add(emp);

            emp = new Employee("Paul");
            managed = new Employee("Michael");
            emp.ManagedEmployees.Add(managed);
            managed = new Employee("Cindy");
            emp.ManagedEmployees.Add(managed);
            type.Employees.Add(emp);
            this.Add(type);

            type = new EmployeeType("Project Managers");
            type.Employees.Add(new Employee("Tim"));
            type.Employees.Add(new Employee("John"));
            type.Employees.Add(new Employee("David"));
            this.Add(type);
        }
    }

}
