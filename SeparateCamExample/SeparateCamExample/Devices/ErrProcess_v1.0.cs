using System;

namespace Crevis.Devices
{
    public delegate void EventHandler(ERR_RESULT result);

    public struct ERR_RESULT
    {
        public string funcName;
        public Int16 errCode;
        public Int16? Inner_errCode; //라이브러리나 다른 에러코드
        public string message;
        public string errTrace;
    }

    public class ErrProcess : Exception
    {
        #region Field
        private Int16 m_ErrCode;

        public const int ERR_SUCCESS = 0;
        public const int SYSTEM_ERR = -10;
        public const int CAM_INIT_FAILED = -100;
        public const int CAM_UPDATE_FAILED = -101;
        public const int CAM_OPEN_FAILED = -102;
        public const int CAM_GET_CAMCOUNT_FAILED = -103;
        public const int CAM_READ_REGISTER_FAILED = -104;
        public const int CAM_REOPEN_FAILED = -105;
        public const int CAM_PIXEL_FORMAT_ERR = -106;
        public const int CAM_AQUISITION_FAILED = -107;
        public const int CAM_GRAB_FAILED = -108;
        public const int CAM_CONVERTING_IMAGE_FAILED = -109;
        public const int CAM_GRAB_TIMEOUT = -110;

        public const int IO_INIT_FAILED = -200;
        public const int IO_INVALID_IP_ADDRESS_ERR = -201;
        public const int IO_OPEN_FAILED = -202;
        public const int IO_ACTIVE_SLOT_LOAD_FAILED = -203;
        public const int IO_UPDATE_FAILED = -204;
        public const int IO_WRITE_FAILED = -205;

        public const int TCP_CONNECTION_FAILED = -300;
        public const int TCP_WRONG_IP_ADDRESS = -301;
        public const int TCP_WRITE_FAILED = -302;
        public const int TCP_READ_FAILED = -303;

        public const int LIGHT_CONNECTION_FAILED = -400;
        public const int LIGHT_WRONG_IP_ADDRESS = -401;
        public const int LIGHT_SETTING_NULL_ERR = -402;

        public const int XML_LOAD_FAIL = -500;
        public const int XML_SAVE_FAIL = -501;
        #endregion

        #region Properties
        public Int16 ErrCode
        {
            get { return m_ErrCode; }
        }
        #endregion

        #region Delegates
        public event EventHandler ActionCallback;
        #endregion

        #region Constructor
        public ErrProcess()
        {
            ActionCallback = null;
            m_ErrCode = -1;
        }
        public ErrProcess(Int16 errCode)
        {
            ActionCallback = null;
            m_ErrCode = errCode;
        }
        #endregion

        #region Methods
        static public ERR_RESULT SetErrResult(Exception err, Int16? InnerErr = null)
        {
            ERR_RESULT result = new ERR_RESULT();
            String[] ErrTrace = new String[255];

            ExtractErrTrace(err.StackTrace, ref ErrTrace);
            int pos = ErrTrace.Length;
            //에러코드와 그에 따른 메세지를 받기위해 
            if (!(err is ErrProcess ep)) //예상외의 오류 
            {
                ep = new ErrProcess(-10); // 재정의
                if (ErrTrace.Length < 2)
                {
                    result.funcName = ErrTrace[0];      // 호출된 함수 Name
                    result.errTrace = ErrTrace[0];      // 에러 위치
                }
                else
                {
                    result.funcName = ErrTrace[pos - 2];      // 호출된 함수 Name
                    result.errTrace = ErrTrace[pos - 1];      // 에러 위치
                }
                result.message = ep.GetErrMessage() + err.Message;
                result.Inner_errCode = InnerErr;
                result.errCode = ep.ErrCode;
            }
            else // 정의되어있는 오류 
            {
                if (ErrTrace.Length < 2)
                {
                    result.funcName = ErrTrace[0];      // 호출된 함수 Name
                    result.errTrace = ErrTrace[0];      // 에러 위치
                }
                else
                {
                    result.funcName = ErrTrace[pos - 2];      // 호출된 함수 Name
                    result.errTrace = ErrTrace[pos - 1];      // 에러 위치
                }

                result.message = ep.GetErrMessage();
                result.Inner_errCode = InnerErr;
                result.errCode = ep.ErrCode;
            }
            return result;
        }
        static public ERR_RESULT SetErrResult_UserMessage(Exception err, Int16? InnerErr = null, String msg = null)
        {
            ERR_RESULT result = new ERR_RESULT();
            String[] ErrTrace = new String[255];

            ExtractErrTrace(err.StackTrace, ref ErrTrace);
            int pos = ErrTrace.Length;
            //에러코드와 그에 따른 메세지를 받기위해 
            if (!(err is ErrProcess ep)) //예상외의 오류 
            {
                ep = new ErrProcess(-10); // 재정의
                result.funcName = ErrTrace[pos - 2]; // 호출된 함수 Name
                result.errTrace = ErrTrace[pos - 1]; // 에러 위치
                result.message = msg;
                result.Inner_errCode = InnerErr;
                result.errCode = ep.ErrCode;
            }

            else // 정의되어있는 오류 
            {
                result.funcName = ErrTrace[pos - 2];      // 호출된 함수 Name
                result.errTrace = ErrTrace[pos - 1];      // 에러 위치
                result.message = msg;
                result.Inner_errCode = InnerErr;
                result.errCode = ep.ErrCode;
            }
            return result;
        }
        static private void ExtractErrTrace(String errTrace, ref String[] extractData)
        {
            String[] token = new String[1] { "\r\n" };
            extractData = (errTrace).Split(token, StringSplitOptions.RemoveEmptyEntries);
            int pos = extractData.Length;
            String str = extractData[pos - 1];
            token = new String[2] { " at ", "in " };
            extractData = (str).Split(token, StringSplitOptions.RemoveEmptyEntries);
        }

