using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVRLibrary.DeviceData
{
    public class AVRInputSource
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _name.Trim();
            }
        }


        private string _renamed;

        public string Renamed
        {
            get { return _renamed; }
            set
            {
                _renamed = value;
                _renamed.Trim();
            }
        }


        public override bool Equals(object obj)
        {
            AVRInputSource other = obj as AVRInputSource;

            if (other != null && _name.Equals(other.Name))
            {
                return true;
            }
            return false;
        }


        public override string ToString()
        {
            return Renamed;
        }


    }
}
