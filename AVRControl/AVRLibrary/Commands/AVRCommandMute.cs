using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{
    public class AVRCommandMute : AVRGETCommand
    {
        private AVRMuteStatus _targetMuteStatus;

        public AVRMuteStatus TargetMuteStatus
        {
            get { return _targetMuteStatus; }
            set { _targetMuteStatus = value; }
        }

        //GET /goform/formiPhoneAppMute.xml?1+MuteOn HTTP/1.1 
        public override string CommandString
        {
            get
            {
                string arguments = (_targetMuteStatus == AVRMuteStatus.MUTE_ON) ? "MuteOn" : "MuteOff";
                return "goform/formiPhoneAppMute.xml?1+" + arguments;
            }
        }


        public AVRCommandMute(AVRMuteStatus targetMuteStatus)
        {
            _targetMuteStatus = (targetMuteStatus == AVRMuteStatus.UNDEFINED)? AVRMuteStatus.MUTE_OFF : targetMuteStatus;
        }

        /*
        <item>
            <Mute>
                <value>
                    on
                </value>
            </Mute>
        </item>
        */
        public override bool ProcessResponse(XDocument info, AVRDevice device)
        {
            //TODO
            //return device.MainZoneStatus.FromResponse(info, device);
            return true;
        }


    }
}
