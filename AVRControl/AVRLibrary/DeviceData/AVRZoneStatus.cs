using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AVRLibrary.DeviceData
{


    public class AVRZoneStatus
    {
        #region Properties


        private string _ZoneName;


        public string ZoneName
        {
            get { return _ZoneName;}
            set { _ZoneName = value; }
        }

        private AVRZonePowerStatus _power;


        public AVRZonePowerStatus Power
        {
            get { return _power; }
            set
            {
                _power = value;
            }
        }


        private AVRInputSource _activeInputSource = null;


        public AVRInputSource ActiveInputSource
        {
            get { return _activeInputSource; }
            set
            {
                if (_activeInputSource == null || !(_activeInputSource.Equals(value)))
                {
                    _activeInputSource = value;
                }
            }
        }


        private AVRVolumeDisplay _volumeDisplay;


        public AVRVolumeDisplay VolumeDisplay
        {
            get { return _volumeDisplay; }
            set
            {
                _volumeDisplay = value;
            }
        }


        private AVRVolume _masterVolume;


        public AVRVolume MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                _masterVolume = value;
            }
        }


        private AVRMuteStatus _muteStatus;


        public AVRMuteStatus MuteStatus
        {
            get { return _muteStatus; }
            set
            {
                _muteStatus = value;
            }
        }


        #endregion


        public AVRZoneStatus()
        {
            _power = AVRZonePowerStatus.OFF;
            _muteStatus = AVRMuteStatus.MUTE_OFF;
            _masterVolume = new AVRVolume();
            _volumeDisplay = AVRVolumeDisplay.Absolute;
        }


        #region Conversion


        public bool FromResponse(XDocument responseDocument)
        {
            if (responseDocument == null || responseDocument.Root == null)
            {
                return false;
            }

            XNamespace ns = responseDocument.Root.Name.Namespace;
            foreach (XElement itemElement in responseDocument.Elements(ns + "item"))
            {
                XElement powerElement = itemElement.Element(ns + "ZonePower");
                if (powerElement != null && !powerElement.IsEmpty)
                {
                    string powerStat = powerElement.Element(ns + "value")?.Value;
                    AVRZonePowerStatus status;
                    if (Enum.TryParse(powerStat, true, out status))
                    {
                        Power = status;
                    }
                }
                XElement inputFuncSelectElement = itemElement.Element(ns + "InputFuncSelect");
                if (inputFuncSelectElement != null && !inputFuncSelectElement.IsEmpty)
                {
                    //AVRInputSource src = device.InputSources.First((x) => x.Name.Equals(inputFuncSelectElement.Element(ns + "value")?.Value));
                    //if (src != null)
                    //{
                    //    ActiveInputSource = src;
                    //}
                    AVRInputSource src = new AVRInputSource();
                    src.Name = inputFuncSelectElement.Element(ns + "value")?.Value;
                    ActiveInputSource = src;
                }
                XElement volumeDisplayElement = itemElement.Element(ns + "VolumeDisplay");
                if (volumeDisplayElement != null && !volumeDisplayElement.IsEmpty)
                {
                    string volumeDisplayString = volumeDisplayElement.Element(ns + "value")?.Value;
                    AVRVolumeDisplay volumeDisplay;
                    if (Enum.TryParse(volumeDisplayString, true, out volumeDisplay))
                    {
                        VolumeDisplay = volumeDisplay;
                    }
                }
                XElement masterVolumeElement = itemElement.Element(ns + "MasterVolume");
                if (masterVolumeElement != null && !masterVolumeElement.IsEmpty)
                {
                    string masterVolumeString = masterVolumeElement.Element(ns + "value")?.Value;
                    float masterVolume;
                    if (float.TryParse(masterVolumeString, out masterVolume))
                    {
                        if (VolumeDisplay == AVRVolumeDisplay.Absolute)
                        {
                            MasterVolume.AbsoluteVolume = masterVolume;
                        }
                        else
                        {
                            MasterVolume.RelativeVolume = masterVolume;
                        }
                    }
                }
                XElement muteElement = itemElement.Element(ns + "Mute");
                if (muteElement != null && !muteElement.IsEmpty)
                {
                    string muteStat = muteElement.Element(ns + "value")?.Value;

                    MuteStatus = muteStat.Contains("on") ? AVRMuteStatus.MUTE_ON : AVRMuteStatus.MUTE_OFF;
                }
            }
            return true;
        }


        #endregion
        
    }


}