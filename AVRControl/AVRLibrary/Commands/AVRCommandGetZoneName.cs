using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{
    public class AVRCommandGetZoneName : AVRPOSTCommand
    {


        private const string ZONE_PATTERN = "zone";

        public override string CommandString { get { return "GetZoneName"; } }

        /*
         <cmd>
            <zone1>
            MAIN ZONE \r
            </zone1>
            <zone2>
            ZONE2     \r
            </zone2>
        </cmd>
        */

        public override void ProcessResponse(XDocument info, AVRDevice device)
        {
            if (info == null || info.Root == null) return;

            XNamespace ns = info.Root.Name.Namespace;
            XName s = ns + RESPONSE_STR;
            XElement responseElement = info.Element(s);

            if (responseElement != null && !responseElement.IsEmpty)
            {
                foreach (XElement commandElement in responseElement.Elements(ns + COMMAND_STR))
                {
                    foreach (XElement zoneElement in commandElement.Elements().Where(e => e.Name.LocalName.StartsWith(ZONE_PATTERN)))
                    {
                        //zoneElement.Value;
                    }
                }
            }
        }


    }
}
