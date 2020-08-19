using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ViDi_RuntimeResult
{
    public class MainWindow_ViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void PropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public CogRecordDisplay CogRecordDisplay;

        private ViDi2.Runtime.Local.Control m_Control;
        private ViDi2.Runtime.IWorkspace m_Workspace;
        private ViDi2.Runtime.IStream m_Stream;
        private List<string> files;
        private int index;

        //Properties
        public BitmapImage BitmapImageSource { get; set; }

        public MainWindow_ViewModel()
        {
            CogRecordDisplay = new CogRecordDisplay();

            m_Control = new ViDi2.Runtime.Local.Control();

            string wsPath = AppDomain.CurrentDomain.BaseDirectory + "CrevisBox.vrws";
            string wsName = wsPath.Split('\\').Last().Split('.').First();

            m_Workspace = m_Control.Workspaces.Add(wsName, wsPath);
            m_Stream = m_Workspace.Streams.First();

            files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "images").ToList();
            index = 0;
        }

        internal void Loaded()
        {
            CogRecordDisplay.HorizontalScrollBar = false;
            CogRecordDisplay.VerticalScrollBar = false;
        }

        internal void Run()
        {
            Process(files[index]);

            if (index >= files.Count - 1)
                index = 0;
            else
                index++;
        }

        private void Process(string path)
        {
            foreach (var tool in m_Stream.Tools)
            {
                if (tool is ViDi2.IRedTool redTool)
                {
                    //이미지 프로세스
                    using (Bitmap bitmap = new Bitmap(path))
                    {
                        try
                        {
                            //ViDi2.VisionPro.Image image = new ViDi2.VisionPro.Image(new CogImage8Grey(bitmap));
                            ViDi2.IImage image = new ViDi2.FormsImage(bitmap);
                            ViDi2.ISample sample = m_Stream.Process(image);

                            foreach (var marking in sample.Markings)
                            {
                                if (marking.Value is ViDi2.IRedMarking redMarking)
                                {
                                    #region VisionPro CogRecordDisplay에 ViDi검사 결과 변환하여 표시하기
                                    //CogRecord 생성
                                    CogRecord record = new CogRecord();
                                    //record.Content = image.InternalImage;
                                    record.Content = image.Bitmap;

                                    //ViDi 검사결과를 VisionPro Record로 변환
                                    CogGraphicCollection gc = new CogGraphicCollection();
                                    ViDi2.VisionPro.RedViewRecord redViewRecord = new ViDi2.VisionPro.RedViewRecord(redMarking.Views[0] as ViDi2.IRedView, new ViDi2.VisionPro.Records.DefaultRedToolGraphicCreator());
                                    if (redViewRecord.HasGraphics && redViewRecord.GraphicsVisible)
                                    {
                                        foreach (ICogRecord icg in redViewRecord.SubRecords)
                                        {
                                            gc.Add(icg.Content as ICogGraphic);
                                        }
                                    }
                                    record.SubRecords.Add(new CogRecord() { Content = gc, ContentType = typeof(CogGraphicCollection) });

                                    //CogRecordDisplay에 Record 삽입
                                    CogRecordDisplay.Record = record;
                                    CogRecordDisplay.Fit();
                                    #endregion

                                    #region WPF Image control 에 ViDi 검사결과 표시
                                    ViDi2.IImage overlay = redMarking.OverlayImage();

                                    Bitmap result = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                    using (Graphics graphics = Graphics.FromImage(result))
                                    {
                                        graphics.DrawImage(bitmap, 0, 0);
                                        graphics.DrawImage(overlay.Bitmap, 0, 0);
                                    }

                                    using (var memory = new MemoryStream())
                                    {
                                        result.Save(memory, ImageFormat.Bmp);
                                        memory.Position = 0;

                                        var bitmapImage = new BitmapImage();
                                        bitmapImage.BeginInit();
                                        bitmapImage.StreamSource = memory;
                                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                        bitmapImage.EndInit();
                                        bitmapImage.Freeze();

                                        BitmapImageSource = bitmapImage;

                                        PropertyChangedEvent(nameof(BitmapImageSource));
                                    }
                                    #endregion
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

    }
}
