using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AVRLibrary.DeviceData;


namespace AVRLibrary.Commands
{
    public class AVRCommandVolume:AVRGETCommand
    {
        private float _targetVolume;

        public float TargetVolume
        {
            get { return _targetVolume; }
        }



        //GET /goform/formiPhoneAppVolume.xml?1+-45.5 HTTP/1.1 
        //GET /goform/formiPhoneAppDirect.xml?MV###
        public override string CommandString
        {
            get
            {
                string arguments = _targetVolume.ToString("F1");
                return "goform/formiPhoneAppVolume.xml?1+" + arguments;
            }
        }


        public AVRCommandVolume(float targetVolume)
        {
            _targetVolume = targetVolume;
        }
        /*
                <item>
                    <VolumeDisplay>
                        <value>
                        Absolute
                        </value>
                    </VolumeDisplay>
                    <MasterVolume>
                        <value>
                        -50.5
                        </value>
                    </MasterVolume>
                    <Mute>
                        <value>
                        off
                        </value>
                    </Mute>
                </item>
        
                     */
        public override bool ProcessResponse(XDocument info, AVRDevice device)
        {
            //this response actually delivers the old value ... bullcrap
            //return device.MainZoneStatus.FromResponse(info);
            return true;
        }


    }
}
