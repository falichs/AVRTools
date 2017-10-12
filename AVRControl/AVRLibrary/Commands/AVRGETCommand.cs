using System.Xml.Linq;


namespace AVRLibrary.Commands
{
    public abstract class AVRGETCommand
    {


        public abstract string CommandString { get; }
        public abstract bool ProcessResponse(XDocument info, AVRDevice device);
    }
}
