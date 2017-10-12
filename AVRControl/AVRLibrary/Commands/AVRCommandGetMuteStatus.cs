using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{


    public class AVRCommandGetMuteStatus : AVRPOSTCommand
    {


        protected const string MUTE_STR = "mute";


        public override string CommandString
        {
            get { return "GetMuteStatus"; }
        }


        public override void ProcessResponse(XDocument info, AVRDevice device)
        {
            if (info == null || info.Root == null) return;

            XNamespace ns = info.Root.Name.Namespace;
            XElement responseElement = info.Element(ns + RESPONSE_STR);
            if (responseElement != null && !responseElement.IsEmpty)
            {
                foreach (XElement commandElement in responseElement.Elements(ns + COMMAND_STR))
                {
                    string muteStat = commandElement.Element(ns + MUTE_STR)?.Value;
                    AVRMuteStatus status;
                    if (AVRMuteStatus.TryParse(muteStat, true, out status))
                    {
                        //TODO
                        //device.MainZoneStatus.MuteStatus = status;
                    }
                }
            }
        }


    }


}