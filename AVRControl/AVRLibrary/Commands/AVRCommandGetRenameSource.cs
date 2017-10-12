using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{


    public class AVRCommandGetRenameSource : AVRPOSTCommand
    {


        private const string FUNCTIONRENAME_STR = "functionrename";
        private const string NAME_STR = "name";
        private const string RENAME_STR = "rename";


        public override string CommandString
        {
            get { return "GetRenameSource"; }
        }


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
                    XElement functionElement = commandElement.Element(ns + FUNCTIONRENAME_STR);
                    if (functionElement != null && !functionElement.IsEmpty)
                    {
                        foreach (var item in functionElement.Elements())
                        {
                            AVRInputSource renameSource = new AVRInputSource()
                            {
                                Name = item.Element(ns + NAME_STR)?.Value,
                                Renamed = item.Element(ns + RENAME_STR)?.Value
                            };
                            //TODO
                            /*
                            if (!device.InputSources.Contains(renameSource))
                            {
                                device.InputSources.Add(renameSource);
                            }
                            */
                            
                        }
                    }
                }
            }
        }


    }


}