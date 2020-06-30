using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace Crevis.Devices
{
    public class CrevisCamera : IDisposable
    {
        //내부 사용 변수
        public VirtualFG40Library.VirtualFG40Library _vfg;
        Thread ConnectThread;

        //외부 접근 변수
        public List<Camera> CameraList;

        public CrevisCamera()
        {
            _vfg = new VirtualFG40Library.VirtualFG40Library();
            CameraList = new List<Camera>();

            _vfg.InitSystem();
        }

        ~CrevisCamera()
        {
            Dispose();
        }

        //init 및 Update, OpenDevice 전부 한 메서드에서 진행
        public ERR_RESULT Open()
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();

            try
            {
                //카메라 정보 Update 및 Open
                UpdateDevice();

                //카메라 Open Check Thread
                ConnectThread = new Thread(ConnectCheck);
                ConnectThread.Start();
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                Close();
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                Close();
                return m_Err;
            }
        }

        public ERR_RESULT UpdateDevice()
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();

            // Refresh의 경우에만 Cam Close
            foreach (var cam in CameraList)
                if (cam.IsOpen == true)
                {
                    _vfg.CloseDevice(cam.HDevice);
                    cam.IsOpen = false;
                    cam.isAcqStart = false;
                }

            // Cam Info Update
            status = _vfg.UpdateDevice();
            if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
            {
                _vfg.FreeSystem();
                throw new _CamException(-101);
            }

            // GetAvailable 을 해주어야만 Info에 접근가능(Vfg 오류)
            uint camNum = 0;
            status = _vfg.GetAvailableCameraNum(ref camNum);
            if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
            {
                _vfg.FreeSystem();
                throw new _CamException(-103);
            }
            for (int i = 0; i < camNum; i++)
                CameraList.Add(new Camera());




            //사용 가능한 카메라 Open
            for (int i = 0; i < CameraList.Count; i++)
            {
                Byte[] pInfo;
                UInt32 size = 0;

                // Get Device ID
                status = _vfg.GetEnumDeviceID((uint)i, null, ref size);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
                pInfo = new Byte[size - 1];
                _vfg.GetEnumDeviceID((uint)i, pInfo, ref size);
                CameraList[i].DevID = System.Text.Encoding.Default.GetString(pInfo);
                // Open
                status = _vfg.OpenDevice((uint)i, ref CameraList[i].HDevice);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-102);

                // Get Width
                status = _vfg.GetIntReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_WIDTH, ref CameraList[i].Width);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);

                // Get Height
                status = _vfg.GetIntReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_HEIGHT, ref CameraList[i].Height);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
                // Get UserID
                status = _vfg.GetStrReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_DEVICE_USER_ID, null, ref size);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
                if (size <= 0)
                {
                    CameraList[i].UserID = System.Text.Encoding.Default.GetString(pInfo);
                }
                else
                {
                    pInfo = new Byte[size];
                    status = _vfg.GetStrReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_DEVICE_USER_ID, pInfo, ref size);
                    CameraList[i].UserID = System.Text.Encoding.Default.GetString(pInfo);
                }

                //Get PixelFormat
                status = _vfg.GetEnumReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_PIXEL_FORMAT, null, ref size);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
                pInfo = new byte[size];
                status = _vfg.GetEnumReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_PIXEL_FORMAT, pInfo, ref size);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
                CameraList[i].PixelFormat = Encoding.Default.GetString(pInfo);

                ////Set CallBack
                ////S/N만 뽑아서 UserData로 저장
                //VirtualFG40Library.VirtualFG40Library.CallbackFunc func = new VirtualFG40Library.VirtualFG40Library.CallbackFunc(CamCallBack);
                //AvailableCamera[i].UserData = new IntPtr(Int64.Parse(AvailableCamera[i].DevID.Split('_')[AvailableCamera[i].DevID.Split('_').Length - 1]));
                //_vfg.SetCallbackFunction(AvailableCamera[i].HDevice, VirtualFG40Library.VirtualFG40Library.EVENT_NEW_IMAGE, func, AvailableCamera[i].UserData);
                //AvailableCamera[i].Gch = GCHandle.Alloc(func);
                status = _vfg.SetEnumReg(CameraList[i].HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_MODE, VirtualFG40Library.VirtualFG40Library.TRIGGER_MODE_ON);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);

                //Set Buffer Size
                CameraList[i].BufferSize = CameraList[i].Width * CameraList[i].Height;
                //Set PImage Pointer
                CameraList[i].pImage = Marshal.AllocHGlobal(CameraList[i].BufferSize);
                //Set First Image
                Bitmap bitmap = new Bitmap(CameraList[i].Width, CameraList[i].Height, PixelFormat.Format8bppIndexed);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                bitmap.UnlockBits(bitmapData);
                CameraList[i].BitmapImage = bitmap.Clone() as Bitmap;
                //CameraList[i].BitmapImage = bitmap;
                CameraList[i].IsOpen = true;

                //Grab Start Async
                status = _vfg.GrabStartAsync(CameraList[i].HDevice, 0);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
            }

            return m_Err;
        }

        //상시 오픈 체크 Thread
        public void ConnectCheck()
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();

            while (true)
            {
                try
                {
                    foreach (var cam in CameraList)
                    {
                        //for문을 계속 돌면서 카메라 연결 확인
                        _vfg.IsOpenDevice(cam.HDevice, ref cam.IsOpen);
                        if (!cam.IsOpen)
                        {
                            //일정 횟수만큼 다시 연결 시도
                            uint hDevNum = (uint)cam.HDevice;
                            int count = 0;
                            while (count < 4)
                            {
                                try
                                {
                                    _vfg.CloseDevice(cam.HDevice);
                                    status = _vfg.OpenDevice(hDevNum, ref cam.HDevice);
                                    if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                                    {
                                        count++;
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                    _vfg.IsOpenDevice(cam.HDevice, ref cam.IsOpen);
                                    if (cam.IsOpen)
                                    {
                                        //_vfg.SetCallbackFunction(cam.HDevice, VirtualFG40Library.VirtualFG40Library.EVENT_NEW_IMAGE, cam.Gch.Target as VirtualFG40Library.VirtualFG40Library.CallbackFunc, cam.UserData);
                                        if (cam.isAcqStart)
                                            AcqStart();
                                        break;
                                    }
                                }
                                catch
                                {
                                    count++;
                                    Thread.Sleep(3000);
                                    continue;
                                }
                            }
                            if (!cam.IsOpen)
                            {
                                throw new _CamException(-105);
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {

                }
                catch (_CamException err)
                {
                    ERR_RESULT thr_Err = ErrProcess.SetErrResult(err, (short)status);
                    MessageBox.Show(String.Format(m_Err.message + "\nInspection Stopped."), "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                    Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(String.Format("Cam Thread - " + err.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                    Close();
                }
                Thread.Sleep(100);
            }
        }

        //카메라 Close
        public ERR_RESULT Close()
        {
            ERR_RESULT m_Err = new ERR_RESULT();
            //카메라 오픈 체크 스레드
            try
            {
                ConnectThread.Abort();
                ConnectThread.Join(1000);
            }
            catch (ThreadStateException)
            {

            }
            catch (NullReferenceException)
            {

            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
            try
            {
                //카메라 닫기
                foreach (var cam in CameraList)
                {
                    if (cam.isAcqStart)
                    {
                        AcqStop();
                    }
                    if (cam.IsOpen)
                    {
                        _vfg.CloseDevice(cam.HDevice);
                    }
                }
                CameraList.Clear();
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        #region Acquisition Start
        public ERR_RESULT AcqStart()
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (CameraList.Count == 0) throw new _CamException(-107);
                foreach (var cam in CameraList)
                {
                    if (cam.IsOpen && !cam.isAcqStart)
                    {
                        status = _vfg.AcqStart(cam.HDevice);
                        if
                            (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-107);
                        else
                            cam.isAcqStart = true;
                    }
                }

                return m_Err;
            }
            catch (_CamException err)
            {
                AcqStop();
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
            catch (Exception err)
            {
                AcqStop();
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT AcqStart(int idx)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (CameraList[idx].IsOpen)
                {
                    status = _vfg.AcqStart(CameraList[idx].HDevice);
                    if
                        (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-107);
                    else
                        CameraList[idx].isAcqStart = true;
                }
                else
                {
                    throw new _CamException(-107);
                }
                return m_Err;
            }
            catch (_CamException err)
            {
                AcqStop(idx);
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
            catch (Exception err)
            {
                AcqStop(idx);
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT AcqStart(string userID)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (CameraList.Single(a => a.UserID == userID).IsOpen)
                {
                    status = _vfg.AcqStart(CameraList.Single(a => a.UserID == userID).HDevice);
                    if
                        (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-107);
                    else
                        CameraList.Single(a => a.UserID == userID).isAcqStart = true;
                }
                else
                {
                    throw new _CamException(-107);
                }
                return m_Err;
            }
            catch (_CamException err)
            {
                AcqStop(userID);
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
            catch (Exception err)
            {
                AcqStop(userID);
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }
        #endregion

        #region Acquisition Stop
        public ERR_RESULT AcqStop()
        {
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                foreach (var cam in CameraList)
                {
                    if (cam.IsOpen)
                    {
                        if (cam.isAcqStart)
                        {
                            _vfg.AcqStop(cam.HDevice);
                            cam.isAcqStart = false;
                        }
                    }
                }
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT AcqStop(int idx)
        {
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (CameraList[idx].IsOpen)
                {
                    if (CameraList[idx].isAcqStart)
                    {
                        _vfg.AcqStop(CameraList[idx].HDevice);
                        CameraList[idx].isAcqStart = false;
                    }
                }
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT AcqStop(string userID)
        {
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (CameraList.Single(a => a.UserID == userID).IsOpen)
                {
                    if (CameraList.Single(a => a.UserID == userID).isAcqStart)
                    {
                        _vfg.AcqStop(CameraList.Single(a => a.UserID == userID).HDevice);
                        CameraList.Single(a => a.UserID == userID).isAcqStart = false;
                    }
                }
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }
        #endregion

        #region Grab Software Trigger
        public ERR_RESULT GrabSWTrg()
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                foreach (var cam in CameraList)
                {
                    while (!cam.IsOpen)
                    {
                        //카메라 연결 끊겼을 시, 어떻게 할 지 구현
                        //재연결 코드는 스레드로 돌아가기에 여기서 구현할 필요는 없음
                        Thread.Sleep(50);
                    }
                    status = _vfg.SetCmdReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                    if(status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                    {
                        for(int i = 0; i<20; i++)
                        {
                            status = _vfg.SetCmdReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                            if (status == VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                        }
                    }
                    //status = _vfg.GrabImageAsync(cam.HDevice, cam.pImage, (uint)cam.BufferSize, 0xFFFFFFFF);
                    status = _vfg.GrabImage(cam.HDevice, cam.pImage, (uint)cam.BufferSize);

                    if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                    {
                        throw new _CamException(-108);
                    }

                    if (ConvertBitmap(cam).errCode != 0) throw new _CamException(-109);  //*******이미지 Converting 실패
                }
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT GrabSWTrg(int idx)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                var cam = CameraList[idx];

                while (!cam.IsOpen)
                {
                    //카메라 연결 끊겼을 시, 어떻게 할 지 구현
                    //재연결 코드는 스레드로 돌아가기에 여기서 구현할 필요는 없음
                    Thread.Sleep(50);
                }
                status = _vfg.SetCmdReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        status = _vfg.SetCmdReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                        if (status == VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                }
                //status = _vfg.GrabImageAsync(cam.HDevice, cam.pImage, (uint)cam.BufferSize, 0xFFFFFFFF);
                status = _vfg.GrabImage(cam.HDevice, cam.pImage, (uint)cam.BufferSize);

                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS && !cam.IsOpen)
                {
                    throw new _CamException(-108);
                }
                else if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS && cam.IsOpen)
                {
                    throw new _CamException(-110);
                }

                if (ConvertBitmap(cam).errCode != 0) throw new _CamException(-109);  //*******이미지 Converting 실패
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT GrabSWTrg(string userID)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                var cam = CameraList.Single(c => c.UserID.Equals(userID));

                while (!cam.IsOpen)
                {
                    //카메라 연결 끊겼을 시, 어떻게 할 지 구현
                    //재연결 코드는 스레드로 돌아가기에 여기서 구현할 필요는 없음
                }
                status = _vfg.SetCmdReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        status = _vfg.SetCmdReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOFTWARE);
                        if (status == VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                }
                //status = _vfg.GrabImageAsync(cam.HDevice, cam.pImage, (uint)cam.BufferSize, 0xFFFFFFFF);
                status = _vfg.GrabImage(cam.HDevice, cam.pImage, (uint)cam.BufferSize);

                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new _CamException(-108);
                }

                if (ConvertBitmap(cam).errCode != 0) throw new _CamException(-109);  //*******이미지 Converting 실패
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }
        #endregion

        #region Grab Line Trigger
        public ERR_RESULT GrabLineTrg()
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                foreach (var cam in CameraList)
                {
                    while (!cam.IsOpen)
                    {
                        //카메라 연결 끊겼을 시, 어떻게 할 지 구현
                        //재연결 코드는 스레드로 돌아가기에 여기서 구현할 필요는 없음
                        Thread.Sleep(50);
                    }

                    status = _vfg.GrabImageAsync(cam.HDevice, cam.pImage, (uint)cam.BufferSize, 0xFFFFFFFF);
                    //status = _vfg.GrabImage(cam.HDevice, cam.pImage, (uint)cam.BufferSize);

                    if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                    {
                        throw new _CamException(-108);
                    }

                    if (ConvertBitmap(cam).errCode != 0) throw new _CamException(-109);  //*******이미지 Converting 실패
                }
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }
        public ERR_RESULT GrabLineTrg(int idx)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                var cam = CameraList[idx];

                while (!cam.IsOpen)
                {
                    //카메라 연결 끊겼을 시, 어떻게 할 지 구현
                    //재연결 코드는 스레드로 돌아가기에 여기서 구현할 필요는 없음
                    Thread.Sleep(50);
                }
                //status = _vfg.GrabImageAsync(cam.HDevice, cam.pImage, (uint)cam.BufferSize, 0xFFFFFFFF);
                status = _vfg.GrabImage(cam.HDevice, cam.pImage, (uint)cam.BufferSize);

                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS && !cam.IsOpen)
                {
                    throw new _CamException(-108);
                }
                else if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS && cam.IsOpen)
                {
                    throw new _CamException(-110);
                }

                if (ConvertBitmap(cam).errCode != 0) throw new _CamException(-109);  //*******이미지 Converting 실패
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT GrabLineTrg(string userID)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                Camera cam = CameraList.Find(c => c.UserID.Equals(userID));
                if (cam.pImage == null) throw new Exception();

                while (!cam.IsOpen)
                {
                    //카메라 연결 끊겼을 시, 어떻게 할 지 구현
                    //재연결 코드는 스레드로 돌아가기에 여기서 구현할 필요는 없음
                    Thread.Sleep(50);
                }
                //ghdwnsrl
                //status = _vfg.GrabImageAsync(cam.HDevice, cam.pImage, (uint)cam.BufferSize, 0xFFFFFFFF);//여기서 맨날 주거
                status = _vfg.GrabImage(cam.HDevice, cam.pImage, (uint)cam.BufferSize);

                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS && !cam.IsOpen)
                {
                    throw new _CamException(-108);
                }
                else if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS && cam.IsOpen)
                {
                    throw new _CamException(-110);
                }

                if (ConvertBitmap(cam).errCode != 0) throw new _CamException(-109);  //*******이미지 Converting 실패
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }
        #endregion

        //Thread종료 후 재Open
        public ERR_RESULT Refresh()
        {
            ERR_RESULT m_Err = new ERR_RESULT();
            //Thread Abort
            try
            {
                ConnectThread.Abort();
                ConnectThread.Join(1000);
            }
            catch (ThreadStateException)
            {

            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }

            m_Err = Open();

            return m_Err;
        }

        private ERR_RESULT ConvertBitmap(Camera cam)
        {
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (cam.PixelFormat.Contains("Mono"))
                {
                    Int32 bitsPerPixel = System.Windows.Media.PixelFormats.Gray8.BitsPerPixel;
                    Int32 stride = (Int32)((cam.Width * bitsPerPixel + 7) / 8);
                    using (var bitmap = new Bitmap(cam.Width, cam.Height, stride, PixelFormat.Format8bppIndexed, cam.pImage))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                        bitmap.UnlockBits(bitmapData);
                        
                        // ktk1010911
                        if (cam.BitmapImage != null)
                        {
                            //cam.BitmapImage.Dispose();
                        }

                        cam.BitmapImage = bitmap.Clone() as Bitmap;
                        //cam.BitmapImage = bitmap;
                        cam.IsGrab = true;
                    }
                }
                else if (cam.PixelFormat.Contains("BayerBG"))
                {
                    using (var bitmap = new Bitmap(cam.Width, cam.Height, PixelFormat.Format24bppRgb))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                        _vfg.CvtColor(cam.pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.VirtualFG40Library.CV_BayerBG2RGB);

                        bitmap.UnlockBits(bitmapData);

                        // ktk1010911
                        if (cam.BitmapImage != null)
                        {
                            //cam.BitmapImage.Dispose();
                        }

                        cam.BitmapImage = bitmap.Clone() as Bitmap;
                        //cam.BitmapImage = bitmap;
                        cam.IsGrab = true;
                    }
                }
                else if (cam.PixelFormat.Contains("BayerGB"))
                {
                    using (var bitmap = new Bitmap(cam.Width, cam.Height, PixelFormat.Format24bppRgb))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                        _vfg.CvtColor(cam.pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.VirtualFG40Library.CV_BayerGB2RGB);
                        bitmap.UnlockBits(bitmapData);

                        // ktk1010911
                        if (cam.BitmapImage != null)
                        {
                            //cam.BitmapImage.Dispose();
                        }

                        cam.BitmapImage = bitmap.Clone() as Bitmap;//8.39, 11.54
                        //cam.BitmapImage = bitmap;
                        cam.IsGrab = true;
                    }
                }
                else if (cam.PixelFormat.Contains("BayerGR"))
                {
                    using (var bitmap = new Bitmap(cam.Width, cam.Height, PixelFormat.Format24bppRgb))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                        _vfg.CvtColor(cam.pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.VirtualFG40Library.CV_BayerGR2RGB);

                        bitmap.UnlockBits(bitmapData);

                        // ktk1010911
                        if (cam.BitmapImage != null)
                        {
                            //cam.BitmapImage.Dispose();
                        }

                        cam.BitmapImage = bitmap.Clone() as Bitmap;
                        //cam.BitmapImage = bitmap;
                        cam.IsGrab = true;
                    }
                }
                else if (cam.PixelFormat.Contains("BayerRG"))
                {
                    using (var bitmap = new Bitmap(cam.Width, cam.Height, PixelFormat.Format24bppRgb))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                        _vfg.CvtColor(cam.pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.VirtualFG40Library.CV_BayerRG2RGB);

                        bitmap.UnlockBits(bitmapData);

                        // ktk1010911
                        if (cam.BitmapImage != null)
                        {
                            //cam.BitmapImage.Dispose();
                        }

                        cam.BitmapImage = bitmap.Clone() as Bitmap;
                        //cam.BitmapImage = bitmap;
                        cam.IsGrab = true;
                    }
                }
                else if (cam.PixelFormat.Contains("YUV"))
                {
                    using (var bitmap = new Bitmap(cam.Width, cam.Height, PixelFormat.Format24bppRgb))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                        _vfg.CvtColor(cam.pImage, bitmapData.Scan0, bitmap.Width, bitmap.Height, VirtualFG40Library.VirtualFG40Library.CV_YUV2RGB_YUYV);

                        bitmap.UnlockBits(bitmapData);

                        // ktk1010911
                        if (cam.BitmapImage != null)
                        {
                            //cam.BitmapImage.Dispose();
                        }

                        cam.BitmapImage = bitmap.Clone() as Bitmap;
                        //cam.BitmapImage = bitmap;
                        cam.IsGrab = true;
                    }
                }
                else
                {
                    throw new _CamException(-106);
                }
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        //Dispose
        public void Dispose()
        {
            //캠 상시 오픈 체크 스레드 종료            
            Close();
            _vfg.FreeSystem();
        }

        //Set user ID
        public ERR_RESULT SetUserID(Camera device, string changeID)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                status = _vfg.SetStrReg(device.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_DEVICE_USER_ID, changeID);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS) throw new _CamException(-104);
                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT ToggleTriggerMode(bool isLineTrig)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                if (CameraList.Any(c => c.isAcqStart)) AcqStop();
                if (isLineTrig)
                {
                    foreach(var cam in CameraList)
                    {
                        status = _vfg.SetEnumReg(cam.HDevice,VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOURCE,VirtualFG40Library.VirtualFG40Library.TRIGGER_SOURCE_LINE1);
                        if(status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                        {
                            throw new _CamException(-104);
                        }
                    }
                }
                else
                {
                    foreach (var cam in CameraList)
                    {
                        status = _vfg.SetEnumReg(cam.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_TRIGGER_SOURCE, VirtualFG40Library.VirtualFG40Library.TRIGGER_SOURCE_SOFTWARE);
                        if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                        {
                            throw new _CamException(-104);
                        }
                    }
                }

                AcqStart();

                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        public ERR_RESULT SetGainRawValue(string camID, int value)
        {
            int status = 0;
            ERR_RESULT m_Err = new ERR_RESULT();
            try
            {
                Camera c = CameraList.Find(cam => cam.UserID == camID);

                status = _vfg.SetIntReg(c.HDevice, VirtualFG40Library.VirtualFG40Library.MCAM_GAIN_RAW, value);
                if (status != VirtualFG40Library.VirtualFG40Library.MCAM_ERR_SUCCESS)
                {
                    throw new _CamException(-104);
                }

                return m_Err;
            }
            catch (_CamException err)
            {
                m_Err = ErrProcess.SetErrResult(err, (short)status);
                return m_Err;
            }
            catch (Exception err)
            {
                m_Err = ErrProcess.SetErrResult(err);
                return m_Err;
            }
        }

        //-1009
    }

    public class Camera
    {
        public Int32 HDevice;
        public Int32 Width;
        public Int32 Height;
        public Int32 BufferSize;
        public bool IsOpen;
        public bool IsGrab;
        public bool isAcqStart;
        public IntPtr pImage;


        //public GCHandle Gch { get; set; }
        //public IntPtr UserData { get; set; }
        public String UserID { get; set; }
        public String DevID { get; set; }
        public String PixelFormat { get; set; }
        public Bitmap BitmapImage { get; set; }


        public Camera()
        {
            HDevice = -1;
            Width = 0;
            Height = 0;
            BufferSize = 0;
            IsOpen = false;
            IsGrab = false;
            isAcqStart = false;
            //Gch = new GCHandle();
            //UserData = new IntPtr();
            UserID = "";
            DevID = "";
            PixelFormat = "Mono";
        }
    }
}
