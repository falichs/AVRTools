using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using AVRLibrary.Commands;
using AVRLibrary.DeviceData;
using FalichsLib;
using Mono.Ssdp;


namespace AVRLibrary
{


    public delegate void DeviceAddedDelegate(object sender, AVRDevice avrDevice);


    public delegate void DeviceUpdatedDelegate(object sender, AVRDevice avrDevice);


    public class AVRConnection : IDisposable
    {
        #region Constants


        private const string DEVICE_STR = "device";

        private static HashSet<string> SUPPORTED_MODELS = new HashSet<string>() {"AVR-X1300W"};
        
        private const uint INTERVALL_UPDATE_DEVICEDATA = 500;


        #endregion


        #region Fields


        protected bool _disposed = false;
        private readonly object mutex_AVRservice = new object();
        private Dictionary<string, AVRDevice> _avrServices = null;
        private AVRDevice _connectedAvrDevice = null;
        private static HttpClient _httpClient = new HttpClient();
        private static Client _ssdpClient = new Client();
        private System.Timers.Timer _updateTimer;
        private bool _freshConnection = true;


        public event DeviceAddedDelegate AVRDevicAdded;
        public event DeviceUpdatedDelegate AVRDeviceUpdated;


        #endregion


        #region Properties


        public Dictionary<string, AVRDevice> AVRDevices
        {
            get { return _avrServices; }
        }


        #endregion


        #region Constructor


        public AVRConnection()
        {
            _avrServices = new Dictionary<string, AVRDevice>();

            initializeEventHandlers();

            _ssdpClient.Browse("urn:schemas-upnp-org:device:MediaRenderer:1", false);
            _ssdpClient.Start(true);
            
            _updateTimer = new System.Timers.Timer();
            _updateTimer.Interval = INTERVALL_UPDATE_DEVICEDATA;
            _updateTimer.AutoReset = true;
            _updateTimer.Elapsed += UpdateTimerOnElapsed;
            
            
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Application Initialized");
        }


        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            UpdateDeviceData();
        }


        #endregion


        #region Initialization


        private void initializeEventHandlers()
        {
            _ssdpClient.ServiceAdded += SsdpClientOnServiceAdded;
            _ssdpClient.ServiceUpdated += SsdpClientOnServiceUpdated;
            _ssdpClient.ServiceRemoved += SsdpClientOnServiceRemoved;
        }


        #endregion


        #region SSDP


        //private void Tc(object state)
        //{
        //    _ssdpClient.Start(true);
        //    Task.Delay(1000).ContinueWith(x => { StopScanning(); });
        //}


