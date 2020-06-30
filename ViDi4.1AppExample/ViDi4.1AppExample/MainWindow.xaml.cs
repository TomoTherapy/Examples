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
using ViDi2;

namespace ViDi4._1AppExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViDi2.Training.Local.Control control;
        private ViDi2.Training.IWorkspace workspace;
        private ViDi2.Training.IStream stream;
        private ViDi2.Training.IBlueTool blue;

        public MainWindow()
        {
            InitializeComponent();

        }

        public void OpenControl()
        {
            var workspaceDirectory = new ViDi2.Training.Local.WorkspaceDirectory()
            {
                Path = "c:\\ViDi"
            };
            var libraryAccess = new ViDi2.Training.Local.LibraryAccess(workspaceDirectory);
            control = new ViDi2.Training.Local.Control(libraryAccess, GpuMode.SingleDevicePerTool, new List<int>());
        }

        public void CreateWorkspace(string name)
        {
            workspace = control.Workspaces.Add(name);
        }

        public void CreateStream(string name)
        {
            stream = workspace.Streams.Add(name);
        }

        public void CreateBlueTool(string name)
        {
            blue = stream.Tools.Add(name, ViDi2.ToolType.Blue) as ViDi2.Training.IBlueTool;
        }

        public void ChangeLegacyMode(bool check)
        {
            blue.Parameters.GetType().GetProperty("LegacyMode").SetValue(blue.Parameters, check);
        }

        public void SaveWorkspace()
        {
            workspace.Save();
        }
    }
}
