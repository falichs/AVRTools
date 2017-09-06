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
                } }
        }

        private RelayCommand _scanAction;

        public RelayCommand ScanAction
        {
            get { return _scanAction; }
            set { _scanAction = value; }
        }

        public MainViewModel()
        {
            _connection = new AVRConnection();
            _connection.AVRDevicAdded += ConnectionOnAvrServiceAddedEvent;
            _devices = new ObservableCollection<AVRDevice>();
            _scanAction = new RelayCommand(x => { _connection.StartScanning(); });
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
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
