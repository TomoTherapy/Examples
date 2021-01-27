using System;
using System.Drawing;
using System.Windows.Forms;
using Crevis.VirtualFG40Library;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Grab_SoftwareTrigger_Color
{
    public partial class Grab_SoftwareTrigger_Color : Form
    {
        VirtualFG40Library _virtualFG40 = new VirtualFG40Library();
        Int32 _hDevice = 0;
        Int32 _width = 0;
        Int32 _height = 0;
        Int32 _bufferSize = 0;
        Boolean _isOpen = false;
        IntPtr _pImage = new IntPtr();
        IntPtr _cvtImage = new IntPtr();

        public Grab_SoftwareTrigger_Color()
        {
            InitializeComponent();
        }

        private void Grab_SoftwareTrigger_Color8_Load(object sender, EventArgs e)
        {
            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                //System Initialize
                status = _virtualFG40.InitSystem();
                if(status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("System Initialize failed : {0}", status));
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            timer_Grab.Interval = 30;

            btn_Open.Enabled = true;
            btn_Close.Enabled = false;
            btn_Grab.Enabled = false;
            btn_Play.Enabled = false;
            btn_Stop.Enabled = false;
            btn_WBOnce.Enabled = false;
            timer_Grab.Enabled = false;
        }

        private void Grab_SoftwareTrigger_Color8_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_isOpen == true)
            {
                if (timer_Grab.Enabled == true)
                {
                    timer_Grab.Enabled = false;
                }
                if (_pImage != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_pImage);
                    _pImage = IntPtr.Zero;
                }
                if (_cvtImage != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_cvtImage);
                    _cvtImage = IntPtr.Zero;
                }
                // Close Device
                _virtualFG40.CloseDevice(_hDevice);
            }

            _virtualFG40.FreeSystem();
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;
            UInt32 camNum = 0;

            try
            {
                // Update Device List
                status = _virtualFG40.UpdateDevice();
                if(status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    _virtualFG40.FreeSystem();
                    throw new Exception(String.Format("Update Device list failed : {0}", status));
                }

                status = _virtualFG40.GetAvailableCameraNum(ref camNum);
                if(camNum <= 0)
                {            
                    _virtualFG40.FreeSystem();
                    throw new Exception("The camera can not be connected.");
                }
                // camera open
                status = _virtualFG40.OpenDevice(0, ref _hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    _virtualFG40.FreeSystem();
                    throw new Exception(String.Format("Open device failed : {0}", status));
                }

                _isOpen = true;

                // Call Set Feature
                SetFeature();

                // Get Width
                status = _virtualFG40.GetIntReg(_hDevice, VirtualFG40Library.MCAM_WIDTH, ref _width);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Read Register failed : {0}", status));
                }

                // Get Height
                status = _virtualFG40.GetIntReg(_hDevice, VirtualFG40Library.MCAM_HEIGHT, ref _height);

                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Read Register failed : {0}", status));
                }

                // Image buffer allocation
                _bufferSize = _width * _height;
                _pImage = Marshal.AllocHGlobal(_bufferSize);

                // Display Image buffer allocation
                _cvtImage = Marshal.AllocHGlobal(_bufferSize * 3);

                pictureBox_Display.Image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btn_Open.Enabled = false;
            btn_Close.Enabled = true;
            btn_Grab.Enabled = true;
            btn_Play.Enabled = true;
            btn_Stop.Enabled = false;
            btn_WBOnce.Enabled = true;
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            if (_isOpen == true)
            {
                if (timer_Grab.Enabled == true)
                {
                    timer_Grab.Enabled = false;
                }
                if (_pImage != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_pImage);
                    _pImage = IntPtr.Zero;
                }
                if (_cvtImage != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_cvtImage);
                    _cvtImage = IntPtr.Zero;
                }

                // Close Device
                _virtualFG40.CloseDevice(_hDevice);

                _isOpen = false;
            }

            btn_Open.Enabled = true;
            btn_Close.Enabled = false;
            btn_Grab.Enabled = false;
            btn_Play.Enabled = false;
            btn_Stop.Enabled = false;
            btn_WBOnce.Enabled = false;
        }

        private void btn_Grab_Click(object sender, EventArgs e)
        {            
            // 1. Excute Acquisition Start command
            // 2. Excute GrabStartAsync command
            // 3. Excute SoftwareTrigger command
            // 4. Grab Image using GrabImageAsync
            // 5. Image Display
            // 6. Excute Acquisition Stop command

            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                // Acqusition Start
                status = _virtualFG40.AcqStart(_hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Acqusition Start failed : {0}", status));
                }

                // GrabStartAsync
                _virtualFG40.GrabStartAsync(_hDevice, 0xFFFFFFFF);

                // Software Trigger
                status = _virtualFG40.SetCmdReg(_hDevice, VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Software trigger command failed : {0}", status));
                }

                // Grab Function
                status = _virtualFG40.GrabImageAsync(_hDevice, _pImage, (UInt32)_bufferSize, 0xFFFFFFFF);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("GrabImage failed : {0}", status));
                }
                
                _virtualFG40.CvtColor(_pImage, _cvtImage, _width, _height, VirtualFG40Library.CV_BayerRG2RGB);

                // Grab Function
                DispImage();

                // Acqusition Stop
                status = _virtualFG40.AcqStop(_hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Acqusition Stop failed : {0}", status));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btn_Open.Enabled = false;
            btn_Close.Enabled = true;
            btn_Grab.Enabled = true;
            btn_Play.Enabled = true;
            btn_Stop.Enabled = true;
        }

        private void btn_Play_Click(object sender, EventArgs e)
        {
            // 1. Change Acquisition Mode : Continuous
            // 2. Excute Acqusition Start Command
            // 3. Excute GrabStartAsync command
            // 4. Acqusution Loop Function (Timer)
            // -> Excute SoftwareTrigger command
            //	-> Grab Image using GrabImageAsync
            //	-> Image Display

            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                // Change Acqusition Mode
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_ACQUISITION_MODE, VirtualFG40Library.ACQUISITION_MODE_CONTINUOUS);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }

                // Acqusition Start
                status = _virtualFG40.AcqStart(_hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Acqusition Start failed : {0}", status));
                }

                // GrabStartAsync
                _virtualFG40.GrabStartAsync(_hDevice, 0xFFFFFFFF);

                // Acqusution Loop Function
                timer_Grab.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btn_Open.Enabled = false;
            btn_Close.Enabled = false;
            btn_Grab.Enabled = false;
            btn_Play.Enabled = false;
            btn_Stop.Enabled = true;            
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            // 1. Thread suspend or Timer Stop
            // 2. Excute Acqusition Stop Command 

            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                timer_Grab.Enabled = false;

                // Acqusition Stop
                status = _virtualFG40.AcqStop(_hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Acqusition Start failed : {0}", status));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btn_Open.Enabled = false;
            btn_Close.Enabled = true;
            btn_Grab.Enabled = true;
            btn_Play.Enabled = true;
            btn_Stop.Enabled = false;            
        }

        private void btn_WBOnce_Click(object sender, EventArgs e)
        {
            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                // BalanceWhiteAuto
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_BALANCE_WHITE_AUTO, VirtualFG40Library.BALANCE_WHITE_AUTO_ONCE);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        

        private void timer_Grab_Tick(object sender, EventArgs e)
        {
            // Grab Function
            _virtualFG40.SetCmdReg(_hDevice, VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
            _virtualFG40.GrabImageAsync(_hDevice, _pImage, (UInt32)_bufferSize, 0xFFFFFFFF);
            _virtualFG40.CvtColor(_pImage, _cvtImage, _width, _height, VirtualFG40Library.CV_BayerRG2RGB);

            // Display
            DispImage();
        }

        private void SetFeature()
        {
            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                // Set Trigger Mode
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_TRIGGER_MODE, VirtualFG40Library.TRIGGER_MODE_ON);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }

                //TriggerSource => Software
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_TRIGGER_SOURCE, VirtualFG40Library.TRIGGER_SOURCE_SOFTWARE);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }

                //TriggerDelay => 1us
                status = _virtualFG40.SetFloatReg(_hDevice, VirtualFG40Library.MCAM_TRIGGER_DELAY, 1.0f);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }

                // Set PixelFormat
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_PIXEL_FORMAT, VirtualFG40Library.PIXEL_FORMAT_BAYERRG8);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DispImage()
        {
            Int32 bitsPerPixel = 0;
            Int32 stride = 0;
            Bitmap bitmap;
            PixelFormat pixelFormat = PixelFormat.Format24bppRgb;        
       
            //color
            bitsPerPixel = 24;
            stride = (Int32)((_width * bitsPerPixel + 7) / 8);
            bitmap = new Bitmap(_width, _height, stride, pixelFormat, _cvtImage);

            pictureBox_Display.Image = bitmap;                       
        }

       

                     
    }
}
