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

        private ObservableCollection<AVRDeviceDescribtion> _deviceDescribtions;
        
        public ObservableCollection<AVRDeviceDescribtion> DeviceDescribtions
        {
            get { return _deviceDescribtions; }
            set { _deviceDescribtions = value; }
        }

        private AVRDeviceDescribtion _selectedDevice;


        public AVRDeviceDescribtion SelectedDevice
        {
            get { return _selectedDevice; }
            set {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    FalichsLogger.Instance.log(FalichsLogger.Severity.DEBUG, "selected device: " + _selectedDevice.FriendlyName);
                    // connect _connection.
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
            _deviceDescribtions = new ObservableCollection<AVRDeviceDescribtion>();
            _scanAction = new RelayCommand(x => { _connection.StartScanning(); });
        }


        protected void ConnectionOnAvrServiceAddedEvent(object sender, AVRDevice avrDevice)
        {
            lock (_mutex)
            {
                Application.Current.Dispatcher.Invoke(() => { _deviceDescribtions.Clear(); });
                foreach (var dev in _connection.AVRDevices)
                {
                    Application.Current.Dispatcher.Invoke( () => { _deviceDescribtions.Add(new AVRDeviceDescribtion(dev.Value.AvrDeviceDescribtion)); });

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
