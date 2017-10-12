using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AVRLibrary.Commands
{
    public class AVRCommandGetAllZoneVolume : AVRPOSTCommand
    {


        public override string CommandString { get { return "GetAllZoneVolume"; } }
        public override void ProcessResponse(XDocument info, AVRDevice device)
        {
            throw new NotImplementedException();
        }


    }
}
