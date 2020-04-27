using DeepObjectDector.sub.control.ImageListView;
using DeepObjectDector.sub.doc;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using DeepObjectDector.sub.lib;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace DragAndDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        WorkSpaceClass m_WorkspaceDoc;
        private ConvTreeViewItem m_convTreeviewItem;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //List View Event Register
            IMG_LSTVIEW_UI.imageFileDrop += OnImageListViewDropEvent;
            IMG_LSTVIEW_UI.SelectionChanged += OnImageListViewSel;

            
        }

        private void CreateNewSolution()
        {
            //Workspace 
            m_WorkspaceDoc = new WorkSpaceClass();
            MAINTREE_UI.ItemsSource = m_WorkspaceDoc.p_Solutions;

            ProjectClass prj = new ProjectClass
            {
                p_Name = "Project1",
                p_Pareants = m_WorkspaceDoc.p_Solutions[0]
            };

            TaskClass tsk = new TaskClass
            {
                p_Name = "Task1",
                p_Pareants = prj
            };

            ToolClass tl = new ToolClass
            {
                p_Name = "Tool1",
                p_Pareants = tsk
            };

            m_WorkspaceDoc.p_Solutions[0].p_Projects.Add(prj);
            prj.p_Tasks.Add(tsk);
            tsk.p_Tools.Add(tl);

            m_convTreeviewItem = new ConvTreeViewItem();
        }

        private void SaveWorkspace()
        {
            string wsPath = "";
            StreamWriter sw = null;

            try
            {
                SaveFileDialog dialog = new SaveFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    DefaultExt = ".xml",
                    Filter = "xml Files (*.xml)|*.xml"
                };

                // Get the selected file name and display in a TextBox 
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    wsPath = dialog.FileName;

                    WorkSpaceClass sol = m_WorkspaceDoc;
                    sw = new StreamWriter(wsPath);
                    XmlSerializer x = new XmlSerializer(sol.GetType());
                    x.Serialize(sw, sol);
                    sw.Close();

                    MessageBox.Show("Save Complete");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadWorkspace()
        {
            WorkSpaceClass temp = new WorkSpaceClass();

            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                DefaultExt = ".xml",
                Filter = "xml Files (*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CreateNewSolution();
                StreamReader sr = new StreamReader(dialog.FileName);
                XmlSerializer x = new XmlSerializer(m_WorkspaceDoc.GetType());

                temp = x.Deserialize(sr) as WorkSpaceClass;

                if (temp == null) return;

                m_WorkspaceDoc = temp;

                MAINTREE_UI.ItemsSource = null;
                MAINTREE_UI.ItemsSource = m_WorkspaceDoc.p_Solutions;

                //LostFocuse때 SaveDoc을 하기위함.
                IMG_LSTVIEW_UI.Focusable = true;
                IMG_LSTVIEW_UI.Focus();

                inner_BottomSelTreeView();

                IMG_LSTVIEW_UI.SelectedIndex = 0;

                MessageBox.Show("Load Complete");
            }
        }

        private void inner_BottomSelTreeView()
        {
            try
            {
                object item = m_WorkspaceDoc.p_Solutions[0].p_Projects[0].p_Tasks[0].p_Tools[0];
                TreeViewItem trvItem = m_convTreeviewItem.TRV_GetTreeViewItem(MAINTREE_UI, item);
                trvItem.IsSelected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //이벤트 드랍 이벤트
        private void OnImageListViewDropEvent(object sender, RoutedEventArgs e)
        {
            int index = -1;
            ImageListView imgListView = (ImageListView)e.Source;
            List<ImageItem> items = new List<ImageItem>();
            try
            {
                IMG_LSTVIEW_UI.Focusable = true;
                IMG_LSTVIEW_UI.Focus();

                index = IMG_LSTVIEW_UI.SelectedIndex;//하는게 뭐야?
                object obj = MAINTREE_UI.SelectedItem;

                if(obj == null)
                {
                    MessageBox.Show("Task를 선택하세요");
                    return;
                }

                if(obj.GetType().ToString() == "DeepObjectDector.sub.doc.TaskClass")
                {
                    inner_CopyImagePathToTaskClass(imgListView.p_DropFiles, obj as TaskClass);

                    //List Update
                    items = SetImageListViewUpdate(obj);

                    inner_ImageItemsUpdateUI(items);

                    //UI 내부에 뭔가 더럽게 있다면 청소
                    //if (IMG_LSTVIEW_UI.ILV_GetImgItems().Count != 0)
                    //{
                    //    IMG_LSTVIEW_UI.ILV_Clearitem();
                    //}

                    //foreach (ImageItem item in items)
                    //{
                    //    IMG_LSTVIEW_UI.ILV_insertitem(item);
                    //}

                }
                else
                {
                    MessageBox.Show("Task를 선택해주세요");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void inner_CopyImagePathToTaskClass(string[] dropFiles, TaskClass task)
        {
            string[] tempPath = new string[dropFiles.Length];
            string fileName = null;
            TaskClass.IMAGEFILEPATH path;

            int i = 0;
            foreach (string dropfile in dropFiles)
            {
                path = new TaskClass.IMAGEFILEPATH();
                string dropFileName = ExtractDropFileName(dropfile);
                string newPath = m_WorkspaceDoc.localPath
                    + "\\" + task.p_Pareants.p_Pareants.p_Name
                    + "\\" + task.p_Pareants.p_Name
                    + "\\" + task.p_Name 
                    + "\\" + dropFileName;

                tempPath[i] = newPath;

                //Doc → taskClass
                path.targetPath = tempPath[i];
                path.originPath = dropfile;
                path.FileName = dropFileName;
                path.id = "IMG_" + ((UInt32)path.GetHashCode()).ToString();

                task.p_ImageFileKeys.Add((UInt32)path.GetHashCode());
                task.p_ImageFileMaps[(UInt32)path.GetHashCode()] = path;

                //
                List<ImageItem> items = new List<ImageItem>();

                foreach (UInt32 key in task.p_ImageFileKeys)
                {
                    path = task.p_ImageFileMaps[key];
                    ImageItem item = new ImageItem();
                    string filename = ExtractDropFileName(path.targetPath);
                    item.p_FileName = filename;
                    item.p_ImgId = uint.Parse(path.id.Split('_')[1]);
                    items.Add(item);
                }

                //if (IMG_LSTVIEW_UI.ILV_GetImgItems().Count != 0)
                //    IMG_LSTVIEW_UI.ILV_Clearitem();

                //foreach (ImageItem item in items)
                //{
                //    IMG_LSTVIEW_UI.ILV_insertitem(item);
                //}

                i++;
            }

        }

        private List<ImageItem> SetImageListViewUpdate(object obj)
        {
            List<ImageItem> items = new List<ImageItem>();

            TaskClass tskObj = obj as TaskClass;
            items = inner_SetCommonImageItem(tskObj);

            return items;
            
            //ImageItem item = null;

            //foreach (string path in paths)
            //{
            //    item = new ImageItem();
            //    item.p_Image_uri = path;
            //    item.p_FileName = ExtractDropFileName(path);
            //    items.Add(item);
            //}

            //return items;

        }

        private List<ImageItem> inner_SetCommonImageItem(TaskClass tsk)
        {
            TaskClass.IMAGEFILEPATH path;
            List<ImageItem> items = new List<ImageItem>();
            ImageItem item = null;
            String fileName = null;
            try
            {
                foreach (UInt32 key in tsk.p_ImageFileKeys)
                {
                    path = tsk.p_ImageFileMaps[key];
                    item = new ImageItem();
                    item.p_Image_uri = path.targetPath;
                    fileName = ExtractDropFileName(path.targetPath);//파일네임 추출
                    item.p_FileName = fileName;
                    item.p_ImgId = uint.Parse(path.id.Split('_')[1]);
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void inner_ImageItemsUpdateUI(List<ImageItem> items)
        {
            try
            {
                if (IMG_LSTVIEW_UI.ILV_GetImgItems().Count != 0)
                    IMG_LSTVIEW_UI.ILV_Clearitem();

                foreach (ImageItem item in items)
                {
                    IMG_LSTVIEW_UI.ILV_insertitem(item);
                }

                return;
            }
            catch (Exception err)
            {
                return;
            }
            finally
            { }
        }

        private string ExtractDropFileName(string path)
        {
            string[] fileInfo;
            string fileName = "";
            try
            {
                fileInfo = path.Split('\\');
                string comp = fileInfo[fileInfo.Length - 1];
                string[] compInfo = comp.Split('.');
                string compType = compInfo[compInfo.Length - 1];
                if (compType.ToLower().Equals("jpg") || compType.ToLower().Equals("png") || compType.ToLower().Equals("bmp"))
                {
                    fileName = comp;
                }
                else
                {
                    throw new Exception("이미지 형식 에러");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                
            }
            return fileName;
        }

        private bool inner_FilesImageValid(string[] dropfiles)
        {
            string[] tokenPath = null;
            string fileName = null;
            string[] fileInform = null;
            string fileType = null;
            try
            {
                foreach (string path in dropfiles)
                {
                    tokenPath = path.Split('\\');
                    fileName = tokenPath[tokenPath.Length - 1];
                    fileInform = fileName.Split('.');
                    fileType = fileInform[fileInform.Length - 1];

                    if (fileType.ToLower().Equals("jpg") || fileType.ToLower().Equals("png") || fileType.ToLower().Equals("bmp"))
                    {

                    }
                    else
                    {
                        throw new Exception("");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ZoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(cv);
                CvZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(cv);
                CvZoomOut(curContentMousePoint);
            }
        }

        private void OnImageListViewSel(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList list = e.AddedItems;
            if (list.Count == 0) return;

            ImageItem item = (ImageItem)list[0];//오직 첫번재 하나만 표시 - 라고는 하지만 애초에 선택을 복수로 할수없는걸?

            cv.Children.Clear();

            Uri path = new Uri(item.p_Image_uri);
            BitmapImage bitmap = new BitmapImage(path);

            Image image = new Image();
            image.Source = bitmap;
            image.Width = bitmap.Width;
            image.Height = bitmap.Height;

            cv.Width = bitmap.Width;
            cv.Height = bitmap.Height;
            cv.Children.Add(image);
            Canvas.GetLeft(image);
        }

        private void EmptyCanvas()
        {
            cv.Children.Clear();
        }

        private void CvZoomIn(Point point)
        {
            ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale + 0.1, point);
        }

        private void CvZoomOut(Point point)
        {
            ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale - 0.1, point);
        }

        private void cv_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void cv_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void cv_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cv_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void cv_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void MAINTREE_UI_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //MAINTREE_UI.SelectedItem
            if (e.NewValue is TaskClass)
            {
                inner_ImageItemsUpdateUI(SetImageListViewUpdate(e.NewValue));
            }
            else if (!(e.NewValue is TaskClass))
            {
                IMG_LSTVIEW_UI.ILV_Clearitem();
                EmptyCanvas();
            }
            
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkspace();
        }

        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            LoadWorkspace();
        }

        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in MAINTREE_UI.ItemsSource)
            {
                object container = MAINTREE_UI.ItemContainerGenerator.ContainerFromItem(o);
                if (container is TreeViewItem item)
                {
                    item.IsExpanded = false;
                    ExpandMultiItems(item, false);
                }
            }
        }

        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in MAINTREE_UI.ItemsSource)
            {
                object container = MAINTREE_UI.ItemContainerGenerator.ContainerFromItem(o);
                if (container is TreeViewItem item)
                {
                    item.IsExpanded = true;
                    ExpandMultiItems(item, true);
                }
            }
        }

        private void ExpandMultiItems(TreeViewItem item, bool isExpand)
        {
            if (item.ItemsSource == null) return;
            foreach (object o in item.ItemsSource)
            {
                object container = item.ItemContainerGenerator.ContainerFromItem(o);
                if (container is TreeViewItem child)
                {
                    child.IsExpanded = isExpand;
                    ExpandMultiItems(child, isExpand);
                }
            }
        }

        private void TaskAdd_Click(object sender, RoutedEventArgs e)
        {
            string text = "";

            if (ShowInputDialog(ref text) == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(text);
            }
        }

        private void ToolAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewProject_button_Click(object sender, RoutedEventArgs e)
        {
            if(m_WorkspaceDoc != null)
            {
                if(MessageBox.Show("Will you save the present Solution\nbefore creating new one?", "Workspace Save", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveWorkspace();
                }
            }

            CreateNewSolution();
            EmptyCanvas();
        }

        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            System.Windows.Forms.Form inputBox = new System.Windows.Forms.Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Input Box"
            };

            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
            {
                Size = new System.Drawing.Size(size.Width - 10, 23),
                Location = new System.Drawing.Point(5, 5),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            System.Windows.Forms.Button okButton = new System.Windows.Forms.Button
            {
                DialogResult = System.Windows.Forms.DialogResult.OK,
                Name = "okButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&OK",
                Location = new System.Drawing.Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            System.Windows.Forms.Button cancelButton = new System.Windows.Forms.Button
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel,
                Name = "cancelButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&Cancel",
                Location = new System.Drawing.Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

    }

    public class MenuItem
    {
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string Title { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
}
