using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVRLibrary.DeviceData;


namespace AVRLibrary
{
    public class AVRDevice
    {


        

        private readonly string _usn = null;


        public string Usn { get { return _usn;} }


        private readonly AVRDeviceDescribtion _avrDeviceDescribtion;

        public AVRDeviceDescribtion AvrDeviceDescribtion
        {
            get { return _avrDeviceDescribtion; }
        }


        private AVRDeviceData _deviceData = null;


        public AVRDeviceData DeviceData {
            get { return _deviceData; }
            set { UpdateDeviceData(value); }
        }


        public AVRDevice(string usn, AVRDeviceDescribtion deviceDescribtion)
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
