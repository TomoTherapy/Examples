using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DeepObjectDector.sub.lib.err;
using DeepObjectDector.sub.doc;

namespace DeepObjectDector.sub.control.ImageListView
{
    /// <summary>
    /// ImageListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageListView : UserControl
    {
        /*
         * *event Deligate
         */
        public static readonly RoutedEvent ImageDropEvent; //버블링
        static public event EventHandler<ResultEventArgs> SendErrCall;

        /*
         * member EventDeligate
         */
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

       /*
        * * 라우팅 이벤트의 public 인터페이스
        */
        public event RoutedEventHandler imageFileDrop
        {
            add { AddHandler(ImageDropEvent, value); }
            remove { RemoveHandler(ImageDropEvent, value); }
        }

        /*
         * member val
         */
        private String[] m_DropFiles = null;
        private int m_oldSelIndex = 0;

        /*
         * *constructor
         */
        static ImageListView()
        {
            //LDH ImageFileDrag Bubble Event
            ImageDropEvent = EventManager.RegisterRoutedEvent("imageFileDrop", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ImageListView));
        }

        public ImageListView()
        {
            InitializeComponent();
        }
        /*
         * property
         */
        public int SelectedIndex
        {
            get 
            {
                return LV.SelectedIndex;
            }
            set
            {
                LV.SelectedIndex = value;
            }
        }
        public int OldSelIndex
        {
            get
            {
                return m_oldSelIndex;
            }
        }
        public String[] p_DropFiles
        {
            get
            {
                return m_DropFiles;
            }
        }
        public object SelectedItems
        {
            get 
            {
                return LV.SelectedItems;
            }
        }
        public Boolean p_AllowDrop
        {
            get 
            {
                return LV.AllowDrop;
            }
            set 
            {
                LV.AllowDrop = (Boolean) value;
            }
        }
        /*
         * member func
         */
        public long ILV_insertitem(ImageItem item)
        {
            long err = 0;

            try
            {
                LV.Items.Add(item);
                return err;
            }
            catch
            {
                return err;
            }
        }
        public long ILV_Clearitem()
        {
            long err = 0;

            try
            {
                m_oldSelIndex = -1;
                LV.Items.Clear();
                return err;
            }
            catch
            {
                return err;
            }
        }
        public ItemCollection ILV_GetImgItems()
        {
            return LV.Items;
        }
        public ERR_RESULT ILV_ReplaceImgItem(ImageItem item, int index)
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                LV.Items.RemoveAt(index);
                LV.Items.Insert(index, item);
                return result;
            }
            catch (_ImageListViewException err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            
        }
        private void LV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
        private ERR_RESULT inner_FilesImageValid(String[] dropfiles)
        {
            ERR_RESULT result = new ERR_RESULT();
            String[] tokenPath = null;
            String fileName = null;
            String[] fileInform = null;
            String fileType = null;
            try
            {
                foreach (String path in dropfiles)
                {
                    tokenPath = path.Split('\\');
                    fileName = tokenPath[tokenPath.Length -1];
                    fileInform = fileName.Split('.');
                    fileType = fileInform[fileInform.Length -1];

                    if (fileType.ToLower().Equals("jpg") || fileType.ToLower().Equals("png") || fileType.ToLower().Equals("bmp"))
                    {
                        //return result;
                        result.errCode = 0;
                    }
                    else
                    {
                        throw new _ImageListViewException(-50);
                    }
                }

                return result;
            }
            catch (_ImageListViewException err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
        }
        /*
         * *callback
         */

        /// <summary>
        /// 이미지 파일을 드롭 했을 경우 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void inner_OnExcuteErrOut(ResultEventArgs e)
        {
            EventHandler<ResultEventArgs> handler = SendErrCall;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void LV_Drop(object sender, DragEventArgs e)
        {
            ERR_RESULT result = new ERR_RESULT();
            m_DropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                if (m_DropFiles == null)
                    return;
                
                //1. 이미지 파일 검사
                result = inner_FilesImageValid(m_DropFiles);
                if (result.errCode != 0)
                    throw new _ImageListViewException(-50);

                //2. 이미지 파일 경로 Main으로 던짐
                RoutedEventArgs argsEvent = new RoutedEventArgs();
                argsEvent.RoutedEvent = ImageListView.ImageDropEvent;
                argsEvent.Source = this;
                RaiseEvent(argsEvent);

            } 
            catch (_ImageListViewException err)
            { 
                result = ErrProcess.SetErrResult(err);
                inner_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                inner_OnExcuteErrOut(new ResultEventArgs(result));
            }

        }

        private void LV_LostFocus(object sender, RoutedEventArgs e)
        {
            m_oldSelIndex = LV.SelectedIndex;
        }
    }
}
