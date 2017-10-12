using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace AVRLibrary.DeviceData
{


    public class AVRVolume
    {


        private const float _absoluteRangeMin = -80.0f;
        private const float _absoluteRangeMax = 20.0f;
        private float _absoluteVolume;
        private float _relativeVolume;


        public float AbsoluteVolumeMin { get { return _absoluteRangeMin; } }
        public float AbsoluteVolumeMax { get { return _absoluteRangeMax; } }
        public float AbsoluteVolume
        {
            get { return _absoluteVolume; }
            set
            {
                _absoluteVolume = value;
                _relativeVolume = AbsoluteToRelativeVolume(_absoluteVolume);
            }
        }


        public float RelativeVolume
        {
            get { return _relativeVolume; }
            set
            {
                _relativeVolume = value;
                _absoluteVolume = RelativeToAbsoluteVolume(_relativeVolume);
            }
        }


        public AVRVolume()
        {
            _relativeVolume = 0.0f;
            _absoluteVolume = _absoluteRangeMin;
        }

        #region VolumeConversion


        private float AbsoluteToRelativeVolume(float absoluteVolume)
        {
            return (_absoluteVolume - _absoluteRangeMin);
        }


        private float RelativeToAbsoluteVolume(float relativeVolume)
        {
            return (_absoluteRangeMin + relativeVolume);
        }


        #endregion

        
    }


}