using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AVRControl.Commands;
using AVRLibrary;
using AVRLibrary.DeviceData;
using FalichsLib;

namespace AVRControl
{
    public class MainViewModel :INotifyPropertyChanged
    {
        private object _mutex = new object();

        private AVRConnection _connection = null;

        private ObservableCollection<AVRDevice> _devices;
        
        public ObservableCollection<AVRDevice> Devices
        {
            get { return _devices; }
            set { _devices = value; }
        }

        private AVRDevice _selectedDevice;


        public AVRDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    _connection.CennectToDevice(_selectedDevice);
                    OnPropertyChanged();
                } }
        }

        private RelayCommand _powerCommand;

        public RelayCommand PowerCommand
        {
            get { return _powerCommand; }
            set { _powerCommand = value; }
        }


        private RelayCommand _muteCommand;

        public RelayCommand MuteCommand
        {
            get { return _muteCommand; }
            set { _muteCommand = value; }
        }

        public MainViewModel()
        {
            _connection = new AVRConnection();
            _connection.AVRDevicAdded += ConnectionOnAvrServiceAddedEvent;
            _connection.AVRDeviceUpdated += ConnectionOnAvrDeviceUpdated;
            _devices = new ObservableCollection<AVRDevice>();
            _powerCommand = new RelayCommand(x=>
            {
                //var status = _selectedDevice?.DeviceData?.PowerStatus;
                //if (status != null) { _connection.SendDevicePowerCommand((status == AVRDevicePowerStatus.ON)?AVRDevicePowerStatus.STANDBY:AVRDevicePowerStatus.ON);}
                var status = _selectedDevice?.DeviceData?.MainZoneStatus?.Power;
                if (status != null) { _connection.SendZonePowerCommand((status == AVRZonePowerStatus.ON) ? AVRZonePowerStatus.OFF : AVRZonePowerStatus.ON); }
            });
            _muteCommand = new RelayCommand(x => {
                var status = _selectedDevice?.DeviceData?.MainZoneStatus?.MuteStatus;
                if (status != null) { _connection.SendMuteCommand((status == AVRMuteStatus.MUTE_ON) ? AVRMuteStatus.MUTE_OFF : AVRMuteStatus.MUTE_ON); }
            });
        }


        private void ConnectionOnAvrDeviceUpdated(object sender, AVRDevice avrDevice)
        {
            OnPropertyChanged("SelectedDevice");
        }


        protected void ConnectionOnAvrServiceAddedEvent(object sender, AVRDevice avrDevice)
        {
            lock (_mutex)
            {
                Application.Current.Dispatcher.Invoke(() => { _devices.Clear(); });
                foreach (var dev in _connection.AVRDevices)
                {
                    Application.Current.Dispatcher.Invoke( () => { _devices.Add(avrDevice); });

                }
                if (_selectedDevice == null)
                {
                    Application.Current.Dispatcher.Invoke(() => { SelectedDevice = _devices?[0]; });
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
