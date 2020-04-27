using System.Collections.Generic;
using System.Linq;
using ViDi2;
using System.Windows;
using System.Windows.Forms;

namespace ViDiTest
{
    using System;
    using System.Drawing;
    using System.IO;
    using ViDi2.Training;
    using MessageBox = System.Windows.MessageBox;

    class ViDi
    {
        IControl control;
        IWorkspace workspace;
        IStream stream;
        IBlueTool blue;
        IRedTool red;
        IGreenTool green;

        public ViDi()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog() { RootFolder = System.Environment.SpecialFolder.Desktop };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ViDi2.Training.Local.WorkspaceDirectory workspaceDirectory = new ViDi2.Training.Local.WorkspaceDirectory()
                {
                    Path = @"D:\ViDi_Workspace\3.4"
                };

                var libraryAccess = new ViDi2.Training.Local.LibraryAccess(workspaceDirectory);

                control = new ViDi2.Training.Local.Control(libraryAccess, GpuMode.SingleDevicePerTool, new List<int>());
            }

        }


        public void CreateWorkspace()
        {
            string name = "";
            if (ShowInputDialog("Workspace Name", ref name) == DialogResult.OK)
            {
                workspace = control.Workspaces.Add(name);
            }
        }

        public void CreateStream()
        {
            string name = "";
            if (ShowInputDialog("Workspace Name", ref name) == DialogResult.OK)
            {
                stream = workspace.Streams.Add(name);
            }
        }

        public void CreateTool()
        {
            string name = "";
            if (ShowInputDialog("Workspace Name", ref name) == DialogResult.OK)
            {
                red = stream.Tools.Add(name, ToolType.Red) as IRedTool;
            }
        }

        public void Save()
        {
            workspace.Save();
        }

        public void Jesus()
        {
            workspace = control.Workspaces.Add("workspace");

            stream = workspace.Streams.Add("default");

            blue = stream.Tools.Add("localize", ToolType.Blue) as IBlueTool;
            red = stream.Tools.Add("analyze", ToolType.Red) as IRedTool;
            green = stream.Tools.Add("classify", ToolType.Green) as IGreenTool;
        }

        public void GetImages()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog() { RootFolder = System.Environment.SpecialFolder.Desktop };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var ext = new System.Collections.Generic.List<string> { ".jpg", ".bmp", ".png" };
                var myImagesFiles = Directory.GetFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories).Where(s => ext.Any(e => s.EndsWith(e)));
                foreach (var file in myImagesFiles)
                {
                    using (var image = new FormsImage(file))
                    {
                        red.Database.AddImage(image, Path.GetFileName(file));
                    }
                }
            }
        }

        public void SetRoi()
        {
            if (red.RegionOfInterest is IRedRegionOfInterest)
            {
                MessageBox.Show("IRedRegionOfInterest!");
            }
            else if(red.RegionOfInterest is IBlueRegionOfInterest)
            {
                MessageBox.Show("IBlueRegionOfInterest!");
            }
            else if (red.RegionOfInterest is IGreenRegionOfInterest)
            {
                MessageBox.Show("IGreenRegionOfInterest!");
            }
            else if (red.RegionOfInterest is IManualRegionOfInterest)
            {
                MessageBox.Show("IManualRegionOfInterest!!!");
            }
            else if(red.RegionOfInterest is IRegionOfInterest)
            {
                MessageBox.Show("IRegionOfInterest!");
            }

            var redRoi = red.RegionOfInterest as IManualRegionOfInterest;

            redRoi.Size = new ViDi2.Size(0.777, 0.777);
            redRoi.Offset = new ViDi2.Point(0.12, 0.12);

            //red.RegionOfInterest.Scale = new ViDi2.Size(0.5, 0.4); //스케일이라 함은 뭔가 내가생각하는 것과 다른 그것
            

            red.Database.Process();
            red.Wait();
            workspace.Save();

        }

        public void SetMask()
        {
            OpenFileDialog dialog = new OpenFileDialog() { InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Multiselect = false };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                IImage mask = new FormsImage(dialog.FileName);
                red.Database.SetViewMask("", mask);

                
            }
        }

        public void SetSet()
        {
            string filter = "";
            if (ShowInputDialog("Filter", ref filter) == DialogResult.OK)
            {
                string name = "";
                if (ShowInputDialog("SetName", ref name) == DialogResult.OK)
                {
                    red.Database.AddSamplesToSet("'" + filter + "'", name);
                }
            }

        }

        public void LabelViews()
        {
            red.Database.LabelViews("''","");
        }


        public void SettingTools()
        {
            bool local = true; //changes this to false if you want to connect to a runtime service
                               //remote capabilities are only available with advanced licenses
            string remoteServerAddress = "http://localhost:8080";
            // create a new control
            IControl control = null;

            //----------------------------------------------
            //--------Local usage---------------------------
            //----------------------------------------------
            if (local)
            {
                var workspaceDirectory = new ViDi2.Training.Local.WorkspaceDirectory()
                {
                    Path = "c:\\ViDi" //path where the workspaces are stored
                };

                var libraryAccess = new ViDi2.Training.Local.LibraryAccess(workspaceDirectory);

                // holds the main control
                control = new ViDi2.Training.Local.Control(libraryAccess, GpuMode.SingleDevicePerTool, new List<int>());
            }
            //----------------------------------------------
            //--------Connection to a Remote Service--------
            //----------------------------------------------
            else
            {
                var remoteControl = new ViDi2.Training.Remote.Client.Http.HttpControl(ViDi2.FormsImage.Factory);
                try
                {
                    remoteControl.Connect(remoteServerAddress); //specifies the IP address + port of the running training server

                    remoteControl.ConnectionMonitor.ServerTimedOut += (e, a) => System.Console.WriteLine("server disconnected");
                    control = remoteControl;
                }
                catch (TimeoutException)
                {
                    System.Console.WriteLine("failed to connect to service");
                    remoteControl.Dispose(); //you must dispose the control and create a new one to retry a connection
                    return;
                }
                control = remoteControl;
            }


            // creates a new workspace
            var workspace = control.Workspaces.Add("workspace");

            //creates a stream
            var stream = workspace.Streams.Add("default");

            //creates a blue tool at the root of the tool chain
            var blue = stream.Tools.Add("localize", ToolType.Blue) as IBlueTool;
            //creates a red tool linked to the blue tool
            var red = blue.Children.Add("analyze", ToolType.Red) as IRedTool;
            //creates a green tool at the root of the toolchain
            var green = stream.Tools.Add("classify", ToolType.Green) as IGreenTool;

            //loading images from local directory
            var ext = new System.Collections.Generic.List<string> { ".jpg", ".bmp", ".png" };
            var myImagesFiles = Directory.GetFiles(@"C:\Users\jkhong\Pictures\ZZZZ\", "*.*", SearchOption.AllDirectories).Where(s => ext.Any(e => s.EndsWith(e)));
            foreach (var file in myImagesFiles)
            {
                using (var image = new FormsImage(file))
                {
                    stream.Database.AddImage(image, Path.GetFileName(file));
                }
            }

            //---------------BLUE TOOL----------------------

            //modifying the ROI
            IManualRegionOfInterest blueROI = blue.RegionOfInterest as IManualRegionOfInterest; //gets the region of interest
            //changing the angle
            blueROI.Angle = 10.0;

            //processes all images in order to apply the ROI
            blue.Database.Process();
            //waiting fo the end of the processing
            blue.Wait();

            //get the first view in the database
            var firstView = blue.Database.List().First();

            //add some features to the first view in the database
            blue.Database.AddFeature(firstView, "0", new ViDi2.Point(firstView.Size.Width / 2, firstView.Size.Height / 2), 0.0, 1.0);
            blue.Database.AddFeature(firstView, "1", new ViDi2.Point(firstView.Size.Width / 3, firstView.Size.Height / 3), 0.0, 1.0);

            //adding a model to the blue tool
            var model = blue.Models.Add("model1") as INodeModel;
            //adding some nodes in the model
            var node = model.Nodes.Add();
            node.Fielding = new List<string> { "1" };
            node.Position = new ViDi2.Point(0.0, 0.0);
            node = model.Nodes.Add();
            node.Fielding = new List<string> { "0" };
            node.Position = new ViDi2.Point(1.0, 0.0);

            //changing some parameters
            blue.Parameters.FeatureSize = new ViDi2.Size(30, 30);

            //saving the workspace
            workspace.Save();

            //trains and wait for the training to be finished
            blue.Train();

            try
            {
                while (!blue.Wait(1000))
                {
                    System.Console.WriteLine(blue.Progress.Description + " " + blue.Progress.ETA.ToString());
                }
            }
            catch (ViDi2.Exception e)
            {
                /* you'll likely get a "numeric instability detected" exception
                 * that will put you right here. That happens because the resources
                 * that are being used from the "images" folder are probably not
                 * well suited for the specific stream that we have set up.
                 */
                System.Console.WriteLine(e.Message);
                return;
            }

            var blueSummary = blue.Database.Summary();

            //---------------RED TOOL----------------------

            //setting the roi in the red tool. It is a IBlueRegionOfInterest because the red tool is linked to a blue tool
            var redRoi = red.RegionOfInterest as IBlueRegionOfInterest;
            //selecting the model used for the ROI
            redRoi.MatchFilter = "name='" + model.Name + "'";
            //setting the size of the ROI
            redRoi.Size = new ViDi2.Size(100, 100);

            //applying the ROI on all images
            red.Database.Process();

            //waiting for red tool to finish applying ROI
            red.Wait();

            //changing the rotation perturbation parameter
            red.Parameters.Rotation = new System.Collections.Generic.List<Interval>() { new Interval(0.0, 360.0) };

            //labellling images
            red.Database.LabelViews("'bad'", "bad"); //label good images
            red.Database.LabelViews("not 'bad'", ""); // label bad images
            workspace.Save();

            //training the workspace
            red.Train();
            try
            {
                while (!red.Wait(1000))
                {
                    System.Console.WriteLine(red.Progress.Description + " " + red.Progress.ETA.ToString());
                }
            }
            catch (ViDi2.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            var redSummary = red.Database.Summary();

            //---------------GREEN TOOL----------------------

            //Applying the ROI to the green tool
            green.Database.Process();
            red.Wait();

            //tagging the images
            green.Database.Tag("'bad'", "b");
            green.Database.Tag("not 'bad'", "g");

            workspace.Save();
            green.Train();
            try
            {
                while (!green.Wait(1000))
                {
                    System.Console.WriteLine(green.Progress.Description + " " + green.Progress.ETA.ToString());
                }
            }
            catch (ViDi2.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            var greenSummary = green.Database.Summary();

            //closing the workspaces
            foreach (var w in control.Workspaces)
            {
                w.Close();
            }
            control.Dispose();
        }

        public void TrainRed()
        {
            red.Train();
            try
            {
                while (!red.Wait(1000))
                {
                    //red.Progress.Description + " " + red.Progress.ETA.ToString();
                }
            }
            catch (ViDi2.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            var redSummary = red.Database.Summary();
        }

        private static DialogResult ShowInputDialog(string title, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(300, 100);
            System.Windows.Forms.Form inputBox = new System.Windows.Forms.Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = title
            };

            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
            {
                Size = new System.Drawing.Size(size.Width - 20, 18),
                Location = new System.Drawing.Point(10, 30),
                Text = input,
                Font = new Font("Arial", 12)
            };
            inputBox.Controls.Add(textBox);

            System.Windows.Forms.Button okButton = new System.Windows.Forms.Button
            {
                DialogResult = System.Windows.Forms.DialogResult.OK,
                Name = "okButton",
                Size = new System.Drawing.Size(100, 25),
                Text = "&OK",
                Location = new System.Drawing.Point(size.Width - 130 - 130, 60)
            };
            inputBox.Controls.Add(okButton);

            System.Windows.Forms.Button cancelButton = new System.Windows.Forms.Button
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel,
                Name = "cancelButton",
                Size = new System.Drawing.Size(100, 25),
                Text = "&Cancel",
                Location = new System.Drawing.Point(size.Width - 130, 60)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

    }
}