        public void SetErrCall(ERR_RESULT err)
        {
            if (ActionCallback == null)
                return;

            ActionCallback(err);
        }

        public void ResetErr()
        {
            m_ErrCode = -1;
        }

        public String GetErrMessage()
        {
            String errMessage = null;

            switch (m_ErrCode)
            {
                case 0:
                    errMessage = "Success.";
                    break;

                //System Exception
                case -10:
                    errMessage = "";
                    break;

                //Camera Err (-100 ~ -199)
                case -100:
                    errMessage = "CAM - System initialize failed.";
                    break;
                case -101:
                    errMessage = "CAM - Update device list failed.";
                    break;
                case -102:
                    errMessage = "CAM - Camera open failed.";
                    break;
                case -103:
                    errMessage = "CAM - Get camera count failed.";
                    break;
                case -104:
                    errMessage = "CAM - Read register failed.";
                    break;
                case -105:
                    errMessage = "CAM - Re-open failed.";
                    break;
                case -106:
                    errMessage = "CAM - Pixel format error.";
                    break;
                case -107:
                    errMessage = "CAM - Camera acquisition failed.";
                    break;
                case -108:
                    errMessage = "CAM - Camera grab failed.";
                    break;
                case -109:
                    errMessage = "CAM - Image converting failed.";
                    break;
                case -110:
                    errMessage = "CAM - Grab timeout.";
                    break;
                case -111:
                    errMessage = "CAM - Insufficient Cam than signed number.";
                    break;

                //IO Err (-200 ~ -299)
                case -200:
                    errMessage = "I/O - System initialize failed.";
                    break;
                case -201:
                    errMessage = "I/O - Invaild IP address.";
                    break;
                case -202:
                    errMessage = "I/O - FnIO open failed.";
                    break;
                case -203:
                    errMessage = "I/O - Active slot load failed.";
                    break;
                case -204:
                    errMessage = "I/O - Update device failed.";
                    break;
                case -205:
                    errMessage = "I/O - Bit writing failed.";
                    break;

                //TCP Err (-300 ~ -399)
                case -300:
                    //errMessage = "TCP - Connection failed.";
                    errMessage = "TCP - 로봇 연결 실패.";
                    break;
                case -301:
                    errMessage = "TCP - Invalid IP address.";
                    break;
                case -302:
                    errMessage = "TCP - Write failed.";
                    break;
                case -303:
                    errMessage = "TCP - Read failed.";
                    break;

                //Light Err (-400 ~ -499)                
                case -400:
                    errMessage = "Light - Connection failed.";
                    break;
                case -401:
                    errMessage = "Light - Invalid IP address.";
                    break;
                case -402:
                    errMessage = "Light - Setting is null.";
                    break;

                //XML Err (-500 ~ -501)
                case -500:
                    errMessage = "XML - Load failed";
                    break;
                case -501:
                    errMessage = "XML - Save failed";
                    break;

                default:
                    //Unknown Err
                    errMessage = "Unknown Err";
                    break;
            }

            return errMessage;
        }
        #endregion
    }

    #region Inherit ErrProcess
    public class _CamException : ErrProcess
    {
        public _CamException()
        { }

        public _CamException(Int16 errCode) : base(errCode)
        { }
    }
    public class _FnIOLibException : ErrProcess
    {
        public _FnIOLibException()
        { }

        public _FnIOLibException(Int16 errCode) : base(errCode)
        { }
    }
    public class _TCPException : ErrProcess
    {
        public _TCPException()
        { }

        public _TCPException(Int16 errCode) : base(errCode)
        { }
    }
    public class _LightException : ErrProcess
    {
        public _LightException()
        { }

        public _LightException(Int16 errCode) : base(errCode)
        { }
    }
    public class _InspectionException : ErrProcess
    {
        public _InspectionException()
        { }

        public _InspectionException(Int16 errCode) : base(errCode)
        { }
    }
    #endregion
}
