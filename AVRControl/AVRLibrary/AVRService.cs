using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVRLibrary
{
    public class AVRService
    {
        

        private string _usn = "";


        public string Usn { get { return _usn;} }


        private AVRDeviceDescribtion _avrDeviceDescribtion;

        public AVRDeviceDescribtion AvrDeviceDescribtion
        {
            get { return _avrDeviceDescribtion; }
        }


        private AVRDeviceData _deviceData = null;


        public AVRDeviceData DeviceData {
            get { return _deviceData; }
            set { UpdateDeviceData(value); }
        }


        public AVRService(string usn, AVRDeviceDescribtion deviceDescribtion)
        {
            _usn = usn;
            _avrDeviceDescribtion = deviceDescribtion;
            _deviceData = new AVRDeviceData();
        }

        public void UpdateDeviceData(AVRDeviceData deviceData)
        {
            _deviceData.CopyFrom(deviceData);
        }


    }
}
