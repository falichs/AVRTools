using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FalichsLib;
using Mono.Ssdp;


namespace AVRLibrary
{


    public delegate void DeviceAddedDelegate(object sender, AVRDevice avrDevice);

    public class AVRConnection : IDisposable
    {
        #region Constants


        private const string DEVICE_STR = "device";

        private static HashSet<string> SUPPORTED_MODELS = new HashSet<string>() {"AVR-X1300W"};


        #endregion


        protected bool _disposed = false;
        private readonly object mutex_AVRservice = new object();

        private Dictionary<string, AVRDevice> _avrServices = null;

        public Dictionary<string, AVRDevice> AVRDevices { get { return _avrServices; } }

        private AVRDevice _connectedAvrDevice = null;

        private static HttpClient _httpClient = new HttpClient();
        private static Client _ssdpClient = new Client();


        public event DeviceAddedDelegate AVRDevicAdded;


        public AVRConnection()
        {
            _ssdpClient.Browse("urn:schemas-upnp-org:device:MediaRenderer:1", false);
            _avrServices = new Dictionary<string, AVRDevice>();
            TimerCallback tc = Tc;
            Timer scanTimer = new Timer(tc);
            initializeEventHandlers();
        }


        private void Tc(object state)
        {
            _ssdpClient.Start(true);
            Task.Delay(1000).ContinueWith(x => { StopScanning(); });
        }


        private void initializeEventHandlers()
        {
            _ssdpClient.ServiceAdded += SsdpClientOnServiceAdded;
            _ssdpClient.ServiceUpdated += SsdpClientOnServiceUpdated;
            _ssdpClient.ServiceRemoved += SsdpClientOnServiceRemoved;
        }


        protected virtual void OnAVRServiceAdded(AVRDevice e)
        {
            if (AVRDevicAdded != null && e != null)
            {
                AVRDevicAdded(this, e);
            }
        }


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


        private async void GetDeviceDescribtionAsync(string usn, string location)
        {
            var httpResponseMessage = await _httpClient.GetAsync(location.Trim());
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


        public void StartScanning()
        {
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Scanning for AVR Devices ...");
            _ssdpClient.Start(true);
            Task.Delay(4000).ContinueWith(x => { StopScanning(); });
        }


        private void StopScanning()
        {
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Stop scanning for AVR Devices ...");
            _ssdpClient.Stop(true);
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
            }
            else
            {
                FalichsLogger.Instance.log(FalichsLogger.Severity.WARNING, "Failed to connected to " + device?.AvrDeviceDescribtion?.FriendlyName);
            }
            return success;
        }


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
                _httpClient?.Dispose();
                _httpClient = null;
                _ssdpClient?.Dispose();
                _ssdpClient = null;
                AVRDevicAdded = null;
                _disposed = true;
            }
        }


    }


}