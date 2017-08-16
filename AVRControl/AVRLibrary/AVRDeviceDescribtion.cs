using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AVRLibrary
{


    public class AVRDeviceDescribtion
    {


        public static readonly string XML_FRIENDLYNAME_STR = "friendlyName";
        public static readonly string XML_MANUFACTURER_STR = "manufacturer";
        public static readonly string XML_MODELNUMBER_STR = "modelNumber";
        public static readonly string XML_SERIALNUMBER_STR = "serialNumber";

        private string _friendlyName;


        public string FriendlyName
        {
            get { return _friendlyName; }
        }


        private string _manufacturer;


        public string Manufacturer
        {
            get { return _manufacturer; }
        }


        private string _modelNumber;


        public string ModelNumber
        {
            get { return _modelNumber; }
        }


        private string _serialNumber;


        public string SerialNumber
        {
            get { return _serialNumber; }
        }


        private AVRDeviceDescribtion(string friendlyName, string manufacturer, string modelNumber, string serialNumber)
        {
            _friendlyName = friendlyName;
            _manufacturer = manufacturer;
            _modelNumber = modelNumber;
            _serialNumber = serialNumber;
        }


        public static AVRDeviceDescribtion ReadFromXDocument(XDocument document)
        {
            if (document.Root == null) return null;

            XNamespace ns = document.Root.Name.Namespace;
            XElement deviceElement = document.Root.Element(ns + "device");
            string friendlyName = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_FRIENDLYNAME_STR)?.Value;
            string manufacturer = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MANUFACTURER_STR)?.Value;
            string modelNumber = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MODELNUMBER_STR)?.Value;
            string serialNumber = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_SERIALNUMBER_STR)?.Value;

            if (friendlyName == null || manufacturer == null || modelNumber == null || serialNumber == null)
            {
                return null;
            }
            return new AVRDeviceDescribtion(friendlyName, manufacturer, modelNumber, serialNumber);
        }


    }


}