using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{

    /// <summary>
    /// Queries the active Source
    /// </summary>
    public class AVRCommandGetSourceStatus : AVRPOSTCommand
    {


        private const string SOURCE_STR = "source";


        public override string CommandString
        {
            get { return "GetSourceStatus"; }
        }


        public override void ProcessResponse(XDocument info, AVRDevice device)
        {
            if (info?.Root == null) return;

            XNamespace ns = info.Root.Name.Namespace;
            XElement responseElement = info.Element(ns + RESPONSE_STR);
            if (responseElement != null && !responseElement.IsEmpty)
            {
                foreach (XElement commandElement in responseElement.Elements(ns + COMMAND_STR))
                {
                    XElement sourceElement = commandElement.Element(ns + SOURCE_STR);
                    if (sourceElement != null)
                    {
                        //TODO
                        /*
                        AVRInputSource src = device.InputSources.First((x)=> x.Name.Equals(sourceElement.Value));
                        if (src != null)
                        {
                            device.MainZoneStatus.ActiveInputSource = src;
                        }
                        */
                    }
                }
            }
        }


    }


}