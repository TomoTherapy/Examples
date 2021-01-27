using System;
using System.Drawing;
using System.Windows.Forms;
using Crevis.VirtualFG40Library;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Callback_Normal_Mono
{
    public partial class Callback_Normal_Mono : Form
    {
        VirtualFG40Library _virtualFG40 = new VirtualFG40Library();
        Int32 _hDevice = 0;
        Int32 _width = 0;
        Int32 _height = 0;
        Boolean _isOpen = false;
        Boolean _isGrabed = false;
        IntPtr _userdata = new IntPtr();
        GCHandle _gch;

        public Callback_Normal_Mono()
        {
            InitializeComponent();
        }

        private void Callback_Normal_Mono_Load(object sender, EventArgs e)
        {
            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                //System Initialize
                status = _virtualFG40.InitSystem();
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("System Initialize failed : {0}", status));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btn_Open.Enabled = true;
            btn_Close.Enabled = false;
            btn_Grab.Enabled = false;
            btn_Play.Enabled = false;
            btn_Stop.Enabled = false;
        }

        private void Callback_Normal_Mono_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_isOpen == true)
            {
                _gch.Free();

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
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    _virtualFG40.FreeSystem();
                    throw new Exception(String.Format("Update Device list failed : {0}", status));
                }

                status = _virtualFG40.GetAvailableCameraNum(ref camNum);
                if (camNum <= 0)
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

                // Crevis Callback Function  
                VirtualFG40Library.CallbackFunc vfgCallback = new VirtualFG40Library.CallbackFunc(OnCallback);
                _virtualFG40.SetCallbackFunction(_hDevice, VirtualFG40Library.EVENT_NEW_IMAGE, vfgCallback, _userdata);
                _gch = GCHandle.Alloc(vfgCallback);

                pictureBox_Display.Image = new Bitmap(_width, _height, PixelFormat.Format8bppIndexed);
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

        private void btn_Close_Click(object sender, EventArgs e)
        {
            if (_isOpen == true)
            {
                _gch.Free();

                // Close Device
                _virtualFG40.CloseDevice(_hDevice);

                _isOpen = false;
            }

            btn_Open.Enabled = true;
            btn_Close.Enabled = false;
            btn_Grab.Enabled = false;
            btn_Play.Enabled = false;
            btn_Stop.Enabled = false;
        }

        private void btn_Grab_Click(object sender, EventArgs e)
        {
            // 1. Excute Acquisition Start command	
            // 2. Waiting Grab Finish
            // 3. Excute Acquisition Stop command          

            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                // Acqusition Start
                status = _virtualFG40.AcqStart(_hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Acqusition Start failed : {0}", status));
                }

                while (true)
                {
                    if (_isGrabed == true)
                        break;
                }

                // Acqusition Stop
                status = _virtualFG40.AcqStop(_hDevice);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Acqusition Stop failed : {0}", status));
                }

                _isGrabed = false;
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
            // 1. Excute Acqusition Stop Command 

            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
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
            btn_Stop.Enabled = true;
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

        private void SetFeature()
        {
            Int32 status = VirtualFG40Library.MCAM_ERR_SUCCESS;

            try
            {
                // Set Trigger Mode
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_TRIGGER_MODE, VirtualFG40Library.TRIGGER_MODE_OFF);
                if (status != VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new Exception(String.Format("Write Register failed : {0}", status));
                }

                // Set PixelFormat
                status = _virtualFG40.SetEnumReg(_hDevice, VirtualFG40Library.MCAM_PIXEL_FORMAT, VirtualFG40Library.PIXEL_FORMAT_MONO8);
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

        public Int32 OnCallback(Int32 EventID, IntPtr pImage, IntPtr userData)
        {
            if (EventID != VirtualFG40Library.EVENT_NEW_IMAGE)
                return -1;            

            Int32 bitsPerPixel = 0;
            Int32 stride = 0;
            Bitmap bitmap;
            PixelFormat pixelFormat = PixelFormat.Format8bppIndexed;

            //color
            bitsPerPixel = 8;
            stride = (Int32)((_width * bitsPerPixel + 7) / 8);
            bitmap = new Bitmap(_width, _height, stride, pixelFormat, pImage);

            SetGrayscalePalette(bitmap);
            pictureBox_Display.Image = bitmap;

            _isGrabed = true;
 
            return 0;            
        }

        private void SetGrayscalePalette(Bitmap bitmap)
        {
            ColorPalette GrayscalePalette = bitmap.Palette;

            for (int i = 0; i < 255; i++)
            {
                GrayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
            }

            bitmap.Palette = GrayscalePalette;
        }
    }
}
