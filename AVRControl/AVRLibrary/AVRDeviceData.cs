using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using AVRLibrary.DeviceData;


namespace AVRLibrary
{
    public class AVRDeviceData
    {
        private AVRDevicePowerStatus _powerStatus;


        public AVRDevicePowerStatus PowerStatus
        {
            get { return _powerStatus; }
        }

        private AVRZoneStatus _mainZoneStatus;


        public AVRZoneStatus MainZoneStatus
        {
            get { return _mainZoneStatus; }
        }
        

        public AVRDeviceData()
        {
            _mainZoneStatus = new AVRZoneStatus();
            _powerStatus = AVRDevicePowerStatus.STANDBY;
        }

        public void CopyFrom(AVRDeviceData deviceData)
        {
            _mainZoneStatus = deviceData.MainZoneStatus;
            _powerStatus = deviceData.PowerStatus;
        }


        public bool FromResponse(XDocument responseDocument)
        {
            if (responseDocument == null || responseDocument.Root == null)
            {
                return false;
            }

            XNamespace ns = responseDocument.Root.Name.Namespace;
            foreach (XElement itemElement in responseDocument.Elements(ns + "item"))
            {
                XElement powerElement = itemElement.Element(ns + "Power");
                if (powerElement != null && !powerElement.IsEmpty)
                {
                    string powerStat = powerElement.Element(ns + "value")?.Value;
                    AVRDevicePowerStatus status;
                    if (Enum.TryParse(powerStat, true, out status))
                    {
                        _powerStatus = status;
                    }
                }
            }
            return MainZoneStatus.FromResponse(responseDocument);
        }
    }
}
