using System;
using Crevis.VirtualFG40Library;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media.Imaging;
using Cognex.VisionPro;
using System.Windows.Media;
using System.Threading;

namespace ElaborateTester
{
    class CameraControl
    {
        //Camera Library
        VirtualFG40Library virtualFG40 = new VirtualFG40Library();

        //Camera Variables
        int hDevice = 0;
        int width = 0;
        int height = 0;
        int bufferSize = 0;
        int status = -1;
        IntPtr pImage = new IntPtr();
        bool pFlag = false;
        uint camNum = 0;

        public BitmapSource bitSource { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Initiate the System
        /// </summary>
        public void InitSystem()
        {
            virtualFG40.InitSystem();
        }

        /// <summary>
        /// Check whether Device is opened or not
        /// </summary>
        /// <returns></returns>
        public bool IsOpenDevice()
        {
            if (hDevice == -1) hDevice = 0;

            virtualFG40.IsOpenDevice(hDevice, ref pFlag);
            return pFlag;
        }

        /// <summary>
        /// Open the Device, Start Acquisition
        /// </summary>
        public void OpenDeviceAquisitionStart()
        {
            // 일정시간동안 (5초)  카메라 찾다가 카메라가 검색이되면 빠저나요게
            int count = 0;
            while (true)
            {
                count++;
                status = virtualFG40.UpdateDevice();//새로고침
                status = virtualFG40.GetAvailableCameraNum(ref camNum);
                if (camNum > 0)
                    break;
                Thread.Sleep(500);
                if (count > 10)
                    break;
            }

            if (camNum < 1)
            {
                Message = "No Camera Device";
                return;
            }

            status = virtualFG40.OpenDevice(0, ref hDevice);

            // Get Width
            status = virtualFG40.GetIntReg(hDevice, VirtualFG40Library.MCAM_WIDTH, ref width);
            if (status != VirtualFG40Library.MCAM_ERR_SUCCESS) return;

            // Get Height
            status = virtualFG40.GetIntReg(hDevice, VirtualFG40Library.MCAM_HEIGHT, ref height);
            if (status != VirtualFG40Library.MCAM_ERR_SUCCESS) return;

            // Image buffer allocation
            bufferSize = width * height;
            pImage = Marshal.AllocHGlobal(bufferSize);

            status = virtualFG40.AcqStart(hDevice);
            if (status == 0)
                Message = "Connection Success";
            else
                Message = "Connection Failed";
        }

        /// <summary>
        /// Grab, not async, for Grab Once
        /// </summary>
        /// <returns></returns>
        public CogImage8Grey CamGrab()
        //public CogImage24PlanarColor CamGrab()
        {
            status = virtualFG40.IsOpenDevice(hDevice, ref pFlag);
            if (status == 0)
            {
                status = virtualFG40.SetCmdReg(hDevice, VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                if (status != 0) throw new Exception();
                /*
                 * MVIPConfig에서 트리거 모드를 ON 해놨을때 소프트웨어 트리거 날려주는 거임. ON해놓고 트리거를 안날리면 당연히 이미지가 들어오지않으며
                 * OFF 일 경우에는 날려도 아무 소용없다.
                 */

                //GrabAsync 로하면 왠지 한번만 그랩할때 이전 이미지가 한장 날아온다. 그래서 두번해야함.
                //아마도 어씽크다 보니 카메라가 찍어서 메모리에 넣기도 전에 이미 메모리에 접근해서 가져오기 때문이 아닐까라고 추측.
                status = virtualFG40.GrabImage(hDevice, pImage, (UInt32)bufferSize);
                if (status != 0) return null;

                Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
                virtualFG40.CvtColor(pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.CV_BayerGR2RGB);
                var bitmapSource = BitmapSource.Create(width, height, bitmap.HorizontalResolution, bitmap.VerticalResolution, PixelFormats.Bgr24, null, bitmapData.Scan0, bitmapData.Stride * height, bitmapData.Stride);
                bitmap.UnlockBits(bitmapData);

                bitSource = bitmapSource;

                //코그 디스플레이는 그냥 비트맵 이미지를 사용한다. 아래와같이 변환해서 사용가능.
                //CogImage24PlanarColor cogImage = new CogImage24PlanarColor(bitmap);
                CogImage8Grey cogImage = new CogImage8Grey(bitmap);

                Message = "Grab Success";
                return cogImage;
            }
            else
            {
                Message = "Grab Failed";
                return null;
            }
        }

        /// <summary>
        /// GrabAsync. It is for continuous grabbing
        /// </summary>
        /// <returns></returns>
        public CogImage8Grey CamGrabAsync()
        {
            //네트워크가 불안정하여 로스트 패킷이 존나게 발생하는 환경에서는 오히려 독이 될 수도있다.
            //status = virtualFG40.IsOpenDevice(hDevice, ref pFlag); 

            status = virtualFG40.SetCmdReg(hDevice, VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
            if (status != 0) throw new Exception();
            //트리거모드 OFF 이면 아무짝에도 쓸모가 없는 소프트트리거 날리기 코드.

            status = virtualFG40.GrabImageAsync(hDevice, pImage, (UInt32)bufferSize, 0xFFFFFFFF);
            if (status != 0) throw new Exception();
            /* 
             * 트리거모드 OFF 상태에서 Async를 사용하면 메모리에서 바로 뽑아오기 때문에 이전 이미지가 한번 들어온다. 
             * 트리거 모드없이 쓸꺼면 Grab Once 에서 Async 쓰지마요
             */

            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            virtualFG40.CvtColor(pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.CV_BayerGR2RGB);
            var bitmapSource = BitmapSource.Create(width, height, bitmap.HorizontalResolution, bitmap.VerticalResolution, PixelFormats.Bgr24, null, bitmapData.Scan0, bitmapData.Stride * height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);

            CogImage8Grey cogImage = new CogImage8Grey(bitmap);

            return cogImage;
        }

        /// <summary>
        /// Acquisition Stop, Close Device.
        /// </summary>
        public void CloseDeviceAcquisitionStop()
        {
            status = virtualFG40.IsOpenDevice(hDevice, ref pFlag);
            if (status == 0)
            {
                virtualFG40.AcqStop(hDevice);
                virtualFG40.CloseDevice(hDevice);
                Message = "Device Closed";
            }
            else
                Message = "Device Already Closed";
        }


        /// <summary>
        /// Free the System
        /// </summary>
        public void CamFree()
        {
            virtualFG40.FreeSystem();
        }

    }
}
