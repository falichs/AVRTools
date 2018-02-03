using AVRControl.Device.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AVRControl.Device.Network
{
    public enum AvrConnectivityState { DISCONNECTED, IDLE, TX, RX};

    public class AvrConnectivityEngine
    {
        AVRDevice mConnectedDevice = null;

        AvrConnectivityState mState = AvrConnectivityState.DISCONNECTED;

        public bool ConnectDevice(string host)
        {
            AVRDeviceDescriptor newDeviceDescribtor = new AVRDeviceDescriptor(host, "newDevice");
            AVRDevice newDevice = new AVRDevice(newDeviceDescribtor);
            // try to reach the device by requesting its friendlyname
            AvrPostCommand cmd = new AvrPostCommand(newDevice, new AvrGetPowerStatus());
            //cmd.ExecuteAsync();
            return true;
        }

        public bool DispatchCommand(Type CommandType)
        {
            if (mState == AvrConnectivityState.DISCONNECTED) { return false; }
            // pause device queries
            // execute command
            return true;
        }

        private bool DispatchDeviceQuery()
        {
            return true;
        }
    }

    public enum AvrCommandExecutionStatusCode
    {
        OK,
        BAD_RESPONSE
    }

    public class AvrCommandExecutionStatus
    {

        #region Fields

        private HttpStatusCode mHttpStatusCode;
        private AvrCommandExecutionStatusCode mExecutionStatus;

        #endregion

        #region Properties

        public HttpStatusCode HttpStatusCode
        {
            get { return mHttpStatusCode; }
            set { mHttpStatusCode = value; }
        }

        public AvrCommandExecutionStatusCode ExecutionStatus
        {
            get { return mExecutionStatus; }
            set { mExecutionStatus = value; }
        }

        #endregion

        #region Contructor

        public AvrCommandExecutionStatus(HttpStatusCode httpStatusCode, AvrCommandExecutionStatusCode executionStatus)
        {
            mHttpStatusCode = HttpStatusCode;
            mExecutionStatus = executionStatus;
        }

        #endregion

    }

    interface IAvrCommandInterface
    {
        Task<AvrCommandExecutionStatus> ExecuteAsync();
    }

    public abstract class AvrGetCommand : IAvrCommandInterface
    {
        #region Fields

        protected AVRDevice mAvrDevice = null;

        #endregion

        #region Properties

        protected abstract string CommandPath { get; }

        protected AVRDevice AvrDevice { get { return mAvrDevice; } }

        #endregion

        #region Constructor

        public AvrGetCommand(AVRDevice avrDevice)
        {
            mAvrDevice = avrDevice;
        }

        #endregion

        #region Execution

        public async Task<AvrCommandExecutionStatus> ExecuteAsync()
        {
            UriBuilder bob = new UriBuilder();
            bob.Host = mAvrDevice.DeviceDescribtor.Address.Host;
            bob.Path = CommandPath;
            AvrHttpGetClient httpClient = new AvrHttpGetClient();
            httpClient.Uri = bob.Uri;
            AvrHttpResponse response = await httpClient.SendHttpRequest();
            if (response.Status == HttpStatusCode.OK)
            {
                return new AvrCommandExecutionStatus(response.Status, AvrCommandExecutionStatusCode.OK);
            }
            else
            {
                return new AvrCommandExecutionStatus(response.Status, AvrCommandExecutionStatusCode.BAD_RESPONSE);
            }
                
        }

        #endregion

    }

    public class AvrCommandPowerOn : AvrGetCommand
    {
        private static string sCommandString = "goform/formiPhoneAppDirect.xml?PWON";

        #region Properties

        protected override string CommandPath
        {
            get { return sCommandString; }
        }

        #endregion

        #region Constructor

        public AvrCommandPowerOn(AVRDevice avrDevice) : base(avrDevice)
        {
        }

        #endregion

    }

    public class AvrCommandPowerStandby : AvrGetCommand
    {
        private static string sCommandString = "goform/formiPhoneAppDirect.xml?PWSTANDBY";

        #region Properties

        protected override string CommandPath
        {
            get { return sCommandString; }
        }

        #endregion
        
        #region Constructor

        public AvrCommandPowerStandby(AVRDevice avrDevice) : base(avrDevice)
        {
        }

        #endregion

    }


    public class AvrPostCommand : IAvrCommandInterface
    {
        protected static string sCommandPath = "/goform/AppCommand.xml";

        #region Fields

        private AVRDevice mAvrDevice = null;
        private IAvrPostCommandStrategy mCommandStrategy = null;

        #endregion

        #region Properties

        protected string XmlString
        {
            get { return "<tx>\n" + CommandStrategy.getXmlCommandString() + "</tx>"; }
        }

        protected IAvrPostCommandStrategy CommandStrategy
        {
            get { return mCommandStrategy; }
        }

        protected AVRDevice AvrDevice
        {
            get
            {
                return mAvrDevice;
            }
        }

        #endregion

        #region Constructor

        public AvrPostCommand(AVRDevice avrDevice, IAvrPostCommandStrategy commandStrategy)
        {
            mAvrDevice = avrDevice;
            mCommandStrategy = commandStrategy;
        }

        #endregion

        #region Execution

        public async Task<AvrCommandExecutionStatus> ExecuteAsync()
        {
            UriBuilder bob = new UriBuilder();
            bob.Host = mAvrDevice.DeviceDescribtor.Address.Host;
            bob.Path = sCommandPath;
            AvrHttpPostClient httpClient = new AvrHttpPostClient();
            httpClient.Uri = bob.Uri;
            httpClient.XmlString = XmlString;
            AvrHttpResponse response = await httpClient.SendHttpRequest();
            StringReader reader = new StringReader(response.ResponseString);
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(reader);
            }
            catch (Exception e)
            {
                return new AvrCommandExecutionStatus(response.Status, AvrCommandExecutionStatusCode.BAD_RESPONSE);
            }

            return new AvrCommandExecutionStatus(response.Status, CommandStrategy.ProcessXmlResponse(mAvrDevice, doc));
        }
        
        #endregion

    }

    public class AvrMacroCommandStrategy : IAvrPostCommandStrategy
    {

        #region Properties

        List<IAvrPostCommandStrategy> mStrategyList { get; set; }

        #endregion

        #region IAvrPostCommandStrategy

        public string getXmlCommandString()
        {
            StringBuilder bob = new StringBuilder();
            foreach(var strategy in mStrategyList)
            {
                bob.Append(strategy.getXmlCommandString());
            }
            return bob.ToString();
        }

        public AvrCommandExecutionStatusCode ProcessXmlResponse(AVRDevice target, XmlDocument xmlResponse)
        {
            bool ok = true;
            foreach (var strategy in mStrategyList)
            {
                ok &= (strategy.ProcessXmlResponse(target, xmlResponse) == AvrCommandExecutionStatusCode.OK);
            }
            return (ok ? AvrCommandExecutionStatusCode.OK : AvrCommandExecutionStatusCode.BAD_RESPONSE);
        }

        #endregion

    }

    public class AvrGetPowerStatus : IAvrPostCommandStrategy
    {
        public string getXmlCommandString()
        {
            return "<cmd id=\"1\">GetPowerStatus</cmd>";
        }

        public AvrCommandExecutionStatusCode ProcessXmlResponse(AVRDevice target, XmlDocument xmlResponse)
        {
            XmlNodeList list;
            
            bool parseOK = true;
            list = xmlResponse.GetElementsByTagName("power");
            if (list.Count > 0)
            {
                AVRDevicePowerStatus power;
                parseOK &= Enum.TryParse(list.Item(0).InnerText, true, out power);
                if (parseOK)
                {
                    target.DeviceData.PowerStatus = power;
                }
            }
            return (parseOK) ? AvrCommandExecutionStatusCode.OK : AvrCommandExecutionStatusCode.BAD_RESPONSE;
        }
    }

    public class AvrGetVolumeLevel : IAvrPostCommandStrategy
    {
        public string getXmlCommandString()
        {
            return "<cmd id=\"1\">GetVolumeLevel</cmd>";
        }

        public AvrCommandExecutionStatusCode ProcessXmlResponse(AVRDevice target, XmlDocument xmlResponse)
        {
            XmlNodeList list;
            bool success = true;
            float volume;
            string state;
            string limit;
            AVRVolumeDisplay disptype;
            float dispvalue;

            list = xmlResponse.GetElementsByTagName("volume");
            success &= list.Count > 0;
            if (success)
            {
                float.TryParse(list.Item(0).InnerText, out volume);
            } 
            list = xmlResponse.GetElementsByTagName("state");
            success &= list.Count > 0;
            if (success)
            {
                state = list.Item(0).InnerText;
            }
            list = xmlResponse.GetElementsByTagName("limit");
            success &= list.Count > 0;
            if (success)
            {
                limit = list.Item(0).InnerText;
            }
            list = xmlResponse.GetElementsByTagName("disptype");
            success &= list.Count > 0;
            if (success)
            {
                Enum.TryParse(list.Item(0).InnerText, true, out disptype);
            }
            list = xmlResponse.GetElementsByTagName("dispvalue");
            success &= list.Count > 0;
            if (success)
            {
                float.TryParse(list.Item(0).InnerText, out dispvalue);
            }

            return success?AvrCommandExecutionStatusCode.OK: AvrCommandExecutionStatusCode.BAD_RESPONSE;
        }
    }

    public interface IAvrPostCommandStrategy
    {
        string getXmlCommandString();
        AvrCommandExecutionStatusCode ProcessXmlResponse(AVRDevice target, XmlDocument xmlResponse);
    }

    public interface IAvrHttpClient
    {
        #region Properties

        Uri Uri { get; set; }

        #endregion

        #region Http Request

        Task<AvrHttpResponse> SendHttpRequest();

        #endregion
    }

    public class AvrHttpGetClient : IAvrHttpClient
    {

        #region Fields

        private Uri mUri = null;

        #endregion

        #region Properties

        public Uri Uri
        {
            get { return mUri; }
            set { mUri = value; }
        }

        #endregion

        #region IAvrHttpClient

        public async Task<AvrHttpResponse> SendHttpRequest()
        {
            WebRequest request = HttpWebRequest.Create(Uri);
            request.Method = "GET";

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

            string responseString = "";
            HttpStatusCode responseStatus = response.StatusCode;
            if (responseStatus != HttpStatusCode.OK)
            {
                response.Dispose();
                return new AvrHttpResponse(responseStatus, responseString);
            }

            // process the response
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseString = await reader.ReadToEndAsync();
            }
            response.Dispose();
            return new AvrHttpResponse(responseStatus, responseString);
        }

        #endregion
    }

    public class AvrHttpPostClient : IAvrHttpClient
    {

        #region Fields
        
        private Uri mUri = null;
        private string mXMLString;

        #endregion

        #region Properties

        public Uri Uri
        {
            get { return mUri; }
            set { mUri = value; }
        }

        public string XmlString
        {
            get { return mXMLString; }
            set { mXMLString = value; }
        }

        #endregion

        #region IAvrHttpClient

        public async Task<AvrHttpResponse> SendHttpRequest()
        {
            WebRequest request = HttpWebRequest.Create(Uri);
            request.Method = "POST";
            request.ContentType = "text/xml";
            Stream stream = await request.GetRequestStreamAsync();
            
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(mXMLString);
                streamWriter.Flush();
            }

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            
            string responseString = "";
            HttpStatusCode responseStatus = response.StatusCode;
            if (responseStatus != HttpStatusCode.OK)
            {
                response.Dispose();
                return new AvrHttpResponse(responseStatus, responseString);
            }

            // process the response
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseString = await reader.ReadToEndAsync();
            }
            response.Dispose();
            return new AvrHttpResponse(responseStatus, responseString);
        }

        #endregion
    }

    public class AvrHttpResponse
    {

        #region Fields

        private HttpStatusCode mStatus = HttpStatusCode.NotFound;
        private string mResponseString;
        
        #endregion
        
        #region Properties

        public HttpStatusCode Status
        {
            get { return mStatus; }
        }

        public string ResponseString
        {
            get { return mResponseString; }
        }

        #endregion

        #region Constructor

        public AvrHttpResponse(HttpStatusCode status, string responseString)
        {
            mStatus = status;
            mResponseString = responseString;
        }

        #endregion
    }
}
