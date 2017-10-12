using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{
    public class AVRCommandMainZoneStatus : AVRGETCommand
    {

        ///goform/formMainZone_MainZoneXmlStatusLite.xml
        public override string CommandString { get { return "goform/formMainZone_MainZoneXml.xml"; } }
        public override bool ProcessResponse(XDocument info, AVRDevice device)
        {
            return device.DeviceData.FromResponse(info);
        }


    }
}