        private void SsdpClientOnServiceRemoved(object sender, ServiceArgs serviceArgs)
        {
            if (serviceArgs?.Service == null)
            {
                return;
            }
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "SSDP Service removed:" + serviceArgs.Service.Locations.First());
            lock (mutex_AVRservice)
            {
                if (!_avrServices.ContainsKey(serviceArgs.Service.Usn))
                {
                    return;
                }
                //remove service
                _avrServices.Remove(serviceArgs.Service.Usn);
            }
        }


        private void SsdpClientOnServiceUpdated(object sender, ServiceArgs serviceArgs)
        {
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "SSDP Service updated:" + serviceArgs.Service.Locations.First());
        }


        private void SsdpClientOnServiceAdded(object sender, ServiceArgs serviceArgs)
        {
            if (serviceArgs?.Service == null)
            {
                return;
            }
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "SSdp Service found:" + serviceArgs.Service.Locations.First() + "," + serviceArgs.Service.ServiceType);
            lock (mutex_AVRservice)
            {
                if (_avrServices.ContainsKey(serviceArgs.Service.Usn))
                {
                    return;
                }
            }
            GetDeviceDescribtionAsync(serviceArgs.Service.Usn, serviceArgs.Service.GetLocation(0));
        }


        //public void StartScanning()
        //{
        //    FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Scanning for AVR Devices ...");
        //    _ssdpClient.Start(true);
        //    Task.Delay(4000).ContinueWith(x => { StopScanning(); });
        //}


        //private void StopScanning()
        //{
        //    FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Stop scanning for AVR Devices ...");
        //    _ssdpClient.Stop(true);
        //}


        #endregion


        #region Commands


        public async Task UpdateDeviceData()
        {
            if (_connectedAvrDevice == null)
            {
                return;
            }
            if (_freshConnection)
            {
                _freshConnection = false;
            }
            else
            {
                await PostCommand(new AVRCommandVolume(_connectedAvrDevice.DeviceData.MainZoneStatus.MasterVolume.AbsoluteVolume));
            }
            PostCommand(new AVRCommandMainZoneStatus());
            AVRDeviceUpdated(this, _connectedAvrDevice);
        }


        public void SendDevicePowerCommand(AVRDevicePowerStatus status)
        {
            PostCommand(new AVRCommandDevicePower(status));
        }
        public void SendZonePowerCommand(AVRZonePowerStatus status)
        {
            PostCommand(new AVRCommandMainZonePower(status));
        }
        public void SendMuteCommand(AVRMuteStatus status)
        {
            PostCommand(new AVRCommandMute(status));
        }


        public void SendVolumeCommand(float targetVolume)
        {
            PostCommand(new AVRCommandVolume(targetVolume));
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Sending Volume " + targetVolume);
        }
        #endregion


        #region HTTP Command Interfaces


        //public async void PostCommands(IEnumerable<AVRPOSTCommand> commands)
        //{
        //        if (_connectedAvrDevice == null) return;
        //        XElement txElement = new XElement("tx");
        //        foreach (var command in commands)
        //        {
        //            txElement.Add(command.ToXElement());
        //        }
        //        string hostURL = _connectedAvrDevice.AvrDeviceDescribtion.PresentationUrl;
        //        if (hostURL == null)
        //        {
        //            FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Device host URL invlaid.");
        //            return;
        //        }
        //        XDocument txDocument = new XDocument();
        //        txDocument.Add(txElement);
        //        txDocument.Declaration = new XDeclaration("1.0", "utf-8", "yes");
        //        string s = txDocument.Declaration + "\r\n" + txDocument.ToString(SaveOptions.None);
        //        HttpContent content = new StringContent(s, Encoding.UTF8, "text/xml");
        //        var httpResponseMessage = await _httpClient.PostAsync(hostURL, content);

        //        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        //        {
        //            var response = await httpResponseMessage.Content.ReadAsStreamAsync();
        //            XDocument rxDocument = XDocument.Load(response);
        //            foreach (var command in commands)
        //            {
        //                command.ProcessResponse(rxDocument, _connectedAvrDevice);
        //            }
        //            response.Close();
        //        }
        //        else
        //        {
        //            FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Command Failed. StatusCode: " + httpResponseMessage.StatusCode);
        //        }
        //}


        //public void PostCommand(AVRPOSTCommand command)
        //{
        //    PostCommands(new List<AVRPOSTCommand>() {command});
        //}


        public async Task PostCommand(AVRGETCommand command)
        {
            if (_connectedAvrDevice == null)
            {
                return;
            }
            string hostURL = _connectedAvrDevice.AvrDeviceDescribtion.PresentationUrl;
            if (hostURL == null)
            {
                FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Device host URL invlaid.");
                return;
            }
            Uri uri = new Uri(hostURL + "/" + command.CommandString);
            FalichsLogger.Instance.log(FalichsLogger.Severity.DEBUG, "Sending command " + uri);
            var httpResponseMessage = await _httpClient.GetAsync(uri);

            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var response = await httpResponseMessage.Content.ReadAsStreamAsync();
                if (response.Length>0)
                {
                    XDocument rxDocument = XDocument.Load(response);
                    if (!command.ProcessResponse(rxDocument, _connectedAvrDevice))
                    {
                        FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Error parsing command response.");
                    }
                }
                
                response.Close();
            }
            else
            {
                FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Command Failed. StatusCode: " + httpResponseMessage.StatusCode);
            }
        }


        #endregion


        protected virtual void OnAVRServiceAdded(AVRDevice e)
        {
            if (AVRDevicAdded != null && e != null)
            {
                AVRDevicAdded(this, e);
            }
        }

        private async void GetDeviceDescribtionAsync(string usn, string location)
        {
            string url = location.Trim();
            var httpResponseMessage = await _httpClient.GetAsync(url);
            // check the response
            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var response = await httpResponseMessage.Content.ReadAsStreamAsync();
            XDocument rxDocument = XDocument.Load(response);
            var describtion = AVRDeviceDescribtion.ReadFromXDocument(rxDocument);
            if (describtion == null)
            {
                return;
            }
            AVRDevice avrDevice = null;
            lock (mutex_AVRservice)
            {
                if (_avrServices.ContainsKey(usn))
                {
                    return;
                }
                avrDevice = new AVRDevice(usn, describtion);
                _avrServices.Add(usn, avrDevice);
            }
            OnAVRServiceAdded(avrDevice);
        }


        public bool CennectToDevice(AVRDevice device)
        {
            if (device?.Usn == null)
            {
                return false;
            }
            bool success = _avrServices.TryGetValue(device.Usn, out _connectedAvrDevice);
            if (success)
            {
                FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Connected to " + device?.AvrDeviceDescribtion?.FriendlyName);
                _freshConnection = true;
                UpdateDeviceData();
                _updateTimer.Start();
            }
            else
            {
                FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Failed to connected to " + device?.AvrDeviceDescribtion?.FriendlyName);
            }
            return success;
        }


        #region Finalizer


        ~AVRConnection()
        {
            Dispose(false);
        }
        #endregion

        #region IDisposable


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _updateTimer.Stop();
                _updateTimer.Close();
                _updateTimer = null;
                _httpClient?.Dispose();
                _httpClient = null;
                _ssdpClient?.Stop(true);
                _ssdpClient?.Dispose();
                _ssdpClient = null;
                AVRDevicAdded = null;
                _disposed = true;
            }
        }


        #endregion
    }


}