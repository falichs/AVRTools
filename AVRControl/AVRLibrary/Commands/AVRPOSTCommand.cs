using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AVRLibrary.Commands
{


    public abstract class AVRPOSTCommand
    {


        protected const string RESPONSE_STR = "rx";
        protected const string COMMAND_STR = "cmd";

        private int _ID;


        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }


        public AVRPOSTCommand()
        {
            _ID = 1;
        }


        public abstract string CommandString { get; }
        public abstract void ProcessResponse(XDocument info, AVRDevice device);


        public XElement ToXElement()
        {
            return new XElement(COMMAND_STR, new XAttribute("id", ID), CommandString);
        }


    }


}