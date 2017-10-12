using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{
    public class AVRCommandMainZonePower : AVRGETCommand
    {

        private AVRZonePowerStatus _powerStatus;


        public AVRZonePowerStatus PowerStatus
        {
            get { return _powerStatus; }
        }



        public AVRCommandMainZonePower(AVRZonePowerStatus powerStatus)
        {
            _powerStatus = powerStatus;
        }


        public override string CommandString
        {
            get
            {
                string arguments = (_powerStatus == AVRZonePowerStatus.ON) ? "ZMON" : "ZMOFF";
                return "goform/formiPhoneAppDirect.xml?" + arguments;
            }
        }


        public override bool ProcessResponse(XDocument info, AVRDevice device)
        {
            return true;
        }
    }
}
