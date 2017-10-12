using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{


    public class AVRCommandDevicePower : AVRGETCommand
    {


        //goform/formiPhoneAppPower.xml?1+PowerOn
        //goform/formiPhoneAppPower.xml?1+PowerStandby
        //goform/formiPhoneAppDirect.xml?PWON
        //goform/formiPhoneAppDirect.xml?PWSTANDBY

        private AVRDevicePowerStatus _powerStatus;


        public AVRDevicePowerStatus PowerStatus
        {
            get { return _powerStatus; }
        }

        

        public AVRCommandDevicePower(AVRDevicePowerStatus powerStatus)
        {
            _powerStatus = powerStatus;
        }


        public override string CommandString
        {
            get
            {
                string arguments = (_powerStatus == AVRDevicePowerStatus.ON) ? "PWON" : "PWSTANDBY";
                return "goform/formiPhoneAppDirect.xml?" + arguments;
            }
        }


        public override bool ProcessResponse(XDocument info, AVRDevice device)
        {
            return true;
        }


    }


}