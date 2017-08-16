using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FalichsLib;
using Mono.Ssdp;


namespace AVRLibrary
{


    public class AVRConnection : IDisposable
    {
        #region Constants


        private const string DEVICE_STR = "device";

        private static HashSet<string> SUPPORTED_MODELS = new HashSet<string>() {"AVR-X1300W"};


        #endregion


        protected bool _disposed = false;
        private readonly object mutex_AVRservice = new object();

        private Dictionary<string, AVRService> _avrServices = null;
        private HttpClient _httpClient = null;
        private Client _ssdpClient = null;


        public Dictionary<string, AVRService> AvrServices { get; set; }


        public AVRConnection()
        {
            _httpClient = new HttpClient();
            _ssdpClient = new Client();
            //_ssdpClient.Browse("urn:schemas-upnp-org:device:MediaRenderer:1", false);
            _ssdpClient.BrowseAll(false);
            _avrServices = new Dictionary<string, AVRService>();
            initializeEventHandlers();
        }


        private void initializeEventHandlers()
        {
            _ssdpClient.ServiceAdded += SsdpClientOnServiceAdded;
            _ssdpClient.ServiceUpdated += SsdpClientOnServiceUpdated;
            _ssdpClient.ServiceRemoved += SsdpClientOnServiceRemoved;
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
                if (!_avrServices.ContainsKey(serviceArgs.Usn))
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
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "SSdp Service added:" + serviceArgs.Service.Locations.First() + "," + serviceArgs.Service.ServiceType);
            lock (mutex_AVRservice)
            {
                if (_avrServices.ContainsKey(serviceArgs.Service.Usn))
                {
                    return;
                }
            }
            CheckSsdpService(serviceArgs.Service.Usn, serviceArgs.Service.GetLocation(0));
        }


        private async void CheckSsdpService(string usn, string location)
        {
            var httpResponseMessage = await _httpClient.GetAsync(location.Trim());
            // check the response
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var response = await httpResponseMessage.Content.ReadAsStreamAsync();
                XDocument rxDocument = XDocument.Load(response);
                var describtion = AVRDeviceDescribtion.ReadFromXDocument(rxDocument);
                if (describtion == null)
                {
                    return;
                }
                FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "adding AVR: " + describtion.FriendlyName);
                lock (mutex_AVRservice)
                {
                    if (_avrServices.ContainsKey(usn))
                    {
                        return;
                    }
                    _avrServices.Add(usn, new AVRService(usn, describtion));
                }
            }
        }


        public void StartScanning()
        {
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Scanning for AVR Devices ...");
            _ssdpClient.Start(true);
            Task.Delay(4000).ContinueWith( x => { StopScanning(); });
        }


        public void StopScanning()
        {
            FalichsLogger.Instance.log(FalichsLogger.Severity.INFO, "Stop scanning for AVR Devices ...");
            _ssdpClient.Stop(true);
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
                _ssdpClient?.Dispose();
                _disposed = true;
            }
        }


    }


}