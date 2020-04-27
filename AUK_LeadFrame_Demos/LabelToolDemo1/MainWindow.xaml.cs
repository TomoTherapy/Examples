using DeepObjectDector.sub.control.ImageListView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Orientation = System.Windows.Controls.Orientation;

namespace LabelToolDemo1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Sets> Set;
        PropertiesClass prop;

        public MainWindow()
        {
            InitializeComponent();

            IMG_LSTVIEW_UI.IsEnabled = false;
            //Set = prop.Set;

            //ImageItemsUpdateUI(SetImageListViewUpdate());
            //NewWorkspace();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IMG_LSTVIEW_UI.imageFileDrop += OnImageListViewDropEvent;
            IMG_LSTVIEW_UI.SelectionChanged += OnImageListViewSel;
        }

        #region Button Event
        private void Check_button_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            foreach (StackPanel stack in SetItems_stackPanel.Children)
            {
                var txt = stack.Children[0] as TextBlock;
                result += txt.Text + "  ";
            }

            MessageBox.Show(result);
        }

        private void AddSet_button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewItem();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.NewItemName.Trim() == "") return;

                //Set 콜렉션에 이미 있는 아이템인가 체크(아이템 이름 & 단축키)
                bool dupl = false;
                foreach (Sets set in Set)
                {
                    if (dialog.NewItemName == set.Name) dupl = true;
                    if (dialog.Shortcut == set.Shortcut) dupl = true;
                }
                //중복이 아니라면 추가
                if (dupl == false) Set.Add(new Sets() { Name = dialog.NewItemName, Shortcut = dialog.Shortcut });

                /*
                //SetItems 패널에 이미 있는가 중복체크
                foreach (var item in SetItems_stackPanel.Children)
                {
                    var a = item as StackPanel;
                    var b = a.Children[0] as TextBlock;

                    if (dialog.NewItemName == b.Text) return;
                }
                //패널에 존재하지 않는다면 추가
                SetItems_stackPanel.Children.Add(SetItem(dialog.NewItemName));
                */

                //KnownSet 업데이트
                UpdateKnownSets();
            }
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if (IMG_LSTVIEW_UI.SelectedItems == null) return;

            System.Collections.IList list = IMG_LSTVIEW_UI.SelectedItems as System.Collections.IList;
            if (list.Count != 1) return;

            ImageItem item = (ImageItem)list[0];
            string imageFile = item.p_Image_uri;

            string sets = "";
            foreach (StackPanel stack in SetItems_stackPanel.Children)
            {
                var txt = stack.Children[0] as TextBlock;
                sets += "_" + txt.Text;
            }

            sets += "_OK";
            string dir = Path.GetDirectoryName(imageFile) + "\\OK\\";
            Directory.CreateDirectory(dir);
            string newFileName = dir + Path.GetFileNameWithoutExtension(imageFile) + sets + Path.GetExtension(imageFile);
            File.Copy(imageFile, newFileName, true);

            int nextIndex = IMG_LSTVIEW_UI.SelectedIndex + 1;

            if(IMG_LSTVIEW_UI.ILV_GetImgItems().Count == nextIndex)
            {
                MessageBox.Show("마지막 이미지입니다!");
                return;
            }

            IMG_LSTVIEW_UI.SelectedIndex = nextIndex;

            if (KeepSets_checkBox.IsChecked == false)
            {
                DeleteAllItem();
            }
        }

        private void NG_button_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList list = IMG_LSTVIEW_UI.SelectedItems as System.Collections.IList;
            if (list.Count != 1) return;

            ImageItem item = (ImageItem)list[0];
            string imageFile = item.p_Image_uri;

            string sets = "";
            foreach (StackPanel stack in SetItems_stackPanel.Children)
            {
                var txt = stack.Children[0] as TextBlock;
                sets += "_" + txt.Text;
            }

            sets += "_NG";
            string dir = Path.GetDirectoryName(imageFile) + "\\NG\\";
            Directory.CreateDirectory(dir);
            string newFileName = dir + Path.GetFileNameWithoutExtension(imageFile) + sets + Path.GetExtension(imageFile);
            File.Copy(imageFile, newFileName, true);

            int nextIndex = IMG_LSTVIEW_UI.SelectedIndex + 1;


            if (IMG_LSTVIEW_UI.ILV_GetImgItems().Count == nextIndex)
            {
                MessageBox.Show("마지막 이미지입니다!");
                return;
            }

            IMG_LSTVIEW_UI.SelectedIndex = nextIndex;

            if (KeepSets_checkBox.IsChecked == false)
            {
                DeleteAllItem();
            }
        }

        private void New_menuItem_Click(object sender, RoutedEventArgs e)
        {
            NewWorkspace();
        }

        private void Save_menuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkspace();
        }

        private void Load_menuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadWorkspace();
        }
        #endregion

        #region ListView Event
        private void OnImageListViewDropEvent(object sender, RoutedEventArgs e)
        {
            ImageListView imgListView = (ImageListView)e.Source;
            List<string> items = new List<string>();
            try
            {
                IMG_LSTVIEW_UI.Focusable = true;
                IMG_LSTVIEW_UI.Focus();

                CopyImagePathToTaskClass(imgListView.p_DropFiles);

                //List Update
                items = SetImageListViewUpdate();

                ImageItemsUpdateUI(items);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CopyImagePathToTaskClass(string[] dropFiles)
        {
            string[] tempPath = new string[dropFiles.Length];

            int i = 0;
            foreach (string dropfile in dropFiles)
            {
                //string appPath = AppDomain.CurrentDomain.BaseDirectory;
                //Directory.CreateDirectory(appPath + "images\\");
                string newPath = prop.WorkspacePath + "\\" + Path.GetFileName(dropfile);

                tempPath[i] = newPath;

                File.Copy(dropfile, newPath);

                i++;
            }
        }

        private List<string> SetImageListViewUpdate()
        {
            if (prop is null) return null;

            List<string> files = new List<string>();

            //string[] paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "images\\");
            string[] paths = Directory.GetFiles(prop.WorkspacePath);

            foreach (string path in paths)
            {
                files.Add(path);
            }

            return files;
        }

        private void ImageItemsUpdateUI(List<string> files)
        {
            if (IMG_LSTVIEW_UI.ILV_GetImgItems().Count != 0) IMG_LSTVIEW_UI.ILV_Clearitem();

            List<ImageItem> list = new List<ImageItem>();
            foreach (string file in files)
            {
                list.Add(new ImageItem() { p_Image_uri = file, p_FileName = Path.GetFileName(file) });
            }

            foreach (ImageItem file in list)
            {
                IMG_LSTVIEW_UI.ILV_insertitem(file);
            }
        }

        private void OnImageListViewSel(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList list = e.AddedItems;
            if (list.Count == 0) return;

            ImageItem item = (ImageItem)list[0];

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
        #endregion

        #region ZoomPanControl Event
        private void ZoomPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            Point curContentMousePoint = e.GetPosition(cv);
            if (e.Delta > 0) CvZoomIn(curContentMousePoint);
            else if (e.Delta < 0) CvZoomOut(curContentMousePoint);
        }

        private void CvZoomIn(Point point)
        {
            ZoomPanControl.ZoomAboutPoint(ZoomPanControl.ContentScale + 0.1, point);
        }

        private void CvZoomOut(Point point)
        {
            ZoomPanControl.ZoomAboutPoint(ZoomPanControl.ContentScale - 0.1, point);
        }
        #endregion

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                OK_button_Click(sender, e);
            }
            else if (e.Key == Key.Right)
            {
                NG_button_Click(sender, e);
            }

            foreach (Sets set in Set)
            {
                if (set.Shortcut == e.Key.ToString())
                {
                    foreach (var item in SetItems_stackPanel.Children)
                    {
                        var a = item as StackPanel;
                        var b = a.Children[0] as TextBlock;

                        if (set.Name == b.Text) return;
                    }

                    SetItems_stackPanel.Children.Add(SetItem(set.Name));
                }
            }
        }

        #region Set Event
        public void UpdateKnownSets()
        {
            KnownSets_stackPanel.Children.Clear();

            foreach (Sets item in Set)
            {
                KnownSets_stackPanel.Children.Add(SetKnownItem(item.Name, item.Shortcut));
            }
        }

        private void DeleteKnownItem(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                var btn = e.OriginalSource as Button;
                var stack = btn.Parent as StackPanel;
                var stackMom = stack.Parent as StackPanel;

                stackMom.Children.Remove(stack);

                int index = -1;
                for (int i = 0; i < Set.Count; i++)
                {
                    if (Set[i].Name == btn.Content.ToString())
                    {
                        index = i;
                    }
                }

                if (index != -1)
                {
                    Set.RemoveAt(index);
                }
            }
        }

        public void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                var button = e.OriginalSource as Button;
                var stack = button.Parent as StackPanel;
                var stackMom = stack.Parent as StackPanel;

                stackMom.Children.Remove(stack);
            }
        }

        public void DeleteAllItem()
        {
            SetItems_stackPanel.Children.RemoveRange(0, SetItems_stackPanel.Children.Count);
        }

        public StackPanel SetItem(string setName)
        {
            TextBlock txtBlock = new TextBlock()
            {
                Height = 30,
                Text = setName,
                Padding = new Thickness(2, 2, 0, 0)
                ,
                FontFamily = new FontFamily("Arial"),
                FontSize = 14,
                Background = Brushes.Transparent
                ,
                Margin = new Thickness(5)
            };
            Button btn = new Button()
            {
                Width = 30,
                Height = 30,
                Content = "x",
                Padding = new Thickness(0, -2, 0, 0)
                ,
                FontFamily = new FontFamily("Arial"),
                FontSize = 16,
                Background = Brushes.Transparent
                ,
                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.DarkRed
            };

            btn.Click += DeleteItem;
            StackPanel item = new StackPanel()
            {
                Height = 30,
                Orientation = Orientation.Horizontal
                ,
                Background = Brushes.LightBlue,
                Margin = new Thickness(5)
            };

            item.Children.Add(txtBlock);
            item.Children.Add(btn);

            return item;
        }

        public StackPanel SetKnownItem(string setName, string shortcut)
        {
            Button btn = new Button()
            {
                Height = 30,
                Content = setName,
                Padding = new Thickness(3, 0, 3, 0),
                FontFamily = new FontFamily("Arial"),
                FontSize = 14,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                ToolTip = "Delete Knonwn Set"
            };

            TextBlock block = new TextBlock()
            {
                Height = 30,
                Text = shortcut.ToUpper(),
                Padding = new Thickness(8, 7, 8, 0),
                FontFamily = new FontFamily("Arial"),
                FontSize = 14,
                Background = Brushes.Transparent,
                Foreground = Brushes.IndianRed
            };

            btn.Click += DeleteKnownItem;
            StackPanel item = new StackPanel()
            {
                Height = 30,
                Orientation = Orientation.Horizontal,
                Background = Brushes.LightSalmon,
                Margin = new Thickness(5)
            };

            item.Children.Add(btn);
            item.Children.Add(block);

            return item;
        }
        #endregion

        #region Workspace Event
        private void NewWorkspace()
        {
            if (prop != null)
            {
                if (MessageBox.Show("Save before new?", "New Project", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SaveWorkspace();
                }
            }

            prop = new PropertiesClass();

            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Set = prop.Set;
                    prop.WorkspacePath = dialog.SelectedPath;
                    ImageItemsUpdateUI(SetImageListViewUpdate());
                    UpdateKnownSets();
                    IMG_LSTVIEW_UI.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void SaveWorkspace()
        {
            if (prop is null)
            {
                MessageBox.Show("Create Project first");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = ".xml",
                Filter = "xml files (*.xml)|*.xml",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(PropertiesClass));
                    TextWriter writer = new StreamWriter(dialog.FileName);
                    xml.Serialize(writer, prop);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LoadWorkspace()
        {
            if(prop != null)
            {
                if (MessageBox.Show("Save before load?", "Load Project", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SaveWorkspace();
                }
            }

            OpenFileDialog dialog = new OpenFileDialog()
            {
                DefaultExt = ".xml",
                Filter = "xml files (*.xml)|*.xml",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(PropertiesClass));
                    TextReader reader = new StreamReader(dialog.FileName);
                    prop = xml.Deserialize(reader) as PropertiesClass;

                    Set = prop.Set;
                    ImageItemsUpdateUI(SetImageListViewUpdate());
                    UpdateKnownSets();
                    IMG_LSTVIEW_UI.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

    }

}