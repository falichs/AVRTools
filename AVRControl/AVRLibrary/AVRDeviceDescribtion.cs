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
        public static readonly string XML_MANUFACTURER_URL_STR = "manufacturerURL";
        public static readonly string XML_MODEL_DESCRIBTION_STR = "modelDescription";
        public static readonly string XML_MODEL_NAME_STR = "modelName";
        public static readonly string XML_MODEL_NUMBER_STR = "modelNumber";
        public static readonly string XML_MODEL_URL_STR = "modelURL";
        public static readonly string XML_SERIALNUMBER_STR = "serialNumber";
        public static readonly string XML_UDN_STR = "UDN";
        public static readonly string XML_UPC_STR = "UPC";

        private readonly string _friendlyName;


        public string FriendlyName
        {
            get { return _friendlyName; }
        }


        private readonly string _manufacturer;


        public string Manufacturer
        {
            get { return _manufacturer; }
        }


        private readonly string _manufacturerUrl;


        public string ManufacturerUrl
        {
            get { return _manufacturerUrl; }
        }


        private readonly string _modelDescribtion;


        public string ModelDescribtion
        {
            get { return _modelDescribtion; }
        }


        private readonly string _modelName;


        public string ModelName
        {
            get { return _modelName; }
        }


        private readonly string _modelNumber;


        public string ModelNumber
        {
            get { return _modelNumber; }
        }


        private readonly string _modelUrl;


        public string ModelUrl
        {
            get { return _modelUrl; }
        }


        private readonly string _serialNumber;


        public string SerialNumber
        {
            get { return _serialNumber; }
        }


        private readonly string _udn;


        public string Udn
        {
            get { return _udn; }
        }


        private readonly string _upc;


        public string Upc
        {
            get { return _upc; }
        }


        private AVRDeviceDescribtion(string friendlyName, 
            string manufacturer, 
            string manufacturerUrl, 
            string modelDescribtion, 
            string modelName, 
            string modelNumber, 
            string modelUrl, 
            string serialNumber, 
            string udn, 
            string upc)
        {
            _friendlyName = friendlyName;
            _manufacturer = manufacturer;
            _manufacturerUrl = manufacturerUrl;
            _modelDescribtion = modelDescribtion;
            _modelName = modelName;
            _modelNumber = modelNumber;
            _modelUrl = modelUrl;
            _serialNumber = serialNumber;
            _udn = udn;
            _upc = upc;
        }


        public AVRDeviceDescribtion(AVRDeviceDescribtion other)
        {
            _friendlyName = other.FriendlyName;
            _manufacturer = other.Manufacturer;
            _manufacturerUrl = other.ManufacturerUrl;
            _modelDescribtion = other.ModelDescribtion;
            _modelName = other.ModelName;
            _modelNumber = other.ModelNumber;
            _modelUrl = other.ModelUrl;
            _serialNumber = other.SerialNumber;
            _udn = other.Udn;
            _upc = other.Upc;
        }

        public static AVRDeviceDescribtion ReadFromXDocument(XDocument document)
        {
            if (document.Root == null) return null;

            XNamespace ns = document.Root.Name.Namespace;
            XElement deviceElement = document.Root.Element(ns + "device");
            string friendlyName = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_FRIENDLYNAME_STR)?.Value;
            string manufacturer = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MANUFACTURER_STR)?.Value;
            string manufaturerUrl = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MANUFACTURER_URL_STR)?.Value;
            string modelDescribtion = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MODEL_DESCRIBTION_STR)?.Value;
            string modelName = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MODEL_NAME_STR)?.Value;
            string modelNumber = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MODEL_NUMBER_STR)?.Value;
            string modelUrl = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_MODEL_URL_STR)?.Value;
            string serialNumber = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_SERIALNUMBER_STR)?.Value;
            string udn = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_UDN_STR)?.Value;
            string upc = deviceElement?.Element(ns + AVRDeviceDescribtion.XML_UPC_STR)?.Value;

            if (friendlyName == null || 
                manufacturer == null || 
                manufaturerUrl == null || 
                modelDescribtion == null ||
                modelName == null || 
                modelNumber == null || 
                modelUrl == null || 
                serialNumber == null || 
                udn == null || 
                upc == null)
            {
                return null;
            }
            return new AVRDeviceDescribtion(friendlyName, 
                manufacturer, 
                manufaturerUrl, 
                modelDescribtion, 
                modelName, 
                modelNumber, 
                modelUrl, 
                serialNumber, 
                udn, 
                upc);
        }


    }


}