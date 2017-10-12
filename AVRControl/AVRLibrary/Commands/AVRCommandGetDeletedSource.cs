using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AVRLibrary.Commands
{
    public class AVRCommandGetDeletedSource : AVRPOSTCommand
    {


        public override string CommandString { get { return "GetDeletedSource"; } }
        public override void ProcessResponse(XDocument info, AVRDevice device)
        {
            throw new NotImplementedException();
        }


    }
}
