using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DeepObjectDector.sub.lib.err
{

    public delegate void EventHandler(ERR_RESULT result);

    public struct ERR_RESULT
    {
        public string funcName;
        public Int16  errCode;
        public Int32? Inner_errCode; //라이브러리나 다른 에러코드
        public string message;
        public string errTrace;
    }

    public class ErrProcessOrigin : Exception
    {

        /*
         *  field
         */
        protected Int16 m_ErrCode;
        
        /*
         *  property
         */
        public Int16 ErrCode
        {
            get { return m_ErrCode; }
        }

        /*
         *  delegate
         */
        public event EventHandler ActionCallback;

        /*
         * constructor
         */
        public ErrProcessOrigin()
        {
            m_ErrCode = 0;
            ActionCallback = null;
            m_ErrCode = -1;
        }

        public ErrProcessOrigin(Int16 errCode)
        {
            ActionCallback = null;
            m_ErrCode = errCode;
        }

        static public ERR_RESULT SetErrResult(Exception err, Int16? InnerErr = null)
        {
            ERR_RESULT result = new ERR_RESULT();
            String[] ErrTrace = new String[255];
            
            ExtractErrTrace(err.StackTrace, ref ErrTrace);
            int pos = ErrTrace.Length;
            String ErrType = err.GetType().ToString();

            result = inner_SetErrInform(err, result, ErrTrace, InnerErr);
            if (result.errCode != 0)
                return result;

            return result;
        }

        static public ERR_RESULT SetErrResult_UserMessage(Exception err, Int16? InnerErr = null, String msg = null)
        {
            ERR_RESULT result = new ERR_RESULT();
            String[] ErrTrace = new String[255];

            ExtractErrTrace(err.StackTrace, ref ErrTrace);


            
            result = inner_SetErrInform(err, result, ErrTrace, InnerErr, msg);
            if (result.errCode != 0)
                return result;
            return result;
        }

        //rjm8282
        static private ERR_RESULT inner_SetErrInform(Exception err, ERR_RESULT result, string[] ErrTrace, Int16? InnerErr = null, String msg = null)
        {
            int pos = ErrTrace.Length;
            String[] errType = err.GetType().ToString().Split('.');
            ErrProcessOrigin ep = err as ErrProcessOrigin;

            if (ep == null) //예상외의 오류 
            {
                ep = new ErrProcess(-10); // 재정의
                result.funcName = ErrTrace[pos - 2]; // 호출된 함수 Name
                result.errTrace = ErrTrace[pos - 1]; // 에러 위치
                result.message = inner_GetFactoryMsg(ep);
                //ep.GetErrMessage();
                result.Inner_errCode = InnerErr;
                result.errCode = ep.ErrCode;
            }

            else // 정의되어있는 오류 
            {
                result.funcName = ErrTrace[pos -2];      // 호출된 함수 Name
                result.errTrace = ErrTrace[pos -1];      // 에러 위치
                result.message = inner_GetFactoryMsg(ep);
                result.Inner_errCode = InnerErr;
                result.errCode = ep.ErrCode;
            }
            return result; 
        }
        private static String inner_GetFactoryMsg(ErrProcessOrigin ep)
        {
            //String[] errType = ep.GetType().ToString().Split('.');

            //ep 상속된 클래스 비교
            if (ep is ErrProcess)
            {
                return ((ErrProcess)ep).GetErrMessage();
            }
            else if (ep is ErrProcessXml)
            {
                return ((ErrProcessXml)ep).GetErrMessage();
            }
            else
            {
                return "Func:inner_GetFactoryMsg ErrCode -2";
            }
        }
        private static void ExtractErrTrace(String errTrace, ref String[] extractData)
        {
            String[] token = new String[1] { "\r\n" };
            extractData = (errTrace).Split(token, StringSplitOptions.RemoveEmptyEntries);
            int pos = extractData.Length;
            String str = extractData[pos - 1];
            token = new String[2] { " 위치: ", "파일 " };
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
    }

    //rjm8282  '~' 발견시 사용할 구조체 정의
    

    enum ErrCodeType
    {
        STD     = 0,
        MULTI   = 1,
        RANGE,
    }

    struct ErrInfo
    {
        public ErrCodeType type;
        public short min;
        public short max;
        public short range;
    }

    public class ErrProcessXml : ErrProcessOrigin
    {
        /*
       *  field
       */
        //rjm0000 xml 파싱을 위한 변수 생성
        //1.경로 생성
        string m_path = Environment.CurrentDirectory + "\\resource\\ErrCode_xml\\ErrCode.xml";
        //2. xml 로드
        XmlDocument m_root = new XmlDocument();
        XmlNodeList targetNode;
        //errCode를 담을 dictionary 생성
        Dictionary<short, string> m_ErrDic;
        List<ErrInfo> m_infoList = new List<ErrInfo>();

        //constructor
        //rjm8282 생성자가 불릴 때마다 xml에서 에러코드를 딕셔너리에 저장
        public ErrProcessXml()
        {
            //errcode xml 파싱
            m_root.Load(m_path);
            //targetNode에 리스트 형식으로 xml 값들 저장
            targetNode = m_root.SelectNodes("Err/ErrCode/errMessage");
            // 파싱한 값 저장(딕셔너리)
            m_ErrDic = new Dictionary<short, string>();

            // rjm 8282 190404
            // 파싱한 값에 ',', '~'가 있을 경우 
            // ',' : string 분리 후 dictionary 따로 생성 뒤 같은 메세지를 value로
            // '~' : string 분리 후 min값으로 dictionary에 저장, min, max값으로  구조체에 범위 저장.
            foreach (XmlNode err in targetNode)
            {
                inner_GetErr(err);
                //short ErrNum = short.Parse(err.Attributes["code"].Value);
                //string ErrMsg = err.Attributes["msg"].Value;
                //m_ErrDic.Add(ErrNum, ErrMsg);
            }
        }

        public ErrProcessXml(Int16 errCode) : base(errCode)
        {
            //errcode xml 파싱
            m_root.Load(m_path);
            //targetNode에 리스트 형식으로 xml 값들 저장
           
            targetNode = m_root.SelectNodes("Err/ErrCode/errMessage");
            // 파싱한 값 저장(딕셔너리)
            m_ErrDic = new Dictionary<short, string>();

            // rjm 8282 190404
            // 파싱한 값에 ',', '~'가 있을 경우 
                // ',' : string 분리 후 dictionary 따로 생성 뒤 같은 메세지를 value로
                // '~' : string 분리 후 min, max값으로 구조체에 저장.
            foreach (XmlNode err in targetNode)
            {
                inner_GetErr(err);
                //short ErrNum = short.Parse(err.Attributes["code"].Value);
                //string ErrMsg = err.Attributes["msg"].Value;
                //m_ErrDic.Add(ErrNum, ErrMsg);
            }
        }

        private void inner_GetErr(XmlNode err)
        {
            ErrCodeType codeType = new ErrCodeType();
            //1. xml 파싱 후 안에 있는 특수문자 체크
            short ErrNum = 0;
            string ErrCode = err.Attributes["code"].Value;
            string ErrMsg = err.Attributes["msg"].Value;

            //1. code 값이 standard
            //2. code 값이 multi
            //3. code 값이 range
            //ErrCodeType 세팅
            if (ErrCode.Contains('~'))
            {
                codeType = ErrCodeType.RANGE;
            }
            else if (ErrCode.Contains(','))
            {
                codeType = ErrCodeType.MULTI;
            }
            else
            {
                codeType = ErrCodeType.STD;
            }//rjm9999 이후 switch case문으로 변경

            //2. switch case문을 ErrCodeType 이용한 ErrSetting
            switch (codeType)
            {
                case ErrCodeType.STD:
                    ErrNum = Convert.ToInt16(ErrCode);
                    m_ErrDic.Add(ErrNum, ErrMsg);
                    break;
                case ErrCodeType.MULTI:
                    //','를 기준으로 split
                    string[] multi = ErrCode.Split(',');
                    foreach (string single in multi)
                    {
                        ErrNum = Convert.ToInt16(single);
                        m_ErrDic.Add(ErrNum, ErrMsg);
                    }
                    break;
                case ErrCodeType.RANGE:
                    //'~'를 기준으로 split
                    string[] range = ErrCode.Split('~');
                    //ErrInfo에 min,max 저장
                    ErrInfo tmp = new ErrInfo();
                    tmp.type = codeType;
                    tmp.min = Convert.ToInt16(range[0]);
                    tmp.max = Convert.ToInt16(range[1]);
                    tmp.range = Convert.ToInt16(tmp.max - tmp.min);
                    m_infoList.Add(tmp);
                    //ErrInfo에 있는 min값을 key로 설정하여 dictionary에 ADD
                    m_ErrDic.Add(tmp.min, ErrMsg);
                    break;
            }
        }

        //rjm8282 m_ErrCode를 통해 xml을 parsing한 값을 참조하는 구조로 변경
        public String GetErrMessage()
        {

            String errMessage = null;
            short tmp_ErrCode;


            switch (m_ErrDic.ContainsKey(m_ErrCode))
            {
                case true:
                    errMessage = m_ErrDic[m_ErrCode];
                    break;
                case false:
                    tmp_ErrCode = inner_CheckRange(m_ErrCode);
                    errMessage = m_ErrDic[tmp_ErrCode];
                    break;
            }

            return errMessage;
        }

        private short inner_CheckRange(short ErrCode)
        {
            short tmp = 0;

            foreach (ErrInfo info in m_infoList)
            {
                if (info.min <= ErrCode && info.max >= ErrCode)
                    tmp = info.min;
            }

            return tmp;
        }
    }
    public class _XmlException : ErrProcessXml
    {
        public _XmlException()
        { }

        public _XmlException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _WorkSpaceViewException : ErrProcessXml
    {
        public _WorkSpaceViewException()
        { }

        public _WorkSpaceViewException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _MainException : ErrProcessXml
    {
        public _MainException()
        { }

        public _MainException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _DocException : ErrProcessXml
    {
        public _DocException()
        { }

        public _DocException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _ImageListViewException : ErrProcessXml
    {
        public _ImageListViewException()
        { }
        public _ImageListViewException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _ImageItemException : ErrProcessXml
    {
        public _ImageItemException()
        { }

        public _ImageItemException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _DODLib_EX_Exception : ErrProcessXml
    {
        public _DODLib_EX_Exception()
        { }

        public _DODLib_EX_Exception(Int16 errCode)
            : base(errCode)
        { }
    }
    public class Img_PicBoxData_EX_Exception : ErrProcessXml
    {
        public Img_PicBoxData_EX_Exception()
        { }

        public Img_PicBoxData_EX_Exception(Int16 errCode)
            : base(errCode)
        { }
    }
    public class _MarkerException : ErrProcessXml
    {
        public _MarkerException()
        { }

        public _MarkerException(Int16 errCode)
            : base(errCode)
        { }
    }
    public class ErrProcess : ErrProcessOrigin
    {
         
        
        /*
         *  constructor
         */
        public ErrProcess() { }
        public ErrProcess(Int16 errCode) : base(errCode) { }

        /*
         *  method 
         */
        public String GetErrMessage()
        {
            String errMessage = null;

            switch (m_ErrCode)
            {
                case 0:
                    errMessage = "Succes";
                    break;
                case -1:
                    errMessage = "Not specified";
                    break;
                case -2:
                    errMessage = "It has not been modified or implemented in the future.";
                    break;
                case -3:
                    errMessage = "InspecterResultErr";
                    break;
                case -4:
                    errMessage = "Inspect Stop";
                    break;
                case -10:
                    errMessage = "System Err";
                    break;
                case -11:
                    errMessage = "Connection Err";
                    break;
                case -12:
                    errMessage = "IP format Err";
                    break;
                case -100:
                    errMessage = "Empty xmlfile";
                    break;
                case -101:
                    errMessage = "Incorrect xmlfile";
                    break;
                case -102:
                    errMessage = "No element value in the xmlfile";
                    break;
                case -103:
                    errMessage = "Contains elements that are not defined node";
                    break;
                case -200:
                    errMessage = "Not Select TargetModule";
                    break;
                case -300:
                    errMessage = "Not Insert Channel Size";
                    break;
                case -301:
                    errMessage = "Module Channel Size 4 or more";
                    break;
                case -302:
                    errMessage = "Target Serial Num length Err";
                    break;
                case -303:
                    errMessage = "Xml Serial Num length Err";
                    break;
                case -330:
                    errMessage = "Channel Size Over Flow";
                    break;
                case -331:
                    errMessage = "Cali Step Size Over Flow";
                    break;
                case -334:
                    errMessage = "The MacId packet format fo the Rx Buffer is incorrect.";
                    break;
                case -335:
                    errMessage = "Switch Setvalue Size Over Flow";
                    break;
                case -336:
                    errMessage = "MacId format si not match";
                    break;
                case -337:
                    errMessage = "NA System Consumption Curr Err";
                    break;
                case -338:
                    errMessage = "NA Field Consumption Curr Err";
                    break;
                case -340:
                    errMessage = "System current consumption Check Err";
                    break;
                case -341:
                    errMessage = "Field current consumption Check Err";
                    break;
                case -342:
                    errMessage = "Channel Check Err";
                    break;
                case -343:
                    errMessage = "Extend Check Err";
                    break;
                case -344:
                    errMessage = "AI TC module CJC Check Err";
                    break;
                case -345:
                    errMessage = "TC Channel per Channel ErrRate Err";
                    break;
                case -400:
                    errMessage = "Not powSupply232Open Err";
                    break;
                case -401:
                    errMessage = "ReadTimeOut Err";
                    break;
                case -402:
                    errMessage = "IsConnectPowerSupply Err";
                    break;
                case -403:
                    errMessage = "Can not Change ChannelScope Err";
                    break;
                case -404:
                    errMessage = "CurrValue Not Same SetVoltageVal Err";
                    break;
                case -405:
                    errMessage = "CurrValue Not Same SetCurrentVal Err";
                    break;
                case -406:
                    errMessage = "VoltageOutPutON Err";
                    break;
                case -407:
                    errMessage = "It is already Pow Open Err";
                    break;
                case -408:
                    errMessage = " Set value Err";
                    break;
                case -450:
                    errMessage = "FnIOLib Err";
                    break;
                case -451:
                    errMessage = "FnIOLib OpenDevice Err";
                    break;
                case -500:
                    errMessage = "Ip format is not correct";
                    break;
                case -501:
                    errMessage = "Already FnIO Device is open";
                    break;
                case -502:
                    errMessage = "Not found module property type and can't work linker";
                    break;
                case -503:
                    errMessage = "Not found module category type and can't work linker";
                    break;
                case -504:
                    errMessage = "Not found module channel";
                    break;
                case -505:
                    errMessage = "Already FnIODevice is Init";
                    break;
                case -600:
                    errMessage = "Pannel meta is not open";
                    break;
                case -607:
                    errMessage = "It is already Pannel Meta Open Err";
                    break;
                case -608:
                    errMessage = " Set value Err";
                    break;
                case -609:
                    errMessage = " crc Check Err";
                    break;
                case -700:
                    errMessage = "Not Found LogFile";
                    break;
                case -701:
                    errMessage = "Inspectlog items are Not Found";
                    break;
                case -800:
                    errMessage = "Cali Open Err";
                    break;
                case -997:
                    errMessage = "Recv Err";
                    break;
                case -998:
                    errMessage = "Target Module Parameter Size Err";
                    break;
                case -1000:
                    errMessage = "Open Fail System err";
                    break;
                case -1001:
                    errMessage = "Free Fail System err";
                    break;
                case -1002:
                    errMessage = "Open Fail Device err";
                    break;
                case -1003:
                    errMessage = "Close Fail Device err";
                    break;
                case -1004:
                    errMessage = "IO Data Read Failure err";
                    break;
                case -1005:
                    errMessage = "IO Data Write Failure err";
                    break;
                case -1006:
                    errMessage = "Parameter Read err";
                    break;
                case -1007:
                    errMessage = "Make packet err";
                    break;
                case -1010:
                    errMessage = "Handle Value err";
                    break;
                case -999:
                default:
                    //Unknown Err
                    errMessage = "Unknown Err";
                    break;
            }
            return errMessage;
        }
    }
}
