using Cognex.VisionPro;
using Cognex.VisionPro.PMRedLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageStitchAndAlign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ImageSource _finalSource;
        private List<Bitmap> bmpList;
        private Bitmap MergedBitmap;
        CogPMRedLineTool red;

        public ImageSource FinalSource { get => _finalSource; set { _finalSource = value; RaisePropertyChanged(nameof(FinalSource)); } }
        public int CamNum { get => _camNum; set { _camNum = value; RaisePropertyChanged(nameof(CamNum)); } }
        public int WatchTime { get => _watchTime; set { _watchTime = value; RaisePropertyChanged(nameof(WatchTime)); } }
        public Crevis.Devices.CrevisCamera camera;
        private int _camNum;
        private int _watchTime;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            red = CogSerializer.LoadObjectFromFile(@"C:\Users\crevis\Desktop\redline.vpp") as CogPMRedLineTool;

            camera = new Crevis.Devices.CrevisCamera();
            camera.Open();
            camera.AcqStart();
            //camera.UpdateDevice();
            CamNum = camera.CameraList.Count();

            bmpList = new List<Bitmap>();
            //Process();

            //this.Close();
        }

        private void Merge()
        {
            List<byte[]> arrList = new List<byte[]>();
            const int IMG_CNT = 6;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < IMG_CNT; i++)
            {
                //bmpList.Add(new Bitmap(@"D:\0.bmp"));

                //Bmp to Byte[]
                BitmapData bmpdata =
                    bmpList[i].LockBits(new System.Drawing.Rectangle(0, 0, bmpList[i].Width, bmpList[i].Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                // 2x2 사이즈 bmp
                // 0 , 125,
                // 125, 0
                //일때 첫번째 0 데이터의 시작 주소 Pointer 값
                //scan0 = Pixel Data 시작점을 알려주는 포인터

                //Stride = Byte 단위 기준 Width에 가장 근접한 "상위" 4의 배수 : 왜? 이게 알고리즘적으로 빠르다고함 누군가그랫음
                //gray8bpp = 1Byte 
                //Width = 101 때, 이미지 가로의 Byte = 101 / 104
                //rgb 24bpp = 3Byte
                ////Width = 101 때, 이미지 가로의 Byte = 303 / strie = 304 -> int형으로 이미지의 실질적인 계산 Width를 나타냄
                //strid는 GDiPlus가 바라보는 알고리즘적 Width의 길이.

                //Row -> Colum
                //3 x 2 Bitmap
                // 0, 123, 123,
                // 123, 123, 0

                byte[] rawdata = new byte[bmpdata.Stride * bmpdata.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, rawdata, 0, rawdata.Length);
                arrList.Add(rawdata);
                bmpList[i].UnlockBits(bmpdata);
            }

            // Byte Arr Merge
            // RawData 6개를 1개의 Arrlist로 병합
            byte[] mergeArrList;
            mergeArrList = new byte[arrList[0].Length * IMG_CNT];
            for (int i = 0; i < IMG_CNT; i++)
            {
                Array.Copy(arrList[i], 0, mergeArrList, arrList[i].Length * i, arrList[i].Length);
            }

            // Byte Arr to Bitmap : 세로 병합버전
            //int mergeW = bmpList[0].Width * IMG_CNT;
            int mergeW = bmpList[0].Width;
            int mergeH = bmpList[0].Height * IMG_CNT;
            int bpp = System.Windows.Media.PixelFormats.Gray8.BitsPerPixel;

            //8bpp Gray기준.
            //byte기준으로 가장 근접한 4의 배수 

            //101 * 8 = 808bit //현재 Pixel Per Pit로 변환한 bit수
            //808 + 31 = 839  // 가장 근접한 4byte 배수를 구하기 위한 덧셈 (3.999..byte)
            //+32 100 -> 104 100->100 . 101~103 -> 104
            //839 / 8 / 4 
            // * 4 = 104
            //
            int stride = 4 * ((mergeW * bpp + 31) / 32);
            //IntPtr는 데이터가 담긴 Scan0의 포인터 이나, 어차피 데이터를 RawData로 입힐거기 때문에 new 로 집어넣어도 상관없음
            Bitmap mergeBitmap = new Bitmap(mergeW, mergeH, stride, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, new IntPtr());
            {
                //팔레트 만들기 //모노에 이거 없으면 칼라풀한 모노 이미지가 튀어나옴 ㅇ
                //ColorPalette pal = mergeBitmap.Palette;
                ColorPalette pal = mergeBitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    pal.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                }

                mergeBitmap.Palette = pal;


                System.Drawing.Imaging.BitmapData bmpdata =
                    mergeBitmap.LockBits(new System.Drawing.Rectangle(0, 0, mergeW, mergeH), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                //병합한 RawData 사이즈 만큼 Copy
                System.Runtime.InteropServices.Marshal.Copy(mergeArrList, 0, bmpdata.Scan0, stride * mergeH);

                mergeBitmap.UnlockBits(bmpdata);

                // 문제 AForge의 Gray알고리즘을 못씀! 왜냐면..
                // Aforge는 24bpp RGB 이미지를 Grayscale로 바꾸는 알고리즘임
                // 이건 시작부터 8bpp라서 못쓰니까..
                // 팔레트 i,i,i로 바꿔야 함
                // 참고로 Graphic는 Index 처리가 안된 raw 24bpp 이미지로만 사용할 수 있어서 aforge를 사용할 수 있으나,
                // 그걸 감안하더라도 이게더 빠르더라~
                //mergeBitmap.SetGrayScale();
            }

            mergeBitmap.Save(@"C:\Users\crevis\Desktop\DDDDDD.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Save

            MergedBitmap = mergeBitmap;
        }

        private void Process()
        {

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            red.InputImage = new CogImage8Grey(MergedBitmap);
            
            red.Run();

            if (red.Results.Count == 0) MessageBox.Show("Jesus!!");
            //watch.Stop();
            //WatchTime = (int)watch.ElapsedMilliseconds;

        }



        private void ToArrMS()
        {
            //Bmp의 메타 데이터 까지 전부 Ar 담겨서,, xx
        }

        private void ToArrBData()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            camera.GrabLineTrg(0);
            camera.GrabLineTrg(1);
            camera.GrabLineTrg(2);
            camera.GrabLineTrg(3);
            camera.GrabLineTrg(4);
            camera.GrabLineTrg(5);


            foreach (var cam in camera.CameraList)
                bmpList.Add(cam.BitmapImage);

            Merge();
            Process();

            watch.Stop();
            WatchTime = (int)watch.ElapsedMilliseconds;
        }
    }
}
